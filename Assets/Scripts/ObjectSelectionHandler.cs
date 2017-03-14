using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectSelectionHandler : MonoBehaviour {
    public GameObject canvas;
    public GameObject[] obstacles = new GameObject[2];
    public int numPlayers = 1;
    public int objectsCreated = 0;

    void instantiateGameObjectMenu()
    {
        for(int i = 1; i <= obstacles.Length; i++)
        {
            instantiateObj(obstacles[randomNumberSelector(1,5)]);
        }
    } 


    int randomNumberSelector(int min, int max)
    {
        System.Random rnd = new System.Random();
        return rnd.Next(min, max);
    }
    
    void instantiateObj(GameObject obj)
    {
        //create button
    }
}
