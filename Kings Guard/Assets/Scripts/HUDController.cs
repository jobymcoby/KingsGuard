using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HUDController : MonoBehaviour
{

    public void Start()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseButtonPressed();
        }
    }


    public void PauseButtonPressed()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            Debug.Log("Paused");
        }
        else
        {
            Time.timeScale = 1;
            Debug.Log("Resume");
        }
    }
}
