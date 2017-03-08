using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenuButtonHandler: MonoBehaviour,
                                      IFocusable,
                                      IInputClickHandler
{
    public Material active_material;
    public Material inactive_material;

    [Tooltip("Menu GameObjects to be deactivated when button pressed")]
    public GameObject[] toDeactivate;
    public GameObject toActivate;

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
        Debug.Log(this.name + ": OnInputClicked()");
        toActivate.SetActive(true);
        for (int i=0; i < toDeactivate.Length; i++)
        {
            toDeactivate[i].SetActive(false);
        }
    }
}
