using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DialogueManagement.Core;
using DialogueManagement.Tools;

public class SystemDialogueModule : Module
{
    public override string ModuleName => "sys";

    const string KEY_SLEEP = "sleep";
    const string KEY_CHOICE = "choice";
    const string KEY_FLAG = "flag"; // para configurar una variable.

    const string KEY_CHOICE_SEPARATOR = "|";

    public override void HandleCommand(string command)
    {
        if (StringExtension.TextStartsWith(command, KEY_CHOICE))
        {
            command = command.Substring(KEY_CHOICE.Length).Trim();

            dialogueManager.IsChoicing = true;

            string[] choices = command.Split(new string[] { KEY_CHOICE_SEPARATOR }, StringSplitOptions.None);
            command = choices[0].Split(' ')[0];
            choices[0] = choices[0].Substring(command.Length).Trim();
            for (int i = 0; i < choices.Length; i++)
            {
                ChoiceController choiceController = Instantiate(dialogueManager.ChoicePrefab, dialogueManager.ChoiceContainer).GetComponent<ChoiceController>();
                choiceController.SetInformation(dialogueManager, command, i, choices[i].Trim());
            }
        }
        else if (StringExtension.TextStartsWith(command, KEY_FLAG))
        {
            dialogueManager.SetFlag(command, true);
        }
        else if (StringExtension.TextStartsWith(command, KEY_SLEEP))
        {
            command = command.Substring(KEY_SLEEP.Length).Trim();
            dialogueManager.Sleep(float.Parse(command));
        }
        else
        {
            dialogueManager.ErrorMessages = "KEY SYS COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
        }
    }
}
