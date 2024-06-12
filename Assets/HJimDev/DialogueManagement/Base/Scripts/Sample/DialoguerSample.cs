using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Core
    {
        namespace Sample
        {
            public class DialoguerSample : Core.Dialoguer
            {
                [SerializeField] Animator anim;
                public override void Talk(AudioClip audio = null, string text = null)
                {
                    anim.SetTrigger("Talk");
                }

                public override void StopTalking()
                {
                    anim.SetTrigger("Idle");
                }
            }
        }
    }
}