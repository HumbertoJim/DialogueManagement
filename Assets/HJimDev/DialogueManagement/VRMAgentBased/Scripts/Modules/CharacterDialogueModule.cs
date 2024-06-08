using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        public class CharacterDialogueModule : Core.Module
        {
            [Header("Dialoguers")]
            [SerializeField] DialoguerCluster dialoguers;
            const string INITIAL_COMMAND = "initial";
            const string LOAD_COMMAND = "";
            const string MOOD_COMMAND = "";
            const string ANIM_COMMAND = "";
            const string MOVE_COMMAND = "";
            const string ROTATE_COMMAND = "";
            const string IMPROVE_RELATION_COMMAND = "";
            const string DECREASE_RELATION_COMMAND = "";
            const string REGISTER_INTERACTION_COMMAND = "";
            const string ENABLE_COMMAND = "";
            const string DISABLE_COMMAND = "";

            public override void HandleCommand(string command)
            {
                if (Tools.StringExtensions.TextStartsWith(command, LOAD_COMMAND))
                {
                    command = command[LOAD_COMMAND.Length..].Trim();
                    Dialoguer dialoguer = dialoguers.GetDialoguer(command);
                    if (dialoguer)
                    {
                        dialoguer.Agent.Model.LookAt(dialoguers.MainCharacter.Agent.Model.Head);
                        dialoguer.Agent.Rotate(dialoguers.MainCharacter.Agent.Model.Transform);
                    }
                }
                else if (Tools.StringExtensions.TextStartsWith(command, MOOD_COMMAND))
                {
                    command = command[MOOD_COMMAND.Length..].Trim();
                    string[] sentence = command.Split(' ');
                    if (sentence.Length > 1)
                    {
                        Dialoguer dialoguer = dialoguers.GetDialoguer(command);
                        if (dialoguer)
                        {
                            dialoguer.Agent.FacialExpression(command[sentence[0].Length..].Trim());
                        }
                    }
                }
                else if (Tools.StringExtensions.TextStartsWith(command, ANIM_COMMAND))
                {
                    command = command[ANIM_COMMAND.Length..].Trim();
                    string[] sentence = command.Split(' ');
                    if (sentence.Length > 1)
                    {
                        Dialoguer dialoguer = dialoguers.GetDialoguer(command);
                        if (dialoguer)
                        {
                            dialoguer.Agent.Model.Anim(command[sentence[0].Length..].Trim());
                        }
                    }
                }
                else if (Tools.StringExtensions.TextStartsWith(command, MOVE_COMMAND))
                {
                    command = command[MOVE_COMMAND.Length..].Trim();
                    string[] sentence = command.Split(' ');
                    if (sentence.Length > 1)
                    {
                        Dialoguer dialoguer = dialoguers.GetDialoguer(command);
                        if (dialoguer)
                        {
                            string[] position = command[sentence[0].Length..].Trim().Split(',');
                            dialoguer.Agent.Goto(new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2])), enqueue_path: false);
                        }
                    }
                }
                else if (Tools.StringExtensions.TextStartsWith(command, ROTATE_COMMAND))
                {
                    command = command[ROTATE_COMMAND.Length..].Trim();
                    string[] sentence = command.Split(' ');
                    if (sentence.Length > 1)
                    {
                        Dialoguer dialoguer = dialoguers.GetDialoguer(command);
                        if (dialoguer)
                        {
                            string[] rotation = command[sentence[0].Length..].Trim().Split(',');
                            dialoguer.Agent.Rotate(new Vector3(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2])));
                        }
                    }
                }
                else if (Tools.StringExtensions.TextStartsWith(command, IMPROVE_RELATION_COMMAND))
                {
                    command = command[IMPROVE_RELATION_COMMAND.Length..].Trim();
                    string[] sentence = command.Split(' ');
                    if (sentence.Length > 1)
                    {
                        dialoguers.ImproveRelationship(command, int.Parse(command[sentence[0].Length..].Trim()));
                    }
                }
                else if (Tools.StringExtensions.TextStartsWith(command, DECREASE_RELATION_COMMAND))
                {
                    command = command[DECREASE_RELATION_COMMAND.Length..].Trim();
                    string[] sentence = command.Split(' ');
                    if (sentence.Length > 1)
                    {
                        dialoguers.DecreaseRelationship(command, int.Parse(command[sentence[0].Length..].Trim()));
                    }
                }
                else if (Tools.StringExtensions.TextStartsWith(command, REGISTER_INTERACTION_COMMAND))
                {
                    command = command[REGISTER_INTERACTION_COMMAND.Length..].Trim();
                    dialoguers.RegisterInteraction(command);
                }
                else if (Tools.StringExtensions.TextStartsWith(command, ENABLE_COMMAND))
                {
                    command = command[ENABLE_COMMAND.Length..].Trim();
                    dialoguers.SetEnable(command, true);
                }
                else if (Tools.StringExtensions.TextStartsWith(command, DISABLE_COMMAND))
                {
                    command = command[DISABLE_COMMAND.Length..].Trim();
                    dialoguers.SetEnable(command, false);
                }
                else
                {
                    dialogueManager.ErrorMessages = "KEY CHARACTER COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
                }
            }
        }
    }
}