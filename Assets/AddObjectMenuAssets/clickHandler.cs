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

    [Tooltip("Object to be created when this gameobject is clicked")]
    public GameObject selectionObject;

    [Tooltip("Parent menu of this obstacle selection button")]
    public GameObject parentMenu;

    public int numPlayers = 0;
    public int objectsCreated = 0;
    private ObjectSelectionHandler objectSelectionHandler;

    // Use this for initialization
    void Start()
    {
        objectSelectionHandler = parentMenu.GetComponent<ObjectSelectionHandler>();
        numPlayers = objectSelectionHandler.numPlayers;
        objectsCreated = objectSelectionHandler.objectsCreated;
    }

    // Update is called once per frame
    void Update()
    {
        //in case the other buttons have been clicked
    }

    void IFocusable.OnFocusEnter()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": OnFocusEnter()");
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
        Debug.Log(gameObject.name + ": OnInputClicked()");
        //System.Console.Write("ObjectButtons: OnInputClicked()");

        //Make sure variable is up to date
        objectsCreated = objectSelectionHandler.objectsCreated;

        //The reason I do it before I actually create the objects, is so that it creates a "lock" on 
        //creating on object, however it doesn't actually make a lock so errors could happen with multiple
        //hololenses 
        //see if I can make an actual lock in c#
        objectSelectionHandler.objectsCreated += 1;
        objectsCreated += 1;

        //parentMenu.SetActive(false);

        GameObject createdObject;
        createdObject = (GameObject)Instantiate(selectionObject);
        createdObject.transform.position = parentMenu.transform.position /*- parentMenu.transform.forward*/;
        createdObject.SetActive(true);
        
        if (objectsCreated == numPlayers)
        {
            // want to hide object menu until next round
            parentMenu.SetActive(false);
            // randomize menu with 5 obstacles to choose from
            //  objectSelectionHandler.objectsCreated = 0;
            objectsCreated = 0;
        }
    }
}
