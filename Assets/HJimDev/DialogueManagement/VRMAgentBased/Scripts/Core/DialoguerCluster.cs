using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        namespace Core
        {
            public class DialoguerCluster : Base.Core.DialoguerCluster
            {
                public new DialogueManager DialogueManager => (DialogueManager)base.DialogueManager;
                public new Entities.MainCharacter MainCharacter => (Entities.MainCharacter)base.MainCharacter;
                public new Entities.Storyteller Storyteller => (Entities.Storyteller)base.Storyteller;
                public new Entities.Dialoguer CurrentDialoguer => (Entities.Dialoguer)base.CurrentDialoguer;

                public new Entities.Dialoguer GetDialoguer(string code)
                {
                    return (Entities.Dialoguer)base.GetDialoguer(code);
                }
            }
        }
    }
}