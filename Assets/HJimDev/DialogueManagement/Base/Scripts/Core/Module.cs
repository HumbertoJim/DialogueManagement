using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Core
    {
        public class Module : MonoBehaviour
        {
            public virtual string ModuleName { get { throw new NotImplementedException(); } }

            protected DialogueManager dialogueManager;

            public void SetInformation(DialogueManager dialogueManager)
            {
                this.dialogueManager = dialogueManager;
            }

            public virtual void HandleCommand(string command)
            {
                throw new NotImplementedException();
            }
        }
    }
}