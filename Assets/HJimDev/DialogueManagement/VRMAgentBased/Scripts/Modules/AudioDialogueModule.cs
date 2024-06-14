using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        namespace Modules
        {
            public class AudioDialogueModule : Base.Modules.Module
            {
                public override string ModuleName => "audio";

                const string MUSIC_COMMAND = "music";

                public override void HandleCommand(string command)
                {
                    if (Tools.StringExtensions.TextStartsWith(command, MUSIC_COMMAND))
                    {
                        command = command[MUSIC_COMMAND.Length..].Trim();
                        Debug.Log("Playig: " + command);
                    }
                    else
                    {
                        dialogueManager.UI.ErrorMessages = "KEY AUDIO COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
                    }
                }
            }
        }
    }
}