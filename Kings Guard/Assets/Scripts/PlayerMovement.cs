using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private TitleController titleController;    // Data Object with title specifics traits
    private Animator animator_P1;               // Animator that controls what sprite plays
    private Rigidbody2D body_P1;                // RigidBody used for Physics of the player

    #region Player Inputs
    float horizontalInput;
    private bool jump = false;
    #endregion


    #region Grounding Varibles
    [SerializeField] private Vector2 groundBox = new Vector2(.48f, .18f);   // Box collider size to check for ground
    [SerializeField] private bool isGrounded;                               // Whether or not the player is grounded.
    [SerializeField] private LayerMask whatIsGround;                        // A layermask determining what is ground to the character
    [SerializeField] private Transform groundCheckCenter;                   // A position marking where to check if the player is grounded.
    [SerializeField] private Transform ceilingCheckCenter;                  // A position marking where to check for ceilings
    [SerializeField] private Collider2D crouchDisableCollider;              // A collider that will be disabled when crouching
    #endregion

    #region Movement Coefficents
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;
    private bool rightFacing = true;
    #endregion

    #region Crouch Varibles
    private bool crouch = false;                                        // Whether or not the player is crouching. 
    private bool wasCrouching = false;                                  // Not used because no crouch function/animation yet. Also i should use the flip logic to change crouch, not this
    [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;    // Amount of maxSpeed applied to crouching movement. 1 = 100%
    const float ceilingRadius = .34f;                                   // Radius of the overlap circle to determine if the player can stand up
    #endregion

    //This is allows to to subscribe functions to events
    [Header("Events")]
    [Space]

    // This tells the animator that the character has landed
    public UnityEvent OnLandEvent;

    public BoolEvent OnJumpEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnCrouchEvent;

    // Start is called before the first frame update
    void Start()
    {
        // Sets up things i want to change based on title, prolly need to be required
        animator_P1 = GetComponent<Animator>();
        body_P1 = GetComponent<Rigidbody2D>();
        titleController = GetComponent<TitleController>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnJumpEvent == null)
            OnJumpEvent = new BoolEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    // Update is called once per frame
    void Update()
    {
        //Is the player faceing the right way
        SetFaceDirection(ref rightFacing, horizontalInput);
  
        // Check to see if you can stand up
        CrouchCheck();

        //Allow movement input while grounded or if title has air control
        if (isGrounded || titleController.P_1.m_AirControl)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            animator_P1.SetFloat("Speed", Mathf.Abs(horizontalInput));
        }

        //Allow jump and crouch if grounded
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                jump = true;
                OnJumpEvent.Invoke(true);
            }

            // If crouching disable stuff (no crouch yet)
            if (crouch)
            {
                //If the crouch button was just pushed
                if (!wasCrouching)
                {
                    wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                horizontalInput *= crouchSpeed;

                // Disable one of the colliders when crouching
                if (crouchDisableCollider != null)
                    crouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (crouchDisableCollider != null)
                    crouchDisableCollider.enabled = true;

                if (wasCrouching)
                {
                    wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Applying new velocity to the character
        body_P1.velocity = Move(
            horizontalInput * Time.fixedDeltaTime,
            body_P1.velocity,
            movementSmoothing
        );

        // If the player jumps...
        if (jump)
        {
            body_P1.AddForce(Jump());
            isGrounded = false;
            jump = false;
        }

        //Ground Check
        bool wasGrounded = isGrounded;
        isGrounded = GroundCheck(wasGrounded);
    }


    public Vector3 Move(

        float move,                 // Direction
        Vector2 prevVelocity,       // Previous Velocity
        float smoothing   // Smoothing Coeffiecent 
    )
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * titleController.P_1.GetSpeed(), prevVelocity.y);
        // And then smoothing it out
        return Vector3.SmoothDamp(prevVelocity, targetVelocity, ref m_Velocity, smoothing);
    }

    public Vector2 Jump()
    {
        // Add a vertical force to the player based on stats
        return new Vector2(0f, titleController.P_1.GetJump());
    }

    private bool GroundCheck(bool wasGrounded)
    {
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.  
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            groundCheckCenter.position, //Center
            groundBox,
            0,
            whatIsGround
        );
        //Bug here the colliders always come back false when the character is on the ground
        for (int i = 0; i < colliders.Length; i++)
        {
            //If its not just overlapping with itself...
            if (colliders[i].gameObject != gameObject)
            {
                //... it is grounded
                if (!wasGrounded)
                {
                    //Land event to for land animation
                    OnLandEvent.Invoke();
                    OnJumpEvent.Invoke(false);
                }
                return true;
            }
        }
        return false;
    }

    private void CrouchCheck()
    {
        // If crouching, check to see if the character can stand up (no crouch yet)
        if (!crouch && wasCrouching)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (
                Physics2D.OverlapCircle(
                    ceilingCheckCenter.position,
                    ceilingRadius,
                    whatIsGround
                )
            )
            {
                //You can't stand up, you need to keep crouching
                crouch = true;
            }
        }
    }
    private void SetFaceDirection(ref bool rightFacing, float move)
    {
        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !rightFacing)
        {
            // ... flip the player.
            Flip(ref rightFacing);
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && rightFacing)
        {
            // ... flip the player.
            Flip(ref rightFacing);
        }
        void Flip(ref bool front)
        {
            // Switch the way the player is labelled as facing.
            front = !front;
            // Multiply the player's x local scale by -1.
            Vector3 theScale = gameObject.transform.localScale;
            theScale.x *= -1;
            gameObject.transform.localScale = theScale;
        }
    }

    private void OnDrawGizmos()
    {
        //Ground Box Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckCenter.position, groundBox);
    }
}
