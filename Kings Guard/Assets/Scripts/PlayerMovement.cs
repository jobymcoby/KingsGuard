using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 m_Velocity = Vector3.zero;

    public TitleController titleController;                   // Data Object with title specifics traits

    // Player Inputs
    float horizontalInput;
    private bool jump = false;
    private Animator animator_P1;       // Animator that controls what sprite plays
    private Rigidbody2D body_P1;        // RigidBody used for Physics of the player
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement


    private bool m_RightFacing = true;  // Whether or not the player is facing right.
    private bool crouch = false;        // Whether or not the player is crouching. 
    private bool m_wasCrouching = false;    // Not used because no crouch function/animation yet. Also i should use the flip logic to change crouch, not this

    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%

    [SerializeField] float k_GroundedRadius = .34f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = .05980872f;  // Radius of the overlap circle to determine if the player can stand up
    [SerializeField] private bool m_Grounded;                                   // Whether or not the player is grounded.
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching


    //IDK what happens here
    [Header("Events")]
    [Space]
    // This tells the animator that the character has landed
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnCrouchEvent;

    // Start is called before the first frame update
    void Start()
    {
        // Sets up things i want to change based on title, prolly need to be required
        animator_P1 = GetComponent<Animator>();
        body_P1 = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    // Update is called once per frame
    void Update()
    {
        //Is the player faceing the right way
        SetFaceDirection(ref m_RightFacing, horizontalInput);

        Debug.DrawRay(m_GroundCheck.position, Vector2.down * k_GroundedRadius);

        CrouchCheck();

        //Allow movement input while grounded or if title has air control
        if (m_Grounded || titleController.P_1.m_AirControl)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            animator_P1.SetFloat("Speed", Mathf.Abs(horizontalInput));
        }

        //Allow jump and crouch if grounded
        if (m_Grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space)) { jump = true; }

            // If crouching disable stuff (no crouch yet)
            if (crouch)
            {
                //If the crouch button was just pushed
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                horizontalInput *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
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
            m_MovementSmoothing
        );

        // If the player jumps...
        if (jump)
        {
            body_P1.AddForce(Jump());
            m_Grounded = false;
            jump = false;
        }

        //Ground Check
        bool wasGrounded = m_Grounded;
        m_Grounded = GroundCheck(wasGrounded);
    }

    public Vector3 Move(
        float move,                 // Direction
        Vector2 prevVelocity,       // Previous Velocity
        float m_MovementSmoothing   // Smoothing Coeffiecent 
    )
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * titleController.P_1.GetSpeed(), prevVelocity.y);
        // And then smoothing it out
        return Vector3.SmoothDamp(prevVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            m_GroundCheck.position, //Center
            k_GroundedRadius, //Radius
            m_WhatIsGround //Ground Layer
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
                }
                return true;
            }
        }
        return false;
    }

    private void CrouchCheck()
    {
        // If crouching, check to see if the character can stand up (no crouch yet)
        if (!crouch && m_wasCrouching)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (
                Physics2D.OverlapCircle(
                    m_CeilingCheck.position,
                    k_CeilingRadius,
                    m_WhatIsGround
                )
            )
            {
                //You can't stand up, you need to keep crouching
                crouch = true;
            }
        }
    }
    private void SetFaceDirection(ref bool m_RightFacing, float move)
    {
        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !m_RightFacing)
        {
            // ... flip the player.
            Flip(ref m_RightFacing);
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && m_RightFacing)
        {
            // ... flip the player.
            Flip(ref m_RightFacing);
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
}
