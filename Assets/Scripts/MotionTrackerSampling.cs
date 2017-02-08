using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Store a few of the previous positions of the object and check 
///  if the distance between any of those positions is less very small.
/// </summary>
public class MotionTrackerSampling : MonoBehaviour {

    public GameObject objectToTrack;
    public float noMovementThreshold = 0.0001f;
    public const int noMovementFrames = 3;

    //Set this to the transform you want to check
    private Transform objectTransfom;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    private bool isMoving;

    //Let other scripts see if the object is moving
    public bool IsMoving
    {
        get { return isMoving; }
    }

    void Start()
    {
        objectTransfom = objectToTrack.transform;
    }

    void Awake()
    {
        //For good measure, set the previous locations
        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
    }

    void Update()
    {
        //Store the newest vector at the end of the list of vectors
        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            previousLocations[i] = previousLocations[i + 1];
        }
        previousLocations[previousLocations.Length - 1] = objectTransfom.position;

        //Check the distances between the points in your previous locations
        //If for the past several updates, there are no movements smaller than the threshold,
        //you can most likely assume that the object is not moving
        /* This loop may cause errors, see http://answers.unity3d.com/comments/1257108/view.html */
        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
            {
                //The minimum movement has been detected between frames
                isMoving = true;
                break;
            }
            else
            {
                isMoving = false;
            }
        }
    }

}
