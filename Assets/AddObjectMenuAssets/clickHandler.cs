using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class clickHandler : MonoBehaviour,
                                 IFocusable,
                                 IInputClickHandler
{
    public Material active_material;
    public Material inactive_material;
    public GameObject selectionObject;
    public GameObject panel;
    public int numPlayers = 1;
    private int objectsCreated = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void IFocusable.OnFocusEnter()
    {
        Debug.Log("ExitButtonHandler: OnFocusEnter()");
        gameObject.GetComponent<MeshRenderer>().material = active_material;
    }

    void IFocusable.OnFocusExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = inactive_material;
    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        //on click function
        Debug.Log("ObjectButtons: OnInputClicked()");

        GameObject createdObject;
        createdObject = (GameObject)Instantiate(selectionObject);
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
