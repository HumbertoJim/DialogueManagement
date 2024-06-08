using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using AgentManagement.Actions;
using AgentManagement.Agents;

namespace AgentManagement
{
    namespace Editor
    {
        public class GenericAgentEditor: EditorWindow
        {
            [Header("Defaults")]
            [SerializeField] protected GameObject modelPrefab;
            [SerializeField] AgentAction idleAction;
            [SerializeField] AgentAction defaultAction;

            float minTimeForRandom = 5f;
            float maxTimeForRandom = 10f;
            bool enabledRandomAction = true;

            // Movement
            float movementSpeed = 2f;
            float maxAllowedFarDistance = 15f;
            bool enabledMovement = true;

            // Random Walk
            float randomWalkRange = 10f;
            bool enableRandomWalk = true;

            // Rotation
            float rotationTime = 3f;

            // Audio
            AudioClip defaultClip;


        [MenuItem("Agent/Create/Generic Agent", priority = 0)]
            public static void ShowWindow()
            {
                GetWindow<GenericAgentEditor>("Generic Agent");
            }

            protected virtual void OnGUI()
            {
                modelPrefab = EditorGUILayout.ObjectField("Model", modelPrefab, typeof(GameObject), false) as GameObject;
                DefaultOnGUI(modelPrefab);
            }

            protected void DefaultOnGUI(GameObject modelPrefab)
            {
                // actions
                idleAction = EditorGUILayout.ObjectField("Idle action", idleAction, typeof(AgentAction), false) as AgentAction;
                defaultAction = EditorGUILayout.ObjectField("Default action", defaultAction, typeof(AgentAction), false) as AgentAction;
                minTimeForRandom = EditorGUILayout.FloatField("Min time for random action", minTimeForRandom);
                maxTimeForRandom = EditorGUILayout.FloatField("Max time for random action", maxTimeForRandom);
                enabledRandomAction = EditorGUILayout.Toggle("Enable random action", enabledRandomAction);

                // Movement
                movementSpeed = EditorGUILayout.FloatField("Movement speed", movementSpeed);
                maxAllowedFarDistance = EditorGUILayout.FloatField("Max allowed far distance", maxAllowedFarDistance);
                enabledMovement = EditorGUILayout.Toggle("Enable random action", enabledMovement);

                // Random Walk
                randomWalkRange = EditorGUILayout.FloatField("Random walk range", randomWalkRange);
                enableRandomWalk = EditorGUILayout.Toggle("Enable random action", enableRandomWalk);

                // Rotation
                rotationTime = EditorGUILayout.FloatField("RotationTime", rotationTime);

                // Audio
                defaultClip = EditorGUILayout.ObjectField("Default clip", defaultClip, typeof(AudioClip), false) as AudioClip;

                if (GUILayout.Button("Create Agent"))
                {
                    if (!ValidateAgentPrefab(out GameObject agent))
                    {
                        DestroyImmediate(agent);
                        return;
                    }
                    if (!ValidateModelPrefab(modelPrefab, out GameObject modelInstance))
                    {
                        DestroyImmediate(agent);
                        DestroyImmediate(modelInstance);
                        return;
                    }
                    GenericAgent genericAgent = agent.GetComponent<GenericAgent>();
                    genericAgent.SetPrefabAttributes(
                        modelInstance,
                        minTimeForRandom,
                        maxTimeForRandom,
                        idleAction,
                        defaultAction,
                        enabledRandomAction,
                        movementSpeed,
                        maxAllowedFarDistance,
                        enabledMovement,
                        randomWalkRange,
                        enableRandomWalk,
                        rotationTime,
                        defaultClip
                    );
                    CreatePrefab(agent);
                    DestroyImmediate(agent);
                }
            }

            private void CreatePrefab(GameObject instantiatedPrefab)
            {
                string localPath = EditorUtility.SaveFilePanelInProject("Save agent", "agent", "prefab", "Save your agent");

                if (string.IsNullOrEmpty(localPath))
                {
                    Debug.Log("Agent Prefab creation cancelled.");
                    return;
                }

                PrefabUtility.SaveAsPrefabAsset(instantiatedPrefab, localPath, out bool prefabSuccess);

                if (prefabSuccess)
                {
                    Debug.Log("Agent Prefab was saved successfully at: " + localPath);
                }
                else
                {
                    Debug.Log("Agent Prefab failed to save.");
                }
            }

            protected virtual bool ValidateAgentPrefab(out GameObject agent)
            {
                EditorVariables variables = CreateInstance<EditorVariables>();
                if (variables.GenericPrefab == null)
                {
                    agent = null;
                    Debug.LogError("Base Generic Prefab not assigned. Please assign a base prefab in the EditorVariables script.");
                }
                else
                {
                    agent = PrefabUtility.InstantiatePrefab(variables.GenericPrefab) as GameObject;
                }

                if (agent == null)
                {
                    Debug.LogError("Failed to instantiate the agent prefab.");
                    return false;
                }
                return true;
            }

            protected virtual bool ValidateModelPrefab(GameObject modelPrefab, out GameObject modelInstance)
            {
                if (modelPrefab == null)
                {
                    Debug.LogError("Model not assigned. Please assign a model.");
                    modelInstance = null;
                    return false;
                }

                modelInstance = PrefabUtility.InstantiatePrefab(modelPrefab) as GameObject;
                if (modelInstance == null)
                {
                    Debug.LogError("Failed to instantiate the agent model.");
                    return false;
                }

                if (modelInstance.GetComponent<Animator>() == null)
                {
                    Debug.LogWarning("Model must have an Animator. Default Animator was added.");
                    modelInstance.AddComponent<Animator>();
                    AssignAnimatorController(modelInstance.GetComponent<Animator>());
                }
                else if(modelInstance.GetComponent<Animator>().runtimeAnimatorController == null)
                {
                    Debug.LogWarning("Model Animator must have an Animator Controller. Default Animator Controller was added.");
                    AssignAnimatorController(modelInstance.GetComponent<Animator>());
                }

                return true;
            }

            protected virtual void AssignAnimatorController(Animator animator)
            {
                animator.runtimeAnimatorController = CreateInstance<EditorVariables>().GenericAnimatorController;
            }
        }
    }
}