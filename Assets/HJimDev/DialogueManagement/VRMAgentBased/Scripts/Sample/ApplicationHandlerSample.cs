using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        namespace Sample
        {
            public class ApplicationHandlerSample : Core.ApplicationHandler
            {
                [SerializeField] Core.DialogueManager dialogueManager;

                private void Update()
                {
                    if (dialogueManager.IsPlaying && Input.GetKeyDown(KeyCode.Space))
                    {
                        dialogueManager.Next();
                    }
                }

                public override void SetEnable(string element, bool value) { }

                public override void HandleEvent(string eventID, string parameters = "") { }

                public override void NotifyDialogueEnded(string dialogueID) { }
            }
        }
    }
}