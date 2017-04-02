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
    }

    // Update is called once per frame
    void Update()
    {
        //in case the other buttons have been clicked
    }

    void IFocusable.OnFocusEnter()
    {
        //Debug.Log(gameObject.name + ": " + this.GetType().Name + ": OnFocusEnter()");
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

        // create associated object and place in front of player
        Debug.Log(gameObject.name + ": " + this.GetType().Name + 
            ": spawning object: " + this.selectionObject.name);
        GameObject createdObject;
        createdObject = (GameObject)Instantiate(selectionObject);
        createdObject.transform.position = Camera.main.transform.position + Vector3.Normalize(Camera.main.transform.forward) * 2;
        createdObject.SetActive(true);

        // make object handdraggable and setup its relation to obstacleSelectionMenu
        createdObject.AddComponent<HandDraggable>();
        createdObject.GetComponent<HandDraggable>().StartedDragging += clickHandler_StartedDragging;
        // only bring back menu after dragging if not last player of current selection round and have multiple players
        if (objectSelectionHandler.numPlayers > 1 && objectSelectionHandler.isPlayAndPassGame) {
            Debug.Log(string.Format("{0} : {1}: objects created={2}: numPlayers={3}", 
                gameObject.name, this.GetType().Name, objectSelectionHandler.objectsCreated, objectSelectionHandler.numPlayers));
            if (objectSelectionHandler.objectsCreated < objectSelectionHandler.numPlayers) {
                createdObject.GetComponent<HandDraggable>().StoppedDragging += clickHandler_StoppedDragging;
            }
            else
            {
                // FIXME: this is just a temporary fix until I can fix the projectil-always-handdragging bug
                createdObject.GetComponent<HandDraggable>().StoppedDragging += clickHandler_lastPlayer_StoppedDragging;
            }
        }
        else
        {
            // FIXME: this is just a temporary fix until I can fix the projectil-always-handdragging bug
            createdObject.GetComponent<HandDraggable>().StoppedDragging += clickHandler_lastPlayer_StoppedDragging;
        }

        // add this createdObject to the list of obstacles created during this round
        objectSelectionHandler.currentObjects.Enqueue(createdObject);

        // randomize selection menu for next player if there are any remaining
        if (objectSelectionHandler.isPlayAndPassGame &&  
            objectSelectionHandler.objectsCreated < objectSelectionHandler.numPlayers)
        {
            objectSelectionHandler.prepareGameObjectMenu();

            // switch to next player
            PlayAndPassManager.Instance.ChangeActivePlayer();
        }
    }

    // TODO: these event handlers should probably be moved to the obstacle menu manager class (ObjectSelectionHandler.cs)
    public void clickHandler_StartedDragging()
    {
        Debug.Log(this.GetType().Name + ": clickHandler_StartedDragging()");
        parentMenu.SetActive(false);
    }

    public void clickHandler_StoppedDragging()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": clickHandler_StoppedDragging()");
        parentMenu.SetActive(true);
    }

    public void clickHandler_lastPlayer_StoppedDragging()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": clickHandler_lastPlayer_StoppedDragging()");
        parentMenu.SetActive(true);
        gameObject.transform.root.SendMessage("cleanupSelectionRound");
    }
}
