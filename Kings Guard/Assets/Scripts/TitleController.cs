using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitleController : MonoBehaviour
{   
    
    public Title P_1;                   // Data Object with title specifics traits
    private Animator animator_P1;       // Animator that controls what sprite plays
    private SpriteRenderer Sprite_P1;   // Renders a sprite (idk if I need this if the animator takes control)
    private Rigidbody2D body_P1;        // RigidBody used for Physics of the player

    void Awake()
    {
        // Sets up things i want to change based on title, prolly need to be required
        animator_P1 = GetComponent<Animator>();
        body_P1 = GetComponent<Rigidbody2D>();
        //Get the players title if it hasnt been overriden in unity
        Debug.Log(P_1);
        if (P_1 == null || P_1.TitleName == "TEST")
        {
            P_1 = TitleSelectorController.Player;
        }
    }

    private void Start()
    {
        GetDressed(P_1);
    }

    private void Update()
    {
    }

    private void GetDressed(Title title)
    {
        //Sets Gameobject attributres from the scriptable object
        animator_P1.runtimeAnimatorController = title.CharacterAnimator;
        title.Awake();
        title.GetHealth();
        Debug.Log("Youve picked the " + title.TitleName + ". Health: " + title.GetHealth());
    }
}

