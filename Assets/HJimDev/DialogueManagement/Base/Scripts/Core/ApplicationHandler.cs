using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Core
    {
        public abstract class ApplicationHandler: MonoBehaviour
        {
            public abstract void SetEnable(string element, bool value);

            public abstract void HandleEvent(string eventID, string parameters = "");

            public abstract void NotifyDialogueEnded(string dialogueID);
        }
    }
}