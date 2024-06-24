using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        namespace Modules
        {
            public class CharacterModule : Base.Modules.CharacterModule
            {
                const string LOAD_COMMAND = "load";
                const string MOOD_COMMAND = "mood";
                const string ANIM_COMMAND = "anim";
                const string MOVE_COMMAND = "move";
                const string ROTATE_COMMAND = "rotate";
                const string ENABLE_COMMAND = "enable";
                const string DISABLE_COMMAND = "disable";

                [Header("Dialoguers")]
                [SerializeField] Core.DialoguerCluster dialoguers;

                public override void HandleCommand(string command)
                {
                    if (Tools.StringExtensions.TextStartsWith(command, LOAD_COMMAND))
                    {
                        command = command[LOAD_COMMAND.Length..].Trim();
                        Entities.Dialoguer dialoguer = dialoguers.GetDialoguer(command);
                        if (dialoguer)
                        {
                            dialoguer.Agent.Model.LookAt(dialoguers.MainCharacter.Agent.Model.Head);
                            dialoguer.Agent.Rotate(dialoguers.MainCharacter.Agent.Model.Transform);
                            dialoguer.Agent.EnableRandomAction = false;
                        }
                    }
                    else if (Tools.StringExtensions.TextStartsWith(command, MOOD_COMMAND))
                    {
                        command = command[MOOD_COMMAND.Length..].Trim();
                        string[] sentence = command.Split(' ');
                        if (sentence.Length > 1)
                        {
                            Entities.Dialoguer dialoguer = dialoguers.GetDialoguer(command);
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
                            Entities.Dialoguer dialoguer = dialoguers.GetDialoguer(command);
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
                            Entities.Dialoguer dialoguer = dialoguers.GetDialoguer(command);
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
                            Entities.Dialoguer dialoguer = dialoguers.GetDialoguer(command);
                            if (dialoguer)
                            {
                                string[] rotation = command[sentence[0].Length..].Trim().Split(',');
                                dialoguer.Agent.Rotate(new Vector3(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2])));
                            }
                        }
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
                        base.HandleCommand(command);
                    }
                }
            }
        }
    }
}