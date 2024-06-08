using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        public class TextDialogueModule : Core.Module
        {
            public override string ModuleName => "text";

            const string INITIAL_COMMAND = "initial";
            const string EMPTY_OPTION = "-e";
            const string ENTER_OPTION = "-n";

            public override void HandleCommand(string command)
            {
                if (command == INITIAL_COMMAND)
                {
                    if (Tools.StringExtensions.TextStartsWith(command, EMPTY_OPTION))
                    {
                        command = command.Substring(EMPTY_OPTION.Length).Trim();
                    }
                    else if (Tools.StringExtensions.TextStartsWith(command, ENTER_OPTION))
                    {
                        command = command.Substring(ENTER_OPTION.Length).Trim() + "\n";
                    }
                    else
                    {
                        command = command.Substring(ENTER_OPTION.Length).Trim() + " ";
                    }
                    dialogueManager.InitialText = command;
                }
                else
                {
                    dialogueManager.ErrorMessages = "KEY TEXT COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
                }
            }
        }
    }
}