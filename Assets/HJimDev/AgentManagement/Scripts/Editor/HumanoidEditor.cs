using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AgentManagement
{
    namespace Editor
    {
        public class HumanoidEditor : GenericAgentEditor
        {
            [MenuItem("Agent/Create/Humanoid Agent", priority = 0)]
            public new static void ShowWindow()
            {
                GetWindow<HumanoidEditor>("Humanoid Agent");
            }

            protected override void OnGUI()
            {
                modelPrefab = EditorGUILayout.ObjectField("Model", modelPrefab, typeof(GameObject), false) as GameObject;
                DefaultOnGUI(modelPrefab);
            }

            protected override bool ValidateAgentPrefab(out GameObject agent)
            {
                EditorVariables variables = CreateInstance<EditorVariables>();
                if (variables.HumanoidPrefab == null)
                {
                    agent = null;
                    Debug.LogError("Base Humanoid Prefab not assigned. Please assign a base prefab in the EditorVariables script.");
                }
                else
                {
                    agent = PrefabUtility.InstantiatePrefab(variables.HumanoidPrefab) as GameObject;
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

                PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(modelPrefab);
                if (prefabType == PrefabAssetType.NotAPrefab)
                {
                    Debug.LogError("Selected model is not a prefab. Please assign a prefab.");
                    modelInstance = null;
                    return false;
                }
                if (prefabType != PrefabAssetType.Model)
                {
                    Debug.LogError("Selected model must be a 3D model. Please assign a valid model.");
                    modelInstance = null;
                    return false;
                }

                return true;
            }

            protected override void AssignAnimatorController(Animator animator)
            {
                animator.runtimeAnimatorController = CreateInstance<EditorVariables>().HumanoidAnimatorController;
            }
        }
    }
}