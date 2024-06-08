using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        namespace Defaults
        {
            public class DefaultApplicationHandler : ApplicationHandler
            {
                public override void SetEnable(string element, bool value) { }

                public override void HandleEvent(string eventID, string parameters = "") { }

                public override void NotifyDialogueEnded(string dialogueID) { }
            }
        }
    }
}