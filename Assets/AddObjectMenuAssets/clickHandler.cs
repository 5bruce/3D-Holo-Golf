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

    private ObjectSelectionHandler objectSelectionHandler;

    // Use this for initialization
    void Start()
    {

        objectSelectionHandler = ObjectSelectionHandler.Instance;
        objectSelectionHandler.prepareGameObjectMenu();
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
        //Debug.Log(gameObject.name + ": OnInputClicked()");
        //System.Console.Write("ObjectButtons: OnInputClicked()");

        //The reason I do it before I actually create the objects, is so that it creates a "lock" on 
        //creating on object, however it doesn't actually make a lock so errors could happen with multiple
        //hololenses 
        //see if I can make an actual lock in c#
        objectSelectionHandler.objectsCreated += 1;

        //parentMenu.SetActive(false);

        GameObject createdObject;
        createdObject = (GameObject)Instantiate(selectionObject);
        createdObject.transform.position = parentMenu.transform.position /*- parentMenu.transform.forward*/;
        createdObject.SetActive(true);

        createdObject.AddComponent<HandDraggable>();
        gameObject.GetComponent<HandDraggable>().StartedDragging += ObjectSelectionHandler_StartedDragging;
        gameObject.GetComponent<HandDraggable>().StoppedDragging += ObjectSelectionHandler_StartedDragging;

        objectSelectionHandler.currentObjects[objectSelectionHandler.objectsCreated - 1] = createdObject;

        if (objectSelectionHandler.isPlayAndPassGame)
        {
            objectSelectionHandler.prepareGameObjectMenu();
        }
        
    }


    public void ObjectSelectionHandler_StartedDragging()
    {
        parentMenu.SetActive(false);
    }

    public void ObjectSelectionHandler_StoppedDragging()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": StoppedDragging event handler called");
        parentMenu.SetActive(true);
    }
}
