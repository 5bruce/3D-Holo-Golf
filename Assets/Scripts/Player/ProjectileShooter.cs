using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

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

    bool isActive;
    bool isDragging;
    bool isColliding;
    bool canScore;
    bool canPlaceFlag;
    int strokes;

    /// <summary>
    /// Used for determining if the projectile is in freefall (has fallen out of spatial mesh)
    /// </summary>
    float launchHeight, currentHeight;
    const float maxDrop = 20;

    // Use this for initialization
    void Start () {
        // exit with error if this projectile does not have gameobject relationships set
        if (!cursor || cursor.GetComponent<AnimatedCursor>() == null ||
            !reference_point ||
            !flag ||
            !goal)
        {
            Debug.LogError(this.name + ": Start(): this script expects 'cursor' GameObject to have 'AnimatedCursor' component from the HoloToolkit");
            Debug.LogError(this.name + ": Start(): reference_point, flag, or goal GameObject may not have been set in this component");
            // note, this does not quit in the Unity editor, see http://answers.unity3d.com/answers/514429/view.html
            Application.Quit();
        }

        if(!GoalManager.Instance)
        {
            Debug.LogError(gameObject.name + ": " + this.GetType().Name + 
                ": Warning: Could not access any GoalManager component at Start()");
        }

        isActive = true;
        forwardOffset = (forwardOffset < 0.5f) ? 0.5f : forwardOffset;
        throwRatio = (throwRatio <= 0) ? 10f : throwRatio;
        isDragging = false;
        canScore = false;
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
        if (resting && isActive)
        {
            // keep ball in user's gaze
            transform.position = Camera.main.transform.position + Vector3.Normalize(Camera.main.transform.forward) * forwardOffset;
            transform.rotation = Camera.main.transform.rotation;
        }
        else
        {
            if (isDragging && isActive)
            {
                UpdateTrajectory(gameObject.transform.position, this.LaunchVelocity(), Physics.gravity);
            }
            else if (canPlaceFlag)  // has been launched
            {
                // still want to be able to roll around and place flag while this player not active

                // check that projectile has not fallen out of spatial mesh
                currentHeight = gameObject.transform.position.y;
                if (launchHeight - currentHeight > maxDrop)
                {
                    // need to freeze ball before reset, else cursor and directional indicators glitch
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                    this.OnReset();
                }
                else
                {
                    // track speed to check when to place projectile's flag
                    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                    float rollingSpeed = rb.velocity.magnitude;
                    // once ball slows enough, place flag
                    if (rollingSpeed < noMovmentThresh)
                    {
                        // freeze the projectile of this component and place flag
                        rb.velocity = new Vector3(0,0,0);
                        //Or
                        //rb.constraints = RigidbodyConstraints.FreezeAll;

                        PlaceFlag();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Used for activating this component in play-and-pass games
    /// </summary>
    void Activate()
    {
        Debug.Log(gameObject.transform.parent.gameObject.name + ": " + this.GetType().Name + ": Activate()");

        isActive = true;

        /*********** 
        * FIXME: this is just part of work around for always-handdragging bug
        * Only really applies to playandpass games
        * It might be good design that players can pick up balls rolling around (since will cause launch)
        * but does not answer underlying problem causing the bug in PlayAndPass.scene
        ************/
        if (resting)
        {
            gameObject.GetComponent<HandDraggable>().enabled = true;
        }
        //gameObject.GetComponent<HandDraggable>().enabled = true;
        /*********** end wrokaround ***********/

        gameObject.GetComponent<DirectionIndicator>().enabled = true;
        gameObject.GetComponent<DirectionIndicator>().DirectionIndicatorObject.GetComponent<MeshRenderer>().enabled = true;


        // for PlayAndPass games, flash light to indicate this projectile has priority
        // only if ball is currently rolling around
        if (isColliding) {
            StartCoroutine(gameObject.GetComponent<Flash>().flashNow());
        }
    }

    /// <summary>
    /// Used for deactivating this component in play-and-pass games
    /// </summary>
    void Deactivate ()
    {
        Debug.Log(gameObject.transform.parent.gameObject.name + ": " + this.GetType().Name + ": Deactivate()");
        isActive = false;
        gameObject.GetComponent<HandDraggable>().enabled = false;
        gameObject.GetComponent<DirectionIndicator>().enabled = false;
        gameObject.GetComponent<DirectionIndicator>().DirectionIndicatorObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnReset()
    {
        if (isActive) {
            Debug.Log(this.name + ": OnReset()");

            // need to freeze ball before reset, else cursor and directional indicators glitch
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            // set projectile back to resting state
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            resting = true;

            // projectile cannot be used to score in resting state
            canScore = false;
            canPlaceFlag = false;
            
            // remove projectile trail
            gameObject.GetComponent<TrailRenderer>().enabled = false;

            // reset cursor back to its min. distance in front of camera
            /* 
             * This is done to avoid bug where cursor gets stuck behind ball after reset. 
             * The actual reset position should not matter, since cursor will continue its usual behavior in next frame.
             */
            cursor.transform.position = Vector3.Normalize(Camera.main.transform.forward) + new Vector3(0f, 0f, cursor.GetComponent<AnimatedCursor>().MinCursorDistance);

            /*********** 
             * FIXME: this is just part of work around for always-handdragging bug
             * Only really applies to playandpass games
            ************/
            gameObject.GetComponent<HandDraggable>().enabled = true;
            /*********** end wrokaround ***********/
        }
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
        if (isActive) {
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
        isColliding = true;

        // TODO: add sound effects

        // make flag visible
        if (canPlaceFlag)
        {
            MeshRenderer[] flag_meshes = flag.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in flag_meshes)
            {
                mesh.enabled = true;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // position of the porjectile
        flagPosition = gameObject.transform.position;  // TODO: add an up_offset to stop flag poking under spatial mesh ball is rolling on
        // normal of the suface hit by the projectile, see http://answers.unity3d.com/answers/59309/view.html
        flagRotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);  
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        isColliding = false;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": OnTriggerEntered()");
        // check that we have entered the designated goal
        if(other.transform.gameObject.name == goal.name && canScore)
        {
            Debug.Log(gameObject.name + ": " + this.GetType().Name + ": goal entered");
            // clean up projectile stuff for this round
            this.OnReset();
            
            // send message to player UI 
            SendMessageUpwards("GoalEntered", strokes);
            strokes = 0;

            // alert GoalManager
            GoalManager.Instance.playersInGoal++;

            this.Deactivate();
            gameObject.SetActive(false);
            //gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Prepares projectile to be dragged from resting position
    /// </summary>
    private void ProjectileShooter_StartedDragging()
    {
        if (isActive)
        {
            Debug.Log(transform.parent.gameObject.name + ": " + this.GetType().Name + 
                ": ProjectileShooter_StartedDragging()");
            resting = false;
            isDragging = true;
            gameObject.GetComponent<LineRenderer>().enabled = true;
        }
    }

    /// <summary>
    /// Launch projectile towards its assigned reference_point gameobject after handdragging
    /// </summary>
    private void ProjectileShooter_StoppedDragging()
    {
        if (isActive) {
            Debug.Log(transform.parent.gameObject.name + ": " + this.GetType().Name + 
                ": ProjectileShooter_StoppedDragging()");
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            resting = false;

            launchHeight = gameObject.transform.position.y;

            rb.velocity = this.LaunchVelocity();

            strokes++;
            SendMessageUpwards("ProjectileLaunched", strokes);

            // remove trajectory prediction line
            gameObject.GetComponent<LineRenderer>().enabled = false;

            // start drawing projectile line
            gameObject.GetComponent<TrailRenderer>().enabled = true;

            isDragging = false;
            canScore = true;
            canPlaceFlag = true;

            /*********** 
             * FIXME: this is just part of work around for always-handdragging bug
             * Only really applies to playandpass games
            ************/
            gameObject.GetComponent<HandDraggable>().enabled = false;
            /*********** end wrokaround ***********/
        }
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
