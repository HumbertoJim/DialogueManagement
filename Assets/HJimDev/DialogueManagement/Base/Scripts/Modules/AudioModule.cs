using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;


namespace DialogueManagement
{
    namespace Base
    {
        namespace Modules
        {
            public class AudioModule : Module
            {
                public override string ModuleName => "audio";

                const string MUSIC_COMMAND = "music";
                const string SOUND_COMMAND = "sound";

                public override void HandleCommand(string command)
                {
                    if (Tools.StringExtensions.TextStartsWith(command, MUSIC_COMMAND))
                    {
                        command = command[MUSIC_COMMAND.Length..].Trim();
                        AudioManager.DefaultManager.PlaySecondaryMusic(command);
                    }
                    else if (Tools.StringExtensions.TextStartsWith(command, SOUND_COMMAND))
                    {
                        command = command[SOUND_COMMAND.Length..].Trim();
                        AudioManager.DefaultManager.PlaySound(command);
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