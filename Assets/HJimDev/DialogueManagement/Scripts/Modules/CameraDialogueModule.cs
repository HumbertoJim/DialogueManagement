using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueManagement.Core;
using DialogueManagement.Tools;

public class CameraDialogueModule : Module
{
    const string KEY_FADE = "fade";
    const string INITIAL_COMMAND = "initial";
    const string EMPTY_OPTION = "-e";
    const string ENTER_OPTION = "-n";

    public override void HandleCommand(string command)
    {
        if (StringExtension.TextStartsWith(command, KEY_FADE))
        {
            command = command.Substring(KEY_FADE.Length).Trim();
            if (command.ToLower() == "true")
            {
                dialogueManager.ApplicationHandler.SetEnable("cameraRotation", false);
                dialogueManager.ApplicationHandler.SetEnable("interactions", false);
                dialogueManager.FadeIn();
            }
            else
            {
                dialogueManager.FadeOut();
                dialogueManager.ApplicationHandler.SetEnable("cameraRotation", true);
                dialogueManager.ApplicationHandler.SetEnable("interactions", true);
            }
        }
        else
        {
            dialogueManager.ErrorMessages = "KEY CAMERA COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
        }
    }
}
