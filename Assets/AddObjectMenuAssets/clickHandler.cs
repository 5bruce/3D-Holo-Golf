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
    public GameObject canvasObj;
    public int numPlayers = 0;
    public int objectsCreated = 0;
    private ObjectSelectionHandler osh;

    // Use this for initialization
    void Start()
    {
        osh = canvasObj.GetComponent<ObjectSelectionHandler>();
        numPlayers = osh.numPlayers;
        objectsCreated = osh.objectsCreated;
    }

    // Update is called once per frame
    void Update()
    {
        //in case the other buttons have been clicked
    }

    void IFocusable.OnFocusEnter()
    {
        Debug.Log("ExitButtonHandler: OnFocusEnter()");
        //System.Console.Write("ExitButtonHandler: OnFocusEnter()");
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
        //System.Console.Write("ObjectButtons: OnInputClicked()");

        //Make sure variable is up to date
        objectsCreated = osh.objectsCreated;

        //The reason I do it before I actually create the objects, is so that it creates a "lock" on 
        //creating on object, however it doesn't actually make a lock so errors could happen with multiple
        //hololenses 
        //see if I can make an actual lock in c#
        osh.objectsCreated += 1;
        objectsCreated += 1;

        //canvasObj.SetActive(false);

        GameObject createdObject;
        createdObject = (GameObject)Instantiate(selectionObject);
        createdObject.transform.position = canvasObj.transform.position - canvasObj.transform.forward;
        createdObject.SetActive(true);
        
        //set draggable
        if (objectsCreated == numPlayers)
        {
            //move on to next round
            canvasObj.SetActive(false);
            //randomize menu with 5 obstacles to choose from
          //  osh.objectsCreated = 0;
            objectsCreated = 0;
        }
    }
}
