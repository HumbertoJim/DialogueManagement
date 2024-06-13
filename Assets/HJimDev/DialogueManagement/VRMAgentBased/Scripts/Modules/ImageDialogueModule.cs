using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        public class ImageDialogueModule : Core.Module
        {
            [Header("Resources Settings")]
            [SerializeField] string resourcesFolderLocation = "Dialogue/Images";

            public override string ModuleName => "im";

            const string KEY_SHOW = "show";
            const string KEY_HIDE = "hide";

            private void Start()
            {
                if (resourcesFolderLocation != "" && resourcesFolderLocation[^1] != '/')
                {
                    resourcesFolderLocation += '/';
                }
            }

            public override void HandleCommand(string command)
            {
                if (Tools.StringExtensions.TextStartsWith(command, KEY_SHOW))
                {
                    dialogueManager.IsImageShown = true;
                    dialogueManager.ApplicationHandler.SetEnable("CameraRotation", false);
                    dialogueManager.ApplicationHandler.SetEnable("UserInteractions", false);
                    command = command.Substring(KEY_SHOW.Length).Trim();
                    if (command != "")
                    {
                        dialogueManager.UI.ImageLabel.sprite = Resources.Load<Sprite>(resourcesFolderLocation + command);
                    }
                    dialogueManager.UI.ImageLabel.color = new Color(1, 1, 1, 1);
                }
                else if (Tools.StringExtensions.TextStartsWith(command, KEY_HIDE))
                {
                    if (!dialogueManager.IsChoicing && !dialogueManager.OnFadeIn)
                    {
                        dialogueManager.ApplicationHandler.SetEnable("CameraRotation", true);
                        dialogueManager.ApplicationHandler.SetEnable("UserInteractions", true);
                    }
                    dialogueManager.UI.ImageLabel.color = new Color(1, 1, 1, 0);
                    dialogueManager.IsImageShown = false;
                }
                else
                {
                    dialogueManager.UI.ErrorMessages = "KEY IMAGE COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
                }
            }
        }
    }
}