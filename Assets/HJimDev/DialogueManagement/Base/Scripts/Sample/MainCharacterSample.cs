using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Base
    {
        namespace Sample
        {
            public class MainCharacterSample : Entities.MainCharacter
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

                public override void SetEnable(bool value)
                {
                    gameObject.SetActive(value);
                }
            }
        }
    }
}