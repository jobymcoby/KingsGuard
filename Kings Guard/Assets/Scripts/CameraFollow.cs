using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void LateUpdate()
    {
        //Current camera's position
        Vector3 temp = transform.position;
        //Camera x pos as temp
        temp.x = playerTransform.position.x;
        //Temp back to camera position
        transform.position = temp;
    }
}
