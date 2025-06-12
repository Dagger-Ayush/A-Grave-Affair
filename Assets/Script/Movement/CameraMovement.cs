using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class CameraMovement : MonoBehaviour
{
    public Transform target;
    void Start()
    {


    }


    void Update()
    {


    }
    private void FixedUpdate()
    {
        transform.LookAt(target);
    }
}

