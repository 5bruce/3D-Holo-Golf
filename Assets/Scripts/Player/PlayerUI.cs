using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Displays information to player's HUD.
/// Receives messages from ProjectileShooter.
/// </summary>
public class PlayerUI : MonoBehaviour {
    /// <summary>
    /// UI Text component to write stroke count to
    /// </summary>
    public Text strokesText;

    public Text goalText;

	// Use this for initialization
	void Start () {
        strokesText.text = "Strokes: 0";
        goalText.text = "";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ProjectileLaunched(int strokes)
    {
        Debug.Log(gameObject.name + this.GetType().Name + ": ProjectileLaunched()");
        strokesText.text = "Strokes: " + strokes.ToString();
    }

    void GoalEntered(int strokes)
    {
        Debug.Log(gameObject.name + this.GetType().Name + ": GoalEntered(): strokes = " + strokes.ToString());
        strokesText.text = "";
        if (strokes == 1) goalText.text = "Hole in one!";
        else goalText.text = strokes.ToString() + " strokes!";  // FIXME: can' concat. a string literal to front
    }
}
