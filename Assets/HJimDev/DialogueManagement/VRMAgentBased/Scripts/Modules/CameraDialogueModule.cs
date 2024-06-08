using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace Defaults
    {
        public class CameraDialogueModule : Core.Module
        {
            const string KEY_FADE = "fade";

            public override void HandleCommand(string command)
            {
                if (Tools.StringExtensions.TextStartsWith(command, KEY_FADE))
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
    }
}