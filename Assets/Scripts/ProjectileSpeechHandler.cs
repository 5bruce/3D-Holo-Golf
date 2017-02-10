using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class ProjectileSpeechHandler : MonoBehaviour, 
                                       ISpeechHandler
{
    // FIXME: I'm not sure that this is properly implementing the interface
    public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        if(eventData.RecognizedText == "Reset Ball")
        {
            Debug.Log("ProjectileSpeechHandler 'Reset Ball' kword recognized");
            SendMessageUpwards("OnReset");
        }
    }
}
