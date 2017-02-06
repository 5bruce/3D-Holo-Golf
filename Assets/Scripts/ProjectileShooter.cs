using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Controls whether the attached-to projectile is launching or resting
/// </summary>
public class ProjectileShooter : MonoBehaviour {
    //public GameObject projectile_prefab;
    public GameObject reference_point;

    const float forwardOffset = 0.5f;
    public const float throwRatio = 10.0f;
    public bool resting;

    // Use this for initialization
    void Start () {
        resting = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        gameObject.GetComponent<HandDraggable>().StoppedDragging += ProjectileShooter_StoppedDragging;
	}

    // Update is called once per frame
    void Update()
    {
        if (resting)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * forwardOffset;
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    void OnReset()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        resting = true;
    }

    /// <summary>
    /// Launch projectile towards its assigned reference_point gameobject after handdragging
    /// </summary>
    private void ProjectileShooter_StoppedDragging()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        resting = false;

        Vector3 direction = reference_point.transform.position - gameObject.transform.position;
        float magnitude = direction.magnitude * throwRatio;
        rb.velocity = direction * magnitude;
    }
}
