using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Controls the launching and reseting of the attached-to projectile gameobject.
/// Also controls when the recent-bounce flag is placed.
/// </summary>
public class ProjectileShooter : MonoBehaviour {
    /// <summary>
    /// Reference this projectile launches towards when relaeased from handdragging
    /// </summary>
    public GameObject reference_point;

    /// <summary>
    /// Icon indicating where Projectile should be shot from next
    /// </summary>
    public GameObject flag;
    Vector3 flag_position;
    Quaternion flag_rotation;
    
    /// <summary>
    /// Trigger object that counts as this projectile's goal trigger
    /// </summary>
    public GameObject goal;

    /// <summary>
    /// Minimum speed the projectile must be moving to stop from freezing in its place after launched
    /// </summary>
    public float noMovmentThresh = 0.05f;

    public float forwardOffset = 0.5f;
    public const float throwRatio = 10.0f;
    public bool resting = true;

    //bool canPlaceFlag;
    int strokes;

    // Use this for initialization
    void Start () {
        strokes = 0;

        //canPlaceFlag = true;
        // initially hide flag
        // make flag visible
        MeshRenderer[] flag_meshes = flag.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in flag_meshes)
        {
            mesh.enabled = false;
        }

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
        else
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            float speed = rb.velocity.magnitude;
            if (speed < noMovmentThresh)
            {
                // freeze the projectile of this component
                rb.velocity = new Vector3(0, 0, 0);
                //Or
                //rb.constraints = RigidbodyConstraints.FreezeAll;

                PlaceFlag();
            }
        }
    }

    void OnReset()
    {
        Debug.Log("ProjectileShooter: OnReset()");
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        resting = true;
        //canPlaceFlag = true;
    }

    /// <summary>
    /// Places the flag object of this component along the surface normal of the 
    /// object colliding with attached-to projectile when called.
    /// </summary>
    void PlaceFlag()
    {
        Debug.Log("ProjectileShooter: PlaceFlag()");
        // move flag to new position and orientation
        flag.transform.position = flag_position;
        flag.transform.rotation = flag_rotation;

        // after placing flag, don't want to be able to place again in same turn/stroke
        //canPlaceFlag = false;
    }


    void OnCollisionEnter(Collision collision)
    {
        // TODO: added sound effects

        // make flag visible
        MeshRenderer[] flag_meshes = flag.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in flag_meshes)
        {
            mesh.enabled = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // position of the porjectile
        flag_position = gameObject.transform.position;  // TODO: add an up_offset to stop flag poking under spatial mesh ball is rolling on
        // normal of the suface hit by the projectile, see http://answers.unity3d.com/answers/59309/view.html
        flag_rotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);  
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("ProjectileShooter: OnTriggerEntered()");
        if(other.transform.gameObject.name == goal.name)
        {
            Debug.Log("ProjectileShooter: goal entered");
            gameObject.SetActive(false);  // can't be activated once inactive (just for initial testing)
            SendMessageUpwards("GoalEntered", strokes);
        }
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

        strokes++;
        SendMessageUpwards("ProjectileLaunched", strokes);
    }
}
