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
    /// The cursor belonging to the player who's projectile this component's attached-to gameobject is.
    /// </summary>
    public GameObject cursor;

    /// <summary>
    /// Reference this projectile launches towards when relaeased from handdragging
    /// </summary>
    public GameObject reference_point;

    /// <summary>
    /// Icon indicating where Projectile should be shot from next
    /// </summary>
    public GameObject flag;
    Vector3 flagPosition;
    Quaternion flagRotation;
    
    /// <summary>
    /// Trigger object that counts as this projectile's goal trigger
    /// </summary>
    public GameObject goal;

    /// <summary>
    /// Minimum speed the projectile must be moving to stop from freezing in its place after launched
    /// </summary>
    public float noMovmentThresh = 0.05f;

    /// <summary>
    /// distance from camera for projectile to stay at when resting (must be above 0.5f)
    /// </summary>
    public float forwardOffset = 0.5f;

    /// <summary>
    /// force multiplier to apply to projectile and reference point distance when released (must be above 0)
    /// </summary>
    public float throwRatio = 10.0f;

    public bool resting = true;

    bool isDragging;
    bool canPlaceFlag;
    int strokes;

    // Use this for initialization
    void Start () {
        // exit with error if this projectile does not have gameobject relationships set
        if (!cursor || cursor.GetComponent<AnimatedCursor>() == null ||
            !reference_point ||
            !flag ||
            !goal)
        {
            Debug.LogError("ProjectileShooter.cs: Start(): this script expects 'cursor' GameObject to have 'AnimatedCursor' component from the HoloToolkit");
            Debug.LogError("ProjectileShooter.cs: Start(): reference_point, flag, or goal GameObject may not have been set in this component");
            // note, this does not quit in the Unity editor, see http://answers.unity3d.com/answers/514429/view.html
            Application.Quit();
        }

        forwardOffset = (forwardOffset < 0.5f) ? 0.5f : forwardOffset;
        throwRatio = (throwRatio <= 0) ? 10f : throwRatio;
        isDragging = false;
        canPlaceFlag = false;
        strokes = 0;

        // initially hide trajectory prediction line
        gameObject.GetComponent<LineRenderer>().enabled = false;

        // initially hide flag
        MeshRenderer[] flagMeshes = flag.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in flagMeshes)
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
            if (isDragging)
            {
                UpdateTrajectory(gameObject.transform.position, this.LaunchVelocity(), Physics.gravity);
            }
            if (canPlaceFlag)
            {
                // track speed to check when to place projectile's flag
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                float rollingSpeed = rb.velocity.magnitude;
                if (rollingSpeed < noMovmentThresh)
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

        // reset cursor back to its min. distance in front of camera
        /* 
         * This is done to avoid bug where cursor gets stuck behind ball after reset. 
         * The actual reset position should not matter, since cursor will continue its usual behavior in next frame.
         */
        //cursor.transform.position = Vector3.Normalize(Camera.main.transform.forward) + new Vector3(0f, 0f, cursor.GetComponent<AnimatedCursor>().MinCursorDistance);
    }

    /// <summary>
    /// draws a line predicting the projectile's basic trajectory.
    /// see https://forum.unity3d.com/threads/projectile-prediction-line.143636/#post-985266
    /// </summary>
    /// <param name="initialPosition"></param>
    /// <param name="initialVelocity"></param>
    /// <param name="gravity"></param>
    void UpdateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity)
    {
        // number of line vertices to draw with
        int numSteps = 20; // for example
        // delta between predicted trajectory to place vertices
        float timeDelta = 0.1f / initialVelocity.magnitude; // for example

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.numPositions = numSteps;

        Vector3 position = initialPosition;
        Vector3 velocity = initialVelocity;
        for (int v = 0; v < numSteps; v++)
        {
            position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
            velocity += gravity * timeDelta;
            lineRenderer.SetPosition(v, position);  // can place this either at start or end of this section
        }
    }

    /// <summary>
    /// Places the flag object of this component along the surface normal of the 
    /// object colliding with attached-to projectile when called.
    /// </summary>
    void PlaceFlag()
    {
        // move flag to new position and orientation
        flag.transform.position = flagPosition;
        flag.transform.rotation = flagRotation;

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
        flagPosition = gameObject.transform.position;  // TODO: add an up_offset to stop flag poking under spatial mesh ball is rolling on
        // normal of the suface hit by the projectile, see http://answers.unity3d.com/answers/59309/view.html
        flagRotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);  
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
        isDragging = true;
        gameObject.GetComponent<LineRenderer>().enabled = true;
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

        rb.velocity = this.LaunchVelocity();

        strokes++;
        SendMessageUpwards("ProjectileLaunched", strokes);

        // remove trajectory prediction line
        gameObject.GetComponent<LineRenderer>().enabled = false;

        // start drawing projectile line
        gameObject.GetComponent<TrailRenderer>().enabled = true;

        isDragging = false;
        canPlaceFlag = true;
    }
    /// <summary>
    /// calculates velocity to apply to projectile based on projectile's reference point
    /// </summary>
    /// <returns></returns>
    private Vector3 LaunchVelocity()
    {
        Vector3 direction = reference_point.transform.position - gameObject.transform.position;
        float magnitude = direction.magnitude * throwRatio;
        return direction * magnitude;
    }
}
