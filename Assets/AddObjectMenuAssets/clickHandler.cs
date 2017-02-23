using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickHandler : MonoBehaviour {

    // Use this for initialization
    public GameObject myPrefab1;
    public GameObject panel;
    public int numPlayers = 1;
    private int objectsCreated = 0;

    public void button_click(GameObject generate)
    {
        GameObject createdObject;
        createdObject = (GameObject)Instantiate(generate);
        objectsCreated += 1;
        //set draggable
        if (objectsCreated == numPlayers)
        {
            //move on to next round
            panel.SetActive(false);
            //randomize menu with 5 obstacles to choose from
            objectsCreated = 0;
        }
    }
}
