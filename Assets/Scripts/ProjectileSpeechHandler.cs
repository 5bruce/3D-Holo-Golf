using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class ProjectileSpeechHandler : MonoBehaviour, 
                                       ISpeechHandler {
    public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        if(eventData.RecognizedText == "Reset Ball")
        {
            Debug.Log("ProjectileSpeechHandler 'Reset Ball' kword recognized");
            SendMessageUpwards("OnReset");
        }
    }
}
