using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UniVRM10;

namespace AgentManagement
{
    namespace Editor
    {
        public class VRMAgentEditor : HumanoidEditor
        {
            [MenuItem("Agent/Create/VRM Agent", priority = 0)]
            public new static void ShowWindow()
            {
                GetWindow<VRMAgentEditor>("VRM Agent");
            }

            protected override void OnGUI()
            {
                modelPrefab = EditorGUILayout.ObjectField("Model", modelPrefab, typeof(GameObject), false) as GameObject;
                if (modelPrefab != null && !AssetDatabase.GetAssetPath(modelPrefab).EndsWith(".vrm", System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogError("Invalid VRM model. Please select a .vrm file.");
                    modelPrefab = null;
                }
                DefaultOnGUI(modelPrefab);
            }

            protected override bool ValidateAgentPrefab(out GameObject agent)
            {
                EditorVariables variables = CreateInstance<EditorVariables>();
                if (variables.VrmPrefab == null)
                {
                    agent = null;
                    Debug.LogError("Base VRM Agent Prefab not assigned. Please assign a base prefab in the EditorVariables script.");
                }
                else
                {
                    agent = PrefabUtility.InstantiatePrefab(variables.VrmPrefab) as GameObject;
                }

                if (agent == null)
                {
                    Debug.LogError("Failed to instantiate the agent prefab.");
                    return false;
                }
                return true;
            }

            protected override bool ValidateModelPrefab(GameObject modelPrefab, out GameObject modelInstance)
            {
                if (!base.ValidateModelPrefab(modelPrefab, out modelInstance))
                {
                    return false;
                }

                if (modelPrefab.GetComponent<Vrm10Instance>() == null)
                {
                    Debug.LogError("Selected model is not a VRM model. Please assign a valid model.");
                    modelInstance = null;
                    return false;
                }

                return true;
            }

            protected override void AssignAnimatorController(Animator animator)
            {
                animator.runtimeAnimatorController = CreateInstance<EditorVariables>().VrmAnimatorController;
            }
        }
    }
}