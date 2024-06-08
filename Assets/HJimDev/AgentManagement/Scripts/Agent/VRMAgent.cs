using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScheduleManagement.Schedulers;
using ScheduleManagement.Schedulables;

using AgentManagement.Actions;
using VRoidExtensions.Models;

namespace AgentManagement
{
    namespace Agents
    {
        public class VRMAgent : HumanoidAgent
        {
            [Header("Talk")]
            [SerializeField] float defaultTimePerVowel = 0.15f;
            string textAsVowelSet;
            float timePerVowel;
            Coroutine animMouthCoroutine;

            [Header("Blink")]
            [SerializeField] float minBlinkTime = 2f;
            [SerializeField] float maxBlinkTime = 5f;

            [Header("Loot At")]
            [SerializeField] Transform defaultLookAt;

            public new VRMModel Model { get { return (VRMModel)base.Model; } }

            protected override void Awake()
            {
                base.Awake();

                Scheduler.Add("Blink", new RandomSchedulable(minBlinkTime, maxBlinkTime, () => Model.Animator.SetTrigger("Blink")));
                Scheduler.Start("Blink");
                Scheduler.Add("AnimMouth",
                    new Schedulable(
                        () =>
                        {
                            if (textAsVowelSet.Length > 0)
                            {
                                Model.Animator.SetTrigger("Vowel" + textAsVowelSet[0]);
                                textAsVowelSet = textAsVowelSet.Substring(1);
                                Scheduler.Start("AnimMouth", timePerVowel);
                            }
                            else Model.Audio.Pause();
                        }
                    )
                );

                Model.LookAt(defaultLookAt);
            }

            /* Talk */
            public override void PlayAudio(AudioClip audio)
            {
                Model.Audio.loop = false;
                base.PlayAudio(audio);
            }

            public override void StopTalking()
            {
                if (animMouthCoroutine != null)
                {
                    StopCoroutine(animMouthCoroutine);
                }
                base.StopTalking();
            }

            public void AnimateMounth(AudioClip audio, string text)
            {
                Scheduler.Cancel("AnimMouth");

                textAsVowelSet = Tools.StringExtension.KeepVowals(text);
                textAsVowelSet = textAsVowelSet == "" ? "AEIUO" : textAsVowelSet;

                if (audio)
                {
                    Model.Audio.loop = false;
                    timePerVowel = audio.length > 0 ? audio.length / textAsVowelSet.Length : timePerVowel;
                }
                else
                {
                    Model.Audio.loop = true;
                    timePerVowel = defaultTimePerVowel;
                }

                Scheduler.Start("AnimMouth", timePerVowel);
            }

            /* Facial Expressions */
            public void FacialExpression(string expression)
            {

            }

            /* Execute Action */
            public override void ExecuteAction(AgentAction action)
            {
                if (animMouthCoroutine != null) StopCoroutine(animMouthCoroutine);
                base.ExecuteAction(action);
            }

            protected override void ExecuteAction()
            {
                base.ExecuteAction();

                if (CurrentAction.facialExpression != "")
                {
                    FacialExpression(CurrentAction.facialExpression);
                }

                if (CurrentAction.audioText != "")
                {
                    AnimateMounth(CurrentAction.audio, CurrentAction.audioText);
                }
            }
        }
    }
}