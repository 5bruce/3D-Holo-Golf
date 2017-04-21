using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;

public class ProjectileShooter_Shared : MonoBehaviour {

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
    GameObject flag;
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
    void Start()
    {
        // Special case ID 0 to mean the _local_ user.  
        int AvatarIndex = LocalPlayerManager.Instance.AvatarIndex;

        // exit with error if this projectile does not have gameobject relationships set
        if (!cursor || cursor.GetComponent<AnimatedCursor>() == null ||
            !reference_point ||
            !goal)
        {
            Debug.LogError(this.name + ": Start(): this script expects 'cursor' GameObject to have 'AnimatedCursor' component from the HoloToolkit");
            Debug.LogError(this.name + ": Start(): reference_point, flag, or goal GameObject may not have been set in this component");
            // note, this does not quit in the Unity editor, see http://answers.unity3d.com/answers/514429/view.html
            Application.Quit();
        }

        if (!GoalManager.Instance)
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

        // set flag to be same as player avatar flag
        flag = (GameObject)Instantiate(PlayerAvatarStore.Instance.PlayerAvatars[AvatarIndex]);

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


        // hook to process ShootProjectile CutomMessages
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ShootProjectile_Velocity] = this.ProjectileShooter_ProcessRemoteProjectile;

        // Set projectile color to be the same as the avatar color.
        Color flag_color = PlayerAvatarStore.Instance.PlayerAvatars[AvatarIndex].transform.FindChild("Flag").GetComponent<Renderer>().material.color;
        if (flag_color != null)
        {
            Debug.LogFormat("{0}: {1}: Start(): setting ball color to {2}",
                gameObject.name, this.GetType().Name, flag_color);
            gameObject.GetComponent<Renderer>().material.color = flag_color;
        }
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
            if (isDragging && isActive)  // projectile is currently being dragged
            {
                UpdateTrajectory(gameObject.transform.position, this.LaunchVelocity(), Physics.gravity);
            }
            else if (canPlaceFlag)  // projectile has been launched
            {
                // still want to be able to roll around and place flag while this player not active

                // check that projectile has not fallen out of spatial mesh
                currentHeight = gameObject.transform.position.y;
                if (launchHeight - currentHeight > maxDrop)
                {
                    // need to freeze ball before reset, else cursor and directional indicators glitch
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
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
                        rb.velocity = new Vector3(0, 0, 0);

                        PlaceFlag();
                    }
                }
            }

            // broadcast projectile position
            //CustomMessages.Instance.SendStageTransform(gameObject.transform.position, gameObject.transform.rotation);
        }
    }

    public void OnReset()
    {
        if (isActive)
        {
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

            // allow user to toss ball again
            gameObject.GetComponent<HandDraggable>().enabled = true;

            // reset cursor back to its min. distance in front of camera
            /* 
             * This is done to avoid bug where cursor gets stuck behind ball after reset. 
             * The actual reset position should not matter, since cursor will continue its usual behavior in next frame.
             */
            cursor.transform.position = Vector3.Normalize(Camera.main.transform.forward) + new Vector3(0f, 0f, cursor.GetComponent<AnimatedCursor>().MinCursorDistance);
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
        if (isActive)
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
        if (other.transform.gameObject.name == goal.name && canScore)
        {
            Debug.Log(gameObject.name + ": " + this.GetType().Name + ": goal entered");
            // clean up projectile stuff for this round
            this.OnReset();

            // send message to player UI 
            SendMessageUpwards("GoalEntered", strokes);
            strokes = 0;

            // alert GoalManager
            GoalManager.Instance.playersInGoal++;

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
                ": ProjectileShooter_Shared_StartedDragging()");
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
        if (isActive)
        {
            Debug.Log(transform.parent.gameObject.name + ": " + this.GetType().Name +
                ": ProjectileShooter_Shared_StoppedDragging()");
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            resting = false;

            launchHeight = gameObject.transform.position.y;
            
            rb.velocity = this.LaunchVelocity();

            /*********** 2 different ways I think can shoot projectile, need to test for better understanding
            // apply velocity based on reference point distance

            Vector3 direction = reference_point.transform.position - gameObject.transform.position;
            float magnitude = direction.magnitude * throwRatio;
            SpawnProjectile(gameObject.transform.position, direction, magnitude, 0);

            
            // anchor relative to the transform of the ImportExportAnchorManager
            Transform anchor = ImportExportAnchorManager.Instance.gameObject.transform;
            // broadcast position and direction info of this fired projectile to other players in world space relative to anchor
            CustomMessages.Instance.SendShootProjectile_Velocity(
                anchor.InverseTransformPoint(gameObject.transform.position),
                anchor.InverseTransformDirection(direction),
                magnitude);
            ***********/


            strokes++;
            SendMessageUpwards("ProjectileLaunched", strokes);

            // remove trajectory prediction line
            gameObject.GetComponent<LineRenderer>().enabled = false;

            // start drawing projectile line
            gameObject.GetComponent<TrailRenderer>().enabled = true;

            // dont let user pick thier ball up when rolling
            gameObject.GetComponent<HandDraggable>().enabled = false;

            isDragging = false;
            canScore = true;
            canPlaceFlag = true;
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


    /// <summary>
    /// Spawns a new projectile in the world if the user
    /// doesn't already have one and fires it, broadcasting this info to other players.
    /// </summary>
    void SpawnProjectile(Vector3 position, Vector3 direction, float magnitude, long UserId)
    {
        ShootProjectile(position, direction, magnitude, UserId);

        // anchor relative to the transform of the ImportExportAnchorManager
        Transform anchor = ImportExportAnchorManager.Instance.gameObject.transform;
        // broadcast position and direction info of this fired projectile to other players in world space relative to anchor
        CustomMessages.Instance.SendShootProjectile_Velocity(
            anchor.InverseTransformPoint(position),
            anchor.InverseTransformDirection(direction),
            magnitude);
    }

    /// <summary>
    /// Adds a new projectile to the world and launches it.
    /// </summary>
    /// <param name="start">Position to shoot from</param>
    /// <param name="direction">Position to shoot toward</param>
    /// <param name="radius">Size of destruction when colliding.</param>
    void ShootProjectile(Vector3 start, Vector3 direction, float magnitude, long OwningUser)
    {
        // Need to know the index in the PlayerAvatarStore to grab for this projectile's behavior.
        int AvatarIndex = 0;

        // Special case ID 0 to mean the _local_ user.  
        if (OwningUser == 0)
        {
            AvatarIndex = LocalPlayerManager.Instance.AvatarIndex;
        }
        else
        {
            RemotePlayerManager.RemoteHeadInfo headInfo = RemotePlayerManager.Instance.GetRemoteHeadInfo(OwningUser);
            AvatarIndex = headInfo.PlayerAvatarIndex;
        }

        PlayerAvatarParameters ownerAvatarParameters = PlayerAvatarStore.Instance.PlayerAvatars[AvatarIndex].GetComponent<PlayerAvatarParameters>();

        // spawn projectile
        GameObject spawnedProjectile = (GameObject)Instantiate(ownerAvatarParameters.PlayerShotObject);
        spawnedProjectile.transform.position = start;

        // fire projectile in direction
        ProjectileBehavior pc = spawnedProjectile.GetComponentInChildren<ProjectileBehavior>();
        pc.startDir = direction * magnitude;
        pc.OwningUserId = OwningUser;
    }

    /// <summary>
    /// Process user hit by someone else's projectile.
    /// </summary>
    /// <param name="msg"></param>
    void ProjectileShooter_ProcessRemoteProjectile(NetworkInMessage msg)
    {
        // Parse the message, getting userID, position vecotor, and velocity vector data
        long userID = msg.ReadInt64();

        Vector3 remoteProjectilePosition = CustomMessages.Instance.ReadVector3(msg);
        Vector3 remoteProjectileDirection = CustomMessages.Instance.ReadVector3(msg);
        float remoteProjectileVelocityMagnitude = msg.ReadFloat();

        // capture position and direction values of remote projectile and add it to our local space
        Transform anchor = ImportExportAnchorManager.Instance.gameObject.transform;
        ShootProjectile(
            anchor.TransformPoint(remoteProjectilePosition),
            anchor.TransformDirection(remoteProjectileDirection),
            remoteProjectileVelocityMagnitude, 
            userID);
    }
}
