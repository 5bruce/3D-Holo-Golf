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
        if (!ObstacleMenu)
            Debug.LogError(this.name + ": no obstacle menu gameobject provided");
        else
        {
            // initially want this menu hidden
            ObstacleMenu.SetActive(false);
            menuActive = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// toggles whether the obstaclemenu gameobject is active 
    /// </summary>
    public void EnableMenu()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": EnableMenu()");
        ObstacleMenu.SetActive(true);
        menuActive = true;
    }
}
