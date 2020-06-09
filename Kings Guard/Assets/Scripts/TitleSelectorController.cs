using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleSelectorController : MonoBehaviour
{
    // This is used in the selection screen to choose a title and play the game

    // Transform of where the hero will stand
    public GameObject Hero;
    public Title Druid, Barbarian, Fencer, Cultist, Necromancer, Mage;
    public Text health_text;
    public Text name_text;
    public Button playButton;
    public static Title Player;
    private SpriteRenderer Sprite_P1;
    private Animator animator_P1;
    private int CharachterInt = 0;
    private List<Title> titlesAvailible = new List<Title>();


    private void Awake()
    {
        // Here I'm building the list of titles that can be scrolled through
        titlesAvailible.Add(Druid); //index 0 LOCKED until move animations
        titlesAvailible.Add(Fencer); //index 1
        titlesAvailible.Add(Cultist); //index 2
        titlesAvailible.Add(Necromancer); //index 3 LOCKED until move animations
        titlesAvailible.Add(Barbarian); //index 4 LOCKED until move animations
        titlesAvailible.Add(Mage); //index 5
        
        // Adding a sprite render and animator component
        Sprite_P1 = Hero.AddComponent<SpriteRenderer>();
        animator_P1 = Hero.AddComponent<Animator>();
        //Set first Title
        DisplayChar(CharachterInt);
    }

    private void DisplayNext(ref int CharachterInt)
    {
        //Select next character 
        CharachterInt++;
        if (CharachterInt > titlesAvailible.Count - 1)
        {
            CharachterInt = 0;
        }
        DisplayChar(CharachterInt);
    }

    private void DisplayPrev(ref int CharachterInt)
    {
        //Select previous character
        CharachterInt--;
        if (CharachterInt < 0)
        {
            CharachterInt = titlesAvailible.Count - 1;
        }
        DisplayChar(CharachterInt);
    }

    private void DisplayChar(int CharachterInt)
    {
        Player = titlesAvailible[CharachterInt];
        //Display Health
        health_text.text = "Health: " + Player.GetHealth();
        //Lock Titles on index
        if (CharachterInt == 0 | CharachterInt == 4)
        {
            name_text.text = "Locked";
            playButton.interactable = false;
        }
        else
        {
            name_text.text = Player.TitleName;
            playButton.interactable = true;
        }
        //Display charachtr idle
        if (titlesAvailible[CharachterInt].CharacterAnimator is null)
        {
            animator_P1.runtimeAnimatorController = null;
            
        }
        else
        {
            animator_P1.runtimeAnimatorController = titlesAvailible[CharachterInt].CharacterAnimator;
            Sprite_P1.sprite = titlesAvailible[CharachterInt].sprite;
        }
    }

    public void NextTitle()
    {
        DisplayNext(ref CharachterInt);
    }

    public void PrevTitle()
    {
        DisplayPrev(ref CharachterInt);
    }

    private void ResetInt()
    {
        if (CharachterInt > 5)
        {
            CharachterInt = 0;
        }
        else
        {
            CharachterInt = 5;
        }
    }

    public void ChangeScene()
    {
        Debug.Log(Player.TitleName);
        SceneManager.LoadScene(1);
    }
}
