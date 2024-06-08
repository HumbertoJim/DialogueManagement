using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AgentManagement
{
    namespace Editor
    {
        public class EditorVariables: ScriptableObject
        {
            [Header("Agent Prefab")]
            [SerializeField] GameObject genericPrefab;
            [SerializeField] GameObject humanoidPrefab;
            [SerializeField] GameObject vrmPrefab;

            [Header("Animator")]
            [SerializeField] RuntimeAnimatorController genericAnimatorController;
            [SerializeField] RuntimeAnimatorController humanoidAnimatorController;
            [SerializeField] RuntimeAnimatorController vrmAnimatorController;

            public GameObject GenericPrefab { get { return genericPrefab; } }
            public GameObject HumanoidPrefab { get { return humanoidPrefab; } }
            public GameObject VrmPrefab { get { return vrmPrefab; } }
            public RuntimeAnimatorController GenericAnimatorController { get { return genericAnimatorController; } }
            public RuntimeAnimatorController HumanoidAnimatorController { get { return humanoidAnimatorController; } }
            public RuntimeAnimatorController VrmAnimatorController { get { return vrmAnimatorController; } }
        }
    }
}