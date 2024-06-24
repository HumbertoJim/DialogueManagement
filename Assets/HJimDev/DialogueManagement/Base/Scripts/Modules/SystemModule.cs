using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



namespace DialogueManagement
{
    namespace Base
    {
        namespace Modules
        {
            public class SystemModule : Module
            {
                public override string ModuleName => "sys";

                const string EVENT_COMMAND = "event";
                const string SLEEP_COMMAND = "sleep";
                const string CHOICE_COMMAND = "choice";
                const string FLAG_COMMAND = "flag"; // para configurar una variable.

                const string KEY_CHOICE_SEPARATOR = "|";

                public override void HandleCommand(string command)
                {
                    if (Tools.StringExtensions.TextStartsWith(command, EVENT_COMMAND))
                    {
                        command = Tools.StringExtensions.CleanText(command.Substring(EVENT_COMMAND.Length));
                        string eventID = command.Split(' ')[0];
                        dialogueManager.ApplicationHandler.HandleEvent(eventID, command.Substring(eventID.Length).Trim());
                    }
                    else if (Tools.StringExtensions.TextStartsWith(command, CHOICE_COMMAND))
                    {
                        command = command.Substring(CHOICE_COMMAND.Length).Trim();

                        dialogueManager.IsChoicing = true;

                        string[] choices = command.Split(new string[] { KEY_CHOICE_SEPARATOR }, StringSplitOptions.None);
                        command = choices[0].Split(' ')[0];
                        choices[0] = choices[0].Substring(command.Length).Trim();
                        for (int i = 0; i < choices.Length; i++)
                        {
                            Elements.ChoiceController choiceController = Instantiate(dialogueManager.ChoicePrefab, dialogueManager.ChoiceContainer).GetComponent<Elements.ChoiceController>();
                            choiceController.SetInformation(dialogueManager, command, i, choices[i].Trim());
                        }
                    }
                    else if (Tools.StringExtensions.TextStartsWith(command, FLAG_COMMAND))
                    {
                        dialogueManager.Data.Dialogue.SetFlag(command, true);
                    }
                    else if (Tools.StringExtensions.TextStartsWith(command, SLEEP_COMMAND))
                    {
                        command = command.Substring(SLEEP_COMMAND.Length).Trim();
                        dialogueManager.Sleep(float.Parse(command));
                    }
                    else
                    {
                        dialogueManager.UI.ErrorMessages = "KEY SYS COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
                    }
                }
            }
        }
    }
}