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
            public class StorytellerSample : Entities.Storyteller
            {
                public override void Talk(AudioClip audio = null, string text = null) { }

                public override void StopTalking() { }

                public override void SetEnable(bool value) { }
            }
        }
    }
}