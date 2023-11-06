using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueManagement.Handlers;

public class DefaultChatApplicationHandler : ApplicationHandler
{
    public override void SetEnable(string element, bool value)
    {
        if( element == "CameraRotation")
        {
            // enable/disable camera rotation
        }
        else if(element == "UserInteractions")
        {
            // enable/disable user interactions
        }
        else if (element == "Mouse")
        {
            // enable/disable mouse
        }
    }

    public override void HandleEvent(string eventID, string parameters = "")
    {
        if(eventID == "LoadScene")
        {
            // load scene
        }
        else if(eventID == "SaveData")
        {
            // save data
        }
        else if(eventID == "ShowChat")
        {
            // show chat
        }
        // etc...
    }

    public override void NotifyDialogueEnded(string dialogueID)
    {
        // musicController.Play("stop");
        // musicController.StopSound();
    }
}
