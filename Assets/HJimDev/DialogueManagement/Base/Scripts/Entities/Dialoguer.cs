using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Base
    {
        namespace Entities
        {
            public abstract class Dialoguer : MonoBehaviour
            {
                [SerializeField] string code;
                public string Code { get { return code; } }

                public abstract void Talk(AudioClip audio = null, string text = null);

                public abstract void StopTalking();

                public abstract void SetEnable(bool value);
            }
        }
    }
}