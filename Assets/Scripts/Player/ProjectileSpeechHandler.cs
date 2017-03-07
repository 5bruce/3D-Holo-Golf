using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class ProjectileSpeechHandler : MonoBehaviour, 
                                       ISpeechHandler
{
    // ISpeechHandler only works on object that are in focus/gazed on
    // Need to attach SetGlobalListener.cs component as well to have global listening
    void ISpeechHandler.OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": key word = " + eventData.RecognizedText);
        if (eventData.RecognizedText == "Reset Ball")
        {
            SendMessageUpwards("OnReset");
        }
    }
}
