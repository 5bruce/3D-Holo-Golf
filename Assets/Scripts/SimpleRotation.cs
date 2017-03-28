using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour {

    public float speed = 20f;

    [Tooltip("Local axes that gameobject should rotate on")]
    public bool x= false, y = false, z = false;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (x) transform.Rotate(Vector3.right, speed * Time.deltaTime);
        if (y) transform.Rotate(Vector3.up, speed * Time.deltaTime);
        if (z) transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}

