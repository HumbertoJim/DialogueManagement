using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace Base
    {
        namespace Modules
        {
            public class TextModule : Module
            {
                public override string ModuleName => "text";

                const string INITIAL_COMMAND = "initial";
                const string EMPTY_OPTION = "-e";
                const string ENTER_OPTION = "-n";

                public override void HandleCommand(string command)
                {
                    if (Tools.StringExtensions.TextStartsWith(command, INITIAL_COMMAND))
                    {
                        command = command[INITIAL_COMMAND.Length..].Trim();
                        if (Tools.StringExtensions.TextStartsWith(command, EMPTY_OPTION))
                        {
                            command = command[EMPTY_OPTION.Length..].Trim();
                        }
                        else if (Tools.StringExtensions.TextStartsWith(command, ENTER_OPTION))
                        {
                            command = command[ENTER_OPTION.Length..].Trim() + "\n";
                        }
                        else
                        {
                            command = command[ENTER_OPTION.Length..].Trim() + " ";
                        }
                        dialogueManager.InitialText = command;
                    }
                    else
                    {
                        dialogueManager.UI.ErrorMessages = "KEY TEXT COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
                    }
                }
            }
        }
    }
}