using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace Base
    {
        namespace Modules
        {
            public class CharacterModule : Module
            {
                public override string ModuleName => "character";

                const string RELATIONSHIP_COMMAND = "relationship";
                const string RELATIONSHIP_COMMAND_DECREASE_OPTION = "-s";
                const string REGISTER_INTERACTION_COMMAND = "interact";

                public override void HandleCommand(string command)
                {
                    if (Tools.StringExtensions.TextStartsWith(command, RELATIONSHIP_COMMAND))
                    {
                        command = command[RELATIONSHIP_COMMAND.Length..].Trim();
                        if (Tools.StringExtensions.TextStartsWith(command, RELATIONSHIP_COMMAND_DECREASE_OPTION))
                        {
                            command = command[RELATIONSHIP_COMMAND_DECREASE_OPTION.Length..].Trim();
                            string[] sentence = command.Split(' ');
                            if (sentence.Length > 1)
                            {
                                dialogueManager.Data.Dialogue.DecreaseRelationship(sentence[0], int.Parse(command[sentence[0].Length..].Trim()));
                            }
                        }
                        else
                        {
                            string[] sentence = command.Split(' ');
                            if (sentence.Length > 1)
                            {
                                dialogueManager.Data.Dialogue.ImproveRelationship(sentence[0], int.Parse(command[sentence[0].Length..].Trim()));
                            }
                        }
                    }
                    else if (Tools.StringExtensions.TextStartsWith(command, REGISTER_INTERACTION_COMMAND))
                    {
                        command = command[REGISTER_INTERACTION_COMMAND.Length..].Trim();
                        dialogueManager.Data.Dialogue.RegisterInteraction(command);
                    }
                    else
                    {
                        dialogueManager.UI.ErrorMessages = "KEY CHARACTER COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
                    }
                }
            }
        }
    }
}