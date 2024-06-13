using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AgentManagement.Agents;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        namespace Entities
        {
            public class Storyteller : Base.Entities.Storyteller
            {
                public override void Talk(AudioClip audio = null, string text = null) { }

                public override void StopTalking() { }

                public override void SetEnable(bool value) { }
            }
        }
    }
}