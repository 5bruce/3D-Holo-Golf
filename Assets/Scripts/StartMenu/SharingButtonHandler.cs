using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharingButtonHandler : MonoBehaviour,
                                    IFocusable,
                                    IInputClickHandler
{
    public Material active_material;
    public Material inactive_material;

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
        Debug.Log(this.name + ": OnFocusEnter()");
        gameObject.GetComponent<MeshRenderer>().material = active_material;
    }

    void IFocusable.OnFocusExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = inactive_material;
    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        /*
         Note: this operation is not thread safe since not mutual exclusion
         and SettingsManager quits program if isPlayAndPassGame && isSharedGame
         are both true. However, I think Unity only uses a single thread.
         */
        SettingsManager.Instance.toggleSharedGame();
    }
}
