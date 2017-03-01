using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMenuDebugger : MonoBehaviour {
    /// <summary>
    /// The obstacle menu gameobject that this component is intended to do debugging for
    /// </summary>
    public GameObject ObstacleMenu;

    bool menuActive;

	// Use this for initialization
	void Start () {
        // initially want this menu hidden
        ObstacleMenu.SetActive(false);
        menuActive = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// toggles whether the obstaclemenu gameobject is active 
    /// </summary>
    public void EnableMenu()
    {
        Debug.Log("ObstacleMenuManager:ObstacleMenuDebugger: EnableMenu()");
        ObstacleMenu.SetActive(!menuActive);
        menuActive = !menuActive;
    }
}
