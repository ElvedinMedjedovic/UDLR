using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZ : MonoBehaviour
{
    [SerializeField] float speed = 1;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0,0,speed);
    }
}
