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

    /// <summary>
    /// Obstacle gameobjects that have been created during this selection round
    /// </summary>
    public GameObject[] currentObjects;
    /// <summary>
    /// Number of obstacles created in this selection round
    /// </summary>
    public int objectsCreated = 0;

    [Tooltip("is this used in a multiplayer game on a single device?")]
    public bool isPlayAndPassGame = true;

    void Start()
    { 
        currentObjects = new GameObject[numPlayers + 1];
    }

    void Update()
    {
       if(objectsCreated == numPlayers)
        {
            //make the menu invisible
            gameObject.SetActive(false);

            for(int i = 0; i < currentObjects.Length - 1; i++)
            {
                //remove hand draggable capability
                Destroy(currentObjects[i].GetComponent<HandDraggable>());
            }

            //randomizes the game object menu for next round
            if (!isPlayAndPassGame)
            {
                prepareGameObjectMenu();
            }

            GameObject placeHolderLastObject = currentObjects[currentObjects.Length - 1];
            //resets objects created back to zero
            objectsCreated = 0;

            currentObjects = new GameObject[numPlayers+1];
           // currentObjects[0] = placeHolderLastObject;
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
