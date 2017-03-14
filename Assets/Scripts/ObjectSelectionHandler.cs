using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class ObjectSelectionHandler : Singleton<ObjectSelectionHandler> {
    public GameObject[] obstaclesPrefab = new GameObject[5];
    public GameObject[] buttons = new GameObject[5];
    public int numPlayers = 1;
    public GameObject[] currentObjects;
    public int objectsCreated = 0;
    public bool isPlayAndPassGame = false;

    void Start()
    { 
        currentObjects = new GameObject[numPlayers+1];
    }

    void Update()
    {
       //TODO: implement selection round cleanup, see diagram
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
            currentObjects[0] = placeHolderLastObject;
        }
    }

    /// <summary>
    /// prepares the Game Object menu by linking each button to a random obstacle 
    /// from the set of all obstacles
    /// </summary>
    public void prepareGameObjectMenu()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            setButtons(obstaclesPrefab[randomNumberSelector(0,obstaclesPrefab.Length-1)], i);
        }
    } 

    //returns a random number from within a given range
    int randomNumberSelector(int min, int max)
    {
        //TODO: don't allow repition of obstacles, remove choosen from pool
        System.Random rnd = new System.Random();
        return rnd.Next(min, max);
    }
    
    //sets the selection object of the button at index buttonNumber
    //this is the object that will pop up when the user clicks the button
    void setButtons(GameObject obj, int buttonNumber)
    {
        buttons[buttonNumber].GetComponent<clickHandler>().selectionObject = obj;
        //TODO: needs to also set the active and the inactive material of the button so user knows what they're selecting
    }



    public void ObjectSelectionHandler_StartedDragging()
    {
        gameObject.SetActive(false);
    }

    public void ObjectSelectionHandler_StoppedDragging()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": StoppedDragging event handler called");
        gameObject.SetActive(true);
    }
}
