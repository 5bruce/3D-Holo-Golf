using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the number of players that have made it to the goal
/// and controls when obstacleSelectionMenu should popup
/// </summary>
public class GoalManager : Singleton<GoalManager> {
    [Tooltip("Goal area/trigger for the game")]
    public GameObject goal;

    [Tooltip("objectSelectionMenu to be activated once all players in goal")]
    public GameObject objectSelectionMenu;

    /// <summary>
    /// The current number of players in the goal
    /// </summary>
    public int playersInGoal;

    int numberOfPlayers;

	// Use this for initialization
	void Start () {
        // FIXME: This may be dangerous since SettingsManagerLoader is also a singleton that gets init. at Start()
        numberOfPlayers = SettingsManagerLoader.Instance.numberOfPlayers;
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": numberOfPlayers = " + numberOfPlayers);

        playersInGoal = 0;
	}
	
	// LateUpdate is called once per frame after all Update()s
	void LateUpdate () {
        if (playersInGoal == numberOfPlayers)
        {
            Debug.Log(gameObject.name + ": " + this.GetType().Name + 
                ": activating " + objectSelectionMenu.name + " and setting firstplayer active");
            // toggle menu on
            objectSelectionMenu.SetActive(true);
            playersInGoal = 0;

            // activate first player to access obstacle selection menu (only AFTER it has been activaed/enabled)
            PlayAndPassManager.Instance.setFirstPlayerActive();
        }
    }
}
