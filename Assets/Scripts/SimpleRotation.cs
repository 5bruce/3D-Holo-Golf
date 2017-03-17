using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour {

    public float speed = 20f;

    void LateUpdate()
    {
        transform.Rotate(Vector3.right, speed * Time.deltaTime);
    }
}

