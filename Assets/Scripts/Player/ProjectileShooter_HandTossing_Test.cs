using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Controls the launching and reseting of the attached-to projectile gameobject.
/// Also controls when the recent-bounce flag is placed.
/// </summary>
public class ProjectileShooter_HandTossing_Test : MonoBehaviour
{
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

    public const float forwardOffset = 0.5f;
    public const float throwRatio = 10.0f;
    public bool resting = true;

    bool can_place_flag;
    // track strokes for score
    int strokes;
    // track position and speed data for launching on release
    Vector3 previous_position;
    Vector3 current_position;
    Vector3 speed, launch_velocity;

    // Use this for initialization
    void Start()
    {
        can_place_flag = false;
        strokes = 0;
        current_position = previous_position = gameObject.transform.position;
        speed = launch_velocity = Vector3.zero;

        // initially hide flag
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
            transform.position = Camera.main.transform.position + Vector3.Normalize(Camera.main.transform.forward) * forwardOffset;
            transform.rotation = Camera.main.transform.rotation;
        }
        else
        {
            if (!resting)
            {
                // track prev. and curr. positions and speed
                previous_position = current_position;
                current_position = gameObject.transform.position;
                float delta = Time.deltaTime;
                // need to calc. projectile motion this way b/c handdragging does not register as ridgidbody velocity
                speed = (delta != 0)
                    ? (current_position - previous_position) / delta
                    : Vector3.zero;
                launch_velocity = (speed != Vector3.zero) ? speed : launch_velocity;
                Debug.Log("ProjectileShooter: LateUpdate: speed=" + speed);
            }

            if (can_place_flag)
            {
                // track speed to check when to place projectile's flag
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                float rolling_speed = rb.velocity.magnitude;
                if (rolling_speed < noMovmentThresh)
                {
                    // freeze the projectile of this component and place flag
                    rb.velocity = new Vector3(0, 0, 0);
                    //Or
                    //rb.constraints = RigidbodyConstraints.FreezeAll;

                    PlaceFlag();
                }
            }
        }
    }

    void OnReset()
    {
        Debug.Log("ProjectileShooter: OnReset()");
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        resting = true;
        gameObject.GetComponent<TrailRenderer>().enabled = false;
    }

    /// <summary>
    /// Places the flag object of this component along the surface normal of the 
    /// object colliding with attached-to projectile when called.
    /// </summary>
    void PlaceFlag()
    {
        // move flag to new position and orientation
        flag.transform.position = flag_position;
        flag.transform.rotation = flag_rotation;

        // after placing flag, don't want to be able to place again in same turn/stroke
        //can_place_flag = false;
    }


    void OnCollisionEnter(Collision collision)
    {
        // TODO: add sound effects

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
        if (other.transform.gameObject.name == goal.name)
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

        Vector3 direction = /*reference_point.transform.position - gameObject.transform.position*/Vector3.Normalize(current_position - previous_position);
        //float magnitude = direction.magnitude * throwRatio;
        Debug.Log("ProjectileShooter: direction=" + direction + " speed=" + speed + " launch_velocity=" + launch_velocity);
        rb.velocity = launch_velocity;

        strokes++;
        SendMessageUpwards("ProjectileLaunched", strokes);

        gameObject.GetComponent<TrailRenderer>().enabled = true;

        can_place_flag = true;
    }
}
