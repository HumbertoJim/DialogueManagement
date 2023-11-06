using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueManagement.Core;

public class TextDialogueModule : Module
{
    const string INITIAL_COMMAND = "initial";
    const string EMPTY_OPTION = "-e";
    const string ENTER_OPTION = "-n";

    public override void HandleCommand(string command)
    {
        if (command == INITIAL_COMMAND)
        {
            if (DialogueManagement.Tools.StringExtension.TextStartsWith(command, EMPTY_OPTION))
            {
                command = command.Substring(EMPTY_OPTION.Length).Trim();
            }
            else if (DialogueManagement.Tools.StringExtension.TextStartsWith(command, ENTER_OPTION))
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
