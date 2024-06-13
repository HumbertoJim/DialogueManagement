using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Base
    {
        namespace Sample
        {
            public class ApplicationHandlerSample : Core.ApplicationHandler
            {
                [SerializeField] Core.DialogueManager dialogueManager;

                private void Update()
                {
                    if(dialogueManager.IsPlaying && Input.GetKeyDown(KeyCode.Space))
                    {
                        dialogueManager.Next();
                    }
                    else if(Input.GetKeyDown(KeyCode.Space))
                        {
                        Debug.Log("SE");
                    }
                }

                public override void SetEnable(string element, bool value)
                {

                }

                public override void HandleEvent(string eventID, string parameters = "")
                {

                }

                public override void NotifyDialogueEnded(string dialogueID)
                {

                }
            }
        }
    }
}