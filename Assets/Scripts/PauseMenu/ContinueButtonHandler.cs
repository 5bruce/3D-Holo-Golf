using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButtonHandler : MonoBehaviour,
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
        gameObject.GetComponent<MeshRenderer>().material = active_material;
    }

    void IFocusable.OnFocusExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = inactive_material;
    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("ContinueButtonHandler: OnInputClicked()");
        // unfreeze game time
        Time.timeScale = 1;
        // deactivate the whole menu, see http://answers.unity3d.com/answers/176504/view.html
        gameObject.transform.root.gameObject.SetActive(false);
    }
}