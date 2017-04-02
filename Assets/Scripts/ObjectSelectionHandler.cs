using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class ObjectSelectionHandler : Singleton<ObjectSelectionHandler> {
    [Tooltip("Potential obstacles that may be presented to players")]
    public GameObject[] obstaclesPrefab = new GameObject[5];

    [Tooltip("The obstacle selection buttons associated with this menu")]
    public GameObject[] buttons = new GameObject[5];

    [Tooltip("The number of players in this game")]
    public int numPlayers = 1;

    /*
    In order to ensure that the last player is able to move their selected object
    around during the current selection round, we allow the last player to HandDrag
    their obstacle until the next round.  
    */
    /// <summary>
    /// Obstacle gameobjects that have been created during this selection round
    /// </summary>
    public Queue<GameObject> currentObjects;

    /// <summary>
    /// Number of obstacles created in this selection round
    /// </summary>
    public int objectsCreated;

    [Tooltip("is this used in a multiplayer game on a single device?")]
    public bool isPlayAndPassGame;

    void Start()
    {
        objectsCreated = 0;
        isPlayAndPassGame = SettingsManagerLoader.Instance.isPlayAndPassGame;
        numPlayers = SettingsManagerLoader.Instance.numberOfPlayers;
        currentObjects = new Queue<GameObject>(numPlayers);

        // initial obstacle selection randomization
        prepareGameObjectMenu();
    }

    // Called after all Update() functions called
    void LateUpdate()
    {
       
    }

    /// <summary>
    /// To be called by clickHandler.cs components
    /// </summary>
    void cleanupSelectionRound()
    {
        Debug.Log(this.GetType().Name + ": current selection round ending");

        // remove hand draggable capability from all objects created this round
        // except for the one created by the last player
        while (currentObjects.Count > 1)
        {
            GameObject popped = currentObjects.Dequeue();
            Destroy(popped.GetComponent(typeof(HandDraggable)));
            Debug.Log(this.GetType().Name + ": un-HandDragging gameobject " + popped.name);
        }

        // randomize menu for next round
        prepareGameObjectMenu();

        // reset number of objects created back to zero for next round
        objectsCreated = 0;

        // disable the menu once all players have selected an object
        gameObject.SetActive(false);

        if (numPlayers > 1 && isPlayAndPassGame) PlayAndPassManager.Instance.setFirstPlayerActive();
    }

    /// <summary>
    /// Prepares the Game Object menu by linking each button to a random obstacle 
    /// from the set of all obstacles.
    /// </summary>
    public void prepareGameObjectMenu()
    {
        System.Random rng = new System.Random();
        for (int i = 0; i < buttons.Length; i++)
        {
            setButtons(obstaclesPrefab[randomNumberInRange(rng, 0, obstaclesPrefab.Length)], i);
        }
    }

    /// <summary>
    /// Returns a random number from within the given range [min, max).
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    int randomNumberInRange(System.Random rng, int min, int max)
    {
        //TODO: don't allow repition of obstacles, remove choosen from pool?
        
        int num = rng.Next(min, max);
        //Debug.Log(gameObject.name + ": " + this.GetType().Name + ": number random: " + num);
        return num;
    }
    
    /// <summary>
    /// Assigns obj as the gameobject that button object at index buttonNumber in this.buttons
    /// creates when clicked. 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="buttonNumber"></param>
    void setButtons(GameObject obj, int buttonNumber)
    {
        buttons[buttonNumber].GetComponent<clickHandler>().selectionObject = obj;

        //TODO: needs to also set the active and the inactive material of the button so user knows what they're selecting
    }


}
