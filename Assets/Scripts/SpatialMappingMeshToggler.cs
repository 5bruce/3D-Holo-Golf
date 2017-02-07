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

    // Use this for initialization
    void Start () {
        spatialMappingManager = SpatialMappingManager.Instance;
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
        if (spatialMappingManager.DrawVisualMeshes) spatialMappingManager.DrawVisualMeshes = false;
        else spatialMappingManager.DrawVisualMeshes = true;
    }
}
