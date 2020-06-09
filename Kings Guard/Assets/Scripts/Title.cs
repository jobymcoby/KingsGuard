using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Title", menuName = "Titles")]
public class Title : ScriptableObject
{

    // This is basically a constructor for Titles, 
    // I want to use this to hold all the inputs and do all relavent calculations,
    // Then have the results referenced in TitleController

    // I want this to chnage the file name when i save.
    public string TitleName;
    // Move sheet for title
    public AnimatorOverrideController CharacterAnimator;
    //Idle sprite (can be removed after AnimatorOverrideController is added)
    public Sprite sprite;
    //Stats
    public int strength;
    public int magic;
    public int speed;
    public int jump;
    public int constitution;
    public int armour;
    public bool m_AirControl;
    public int level = 2;

    //public float xp = 0;
    // Create XP function in here not base stats

    //Stats class list and Inputs
    private List<int> baseStats = new List<int>();
    private List<BaseStat> stats = new List<BaseStat>();
    private List<string> statName = new List<string>();
    private List<string> statDesc = new List<string>();

    //Constants for move controller
    private Vector3 m_Velocity = Vector3.zero;

    public void Awake()
    {
        //Strength
        baseStats.Add(strength);
        statName.Add("Power");
        statDesc.Add("Melee Damage");
        //Magic
        baseStats.Add(magic);
        statName.Add("Magic");
        statDesc.Add("Magic Damage");
        //Speed
        baseStats.Add(speed);
        statName.Add("Speed");
        statDesc.Add("How fast you can move and order in battle");
        //Jump
        baseStats.Add(jump);
        statName.Add("Jump");
        statDesc.Add("Height");
        //Constitution
        baseStats.Add(constitution);
        statName.Add("Constitution");
        statDesc.Add("Health");
        //Armour
        baseStats.Add(armour);
        statName.Add("Armour");
        statDesc.Add("Block Chance");

        //Level of character
        baseStats.Add(level);
        statName.Add("Level");
        statDesc.Add("Current Level");


        //Builds Stat list with update able stats
        for (int i = 0; i < statName.Count; i++)
        {
            stats.Add(new BaseStat(baseStats[i], statName[i], statDesc[i]));
        }
    }

    public float GetAttack()
    {
        //Conversion to attack damage   
        return 10* strength;

    }
    public float GetMagic()
    {
        //Conversion to attack damage   
        return 10* magic;

    }
    public float GetSpeed()
    {
        //Conversion to game movement   
        return (speed + 4) * 50f;

    }
    public float GetJump()
    {
        //Conversion to game movement   
        return (jump*300);

    }
    public float GetHealth()
    {
        //Conversion to Health 
        return constitution;
    }

    //Here is the move controller, I want to make this a component

    public Vector3 Move(
        float move,                 // Direction
        Vector2 prevVelocity,       // Previous Velocity
        float m_MovementSmoothing   // Smoothing Coeffiecent 
        )
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move*GetSpeed(), prevVelocity.y);
        // And then smoothing it out
        return Vector3.SmoothDamp(prevVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);    
    }

    public Vector2 Jump()
    {
        // Add a vertical force to the player based on stats
        return new Vector2(0f, GetJump());
    }
}
