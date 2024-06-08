using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AgentManagement
{
    namespace Actions
    {
        public class AgentAction : ScriptableObject
        {
            [Header("Animation")]
            public string animation = "";

            [Header("Facial Expression")]
            public string facialExpression = "";

            [Header("Audio")]
            public AudioClip audio;
            public string audioText = "";

            [Header("Goto")]
            public List<Vector3> path;
        }
    }
}