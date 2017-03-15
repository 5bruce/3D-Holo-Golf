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
           // currentObjects[0] = placeHolderLastObject;
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
            //I think random only selects a number less than the max, so I'm passing in 5 instead of 4
            setButtons(obstaclesPrefab[randomNumberSelector(0,obstaclesPrefab.Length)], i);
        }
    } 

    //returns a random number from within a given range
    int randomNumberSelector(int min, int max)
    {
        Debug.Log("min: " + min + " max: " + max);
            //TODO: don't allow repition of obstacles, remove choosen from pool
            System.Random rnd = new System.Random();
            int num = rnd.Next(min, max);
        Debug.Log("number random: "+num);
        return num;
        
    }
    

    void setButtons(GameObject obj, int buttonNumber)
    {
        buttons[buttonNumber].GetComponent<clickHandler>().selectionObject = obj;
        //TODO: needs to also set the active and the inactive material of the button so user knows what they're selecting
    }




}
