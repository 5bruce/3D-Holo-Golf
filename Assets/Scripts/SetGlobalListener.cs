using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Register this game object on the InputManager as a global listener.
/// </summary>
public class SetGlobalListener : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
    }

    private void OnDestroy()
    {
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }
}