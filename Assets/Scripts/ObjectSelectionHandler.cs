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
    public int objectsCreated = 0;

    [Tooltip("is this used in a multiplayer game on a single device?")]
    public bool isPlayAndPassGame = true;

    /// <summary>
    /// Which round of obstacle selection we are currently on
    /// </summary>
    private int roundNumber = 1;

    void Start()
    { 
        currentObjects = new Queue<GameObject>(numPlayers);

        // initial obstacle selection randomization
        prepareGameObjectMenu();
    }

    // Called after all Update() functions called
    void LateUpdate()
    {
        // current obstacle selection round over
       if(objectsCreated == numPlayers)
        {
            Debug.Log(this.GetType().Name + ": current selection round ending");

            // remove hand draggable capability from all objects created this round
            // except for the one created by the last player
            for (int i = 0; i < currentObjects.Count - 1; i++)
            {
                GameObject popped = currentObjects.Dequeue();
                Debug.Log(this.GetType().Name + ": un-HandDragging gameobject " + popped.name);
                Destroy(popped.GetComponent<HandDraggable>());
            }

            // randomize menu for next round
            prepareGameObjectMenu();

            // reset number of objects created back to zero for next round
            objectsCreated = 0;

            // disable the menu once all players have selected an object
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Prepares the Game Object menu by linking each button to a random obstacle 
    /// from the set of all obstacles. Associations are removed after each obstacle
    /// selection round.
    /// </summary>
    public void prepareGameObjectMenu()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            //I think random only selects a number less than the max, so I'm passing in 5 instead of 4
            setButtons(obstaclesPrefab[randomNumberSelector(0,obstaclesPrefab.Length)], i);
        }
    }

    /// <summary>
    /// Returns a random number from within the given range
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    int randomNumberSelector(int min, int max)
    {
        //Debug.Log(gameObject.name + ": " + this.GetType().Name + " min: " + min + " max: " + max);
        //TODO: don't allow repition of obstacles, remove choosen from pool?
        System.Random rnd = new System.Random();
        int num = rnd.Next(min, max);
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
