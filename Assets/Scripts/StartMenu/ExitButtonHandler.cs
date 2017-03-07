using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ExitButtonHandler : MonoBehaviour,
                                 IFocusable,
                                 IInputClickHandler
{
    public Material active_material;
    public Material inactive_material;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
        Debug.Log("ExitButtonHandler: OnInputClicked()");
        Application.Quit();
    }
}
