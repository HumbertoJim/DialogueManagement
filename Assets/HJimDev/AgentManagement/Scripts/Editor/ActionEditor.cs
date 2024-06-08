using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AgentManagement.Actions;

namespace AgentManagement
{
    namespace Editor
    {
        public class ActionEditor : EditorWindow
        {
            [MenuItem("Agent/Create/Action")]
            public static void ShowWindow()
            {
                GetWindow<ActionEditor>("Create Action");
            }

            private void OnGUI()
            {
                string animation = EditorGUILayout.TextField("Animation trigger", "");
                string facialExpression = EditorGUILayout.TextField("Facial expression", "");
                AudioClip audioClip = EditorGUILayout.ObjectField("Audio clip", null, typeof(AudioClip), false) as AudioClip;
                string audioText = EditorGUILayout.TextField("Audio text", "");
                Vector3 target = EditorGUILayout.Vector3Field("Target position", Vector3.zero);

                if (GUILayout.Button("Create Action"))
                {
                    AgentAction action = CreateInstance<AgentAction>();
                    action.animation = animation;
                    action.facialExpression = facialExpression;
                    action.audio = audioClip;
                    action.audioText = audioText;
                    action.path = new();
                    action.path.Add(target);

                    CreateScriptableObject(action);
                }
            }

            private void CreateScriptableObject(AgentAction action)
            {
                string localPath = EditorUtility.SaveFilePanelInProject("Save action", "action", "asset", "Save your agent action");

                if (string.IsNullOrEmpty(localPath))
                {
                    Debug.Log("Action creation cancelled.");
                    return;
                }

                AssetDatabase.CreateAsset(action, localPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Agent action was saved successfully at: " + localPath);
            }
        }
    }
}