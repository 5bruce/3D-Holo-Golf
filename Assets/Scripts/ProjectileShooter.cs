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

    public float forwardOffset = 0.5f;
    public const float throwRatio = 10.0f;
    public bool resting;

    // Use this for initialization
    void Start () {
        resting = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        gameObject.GetComponent<HandDraggable>().StartedDragging += ProjectileShooter_StartedDragging;
        gameObject.GetComponent<HandDraggable>().StoppedDragging += ProjectileShooter_StoppedDragging;
	}

    // Update is called once per frame
    void Update()
    {
        if (resting)
        {
            transform.position = Camera.main.transform.position + Vector3.Normalize(Camera.main.transform.forward)*forwardOffset;
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    void OnReset()
    {
        Debug.Log("ProjectileShooter: OnReset()");
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        resting = true;
    }

    /// <summary>
    /// Prepares projectile to be dragged from resting position
    /// </summary>
    private void ProjectileShooter_StartedDragging()
    {
        resting = false;
    }

    /// <summary>
    /// Launch projectile towards its assigned reference_point gameobject after handdragging
    /// </summary>
    private void ProjectileShooter_StoppedDragging()
    {
        Debug.Log("ProjectileShooter: StoppedDragging event handler called");
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        resting = false;

        Vector3 direction = reference_point.transform.position - gameObject.transform.position;
        float magnitude = direction.magnitude * throwRatio;
        rb.velocity = direction * magnitude;
    }
}
