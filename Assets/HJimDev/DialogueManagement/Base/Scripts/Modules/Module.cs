using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Base
    {
        namespace Modules
        {
            public class Module : MonoBehaviour
            {
                public virtual string ModuleName { get { throw new NotImplementedException(); } }

                protected Core.DialogueManager dialogueManager;

                public void SetInformation(Core.DialogueManager dialogueManager)
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
}