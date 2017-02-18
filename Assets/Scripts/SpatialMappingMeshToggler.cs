using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toggles spatial mesh drawing.
/// Intended to be activated/deactiveated by KeywordManager
/// </summary>
public class SpatialMappingMeshToggler : MonoBehaviour {

    /// <summary>
    /// Controls spatial mapping.  In this script we access spatialMappingManager
    /// to control rendering and to access the physics layer mask.
    /// </summary>
    protected SpatialMappingManager spatialMappingManager;

    /// <summary>
    /// If exists in scene, controls creation of planes from suface mesh
    /// </summary>
    protected SurfaceMeshesToPlanes surfaceMeshesToPlanes;

    // Use this for initialization
    void Start () {
        spatialMappingManager = SpatialMappingManager.Instance;
        surfaceMeshesToPlanes = SurfaceMeshesToPlanes.Instance;
        if (spatialMappingManager == null)
        {
            Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleMesh()
    {
        Debug.Log("SpatialMappingToggler: ToggleMesh() called");
        // toggle rendering of spatial mesh
        if (spatialMappingManager.DrawVisualMeshes) spatialMappingManager.DrawVisualMeshes = false;
        else spatialMappingManager.DrawVisualMeshes = true;

        // toggle rendering of surface planes
        if(surfaceMeshesToPlanes != null)
        {
            List<GameObject> activePlanes = surfaceMeshesToPlanes.ActivePlanes;
            if (activePlanes[0].GetComponent<MeshRenderer>().enabled)
            {
                foreach (GameObject plane in activePlanes)
                    plane.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                foreach (GameObject plane in activePlanes)
                    plane.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
