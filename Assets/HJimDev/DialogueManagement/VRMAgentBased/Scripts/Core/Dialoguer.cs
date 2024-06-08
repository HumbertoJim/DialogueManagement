using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AgentManagement.Agents;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        public class Dialoguer: Core.Dialoguer
        {
            [SerializeField] VRMAgent agent;

            public VRMAgent Agent { get { return agent; } }

            public override void Talk(AudioClip audio = null, string text = null) { }

            public override void StopTalking() { }
        }
    }
}