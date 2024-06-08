using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Core
    {
        public class DialoguerCluster: MonoBehaviour
        {
            [Header("Dependencies")]
            [SerializeField] TextManager textManager;

            [Header("Dialoguers")]
            [SerializeField] Dialoguer mainCharacter;
            [SerializeField] Dialoguer[] dialoguers;

            public Dialoguer MainCharacter { get { return mainCharacter; } }

            Dictionary<string, Dialoguer> Dialoguers { get; set; }

            Dictionary<string, string> names;

            string currentDialoguer;

            protected virtual void Awake()
            {
                Dialoguers = new Dictionary<string, Dialoguer>();
                names = new Dictionary<string, string>();
                foreach (Dialoguer dialoguer in dialoguers)
                {
                    Dialoguers.Add(dialoguer.Code, dialoguer);
                    names.Add(dialoguer.Code, textManager.GetText("Name", dialoguer.Code));
                }
                names.Add("Storyteller", textManager.GetText("Name", "Storyteller"));
            }

            public virtual void SetEnable(string code, bool value)
            {
                if (Dialoguers.TryGetValue(code, out Dialoguer dialoguer))
                {
                    dialoguer.gameObject.SetActive(value);
                }
            }

            public virtual void SetTalker(string code)
            {
                if (currentDialoguer != code)
                {
                    StopTalking();
                    currentDialoguer = code;
                }
            }

            public void Talk(string dialogueID, string audioName = null, string text = null)
            {
                Talk(currentDialoguer, dialogueID, audioName, text);
            }

            public virtual void Talk(string code, string dialogueID, string audioName = null, string text = null)
            {
                if (Dialoguers.TryGetValue(code, out Dialoguer dialoguer))
                {
                    AudioClip audio = audioName == null ? null : Resources.Load<AudioClip>("Voices/" + dialogueID + "/" + code + "/" + audioName);
                    dialoguer.Talk(audio, text);
                }
            }

            public void StopTalking()
            {
                StopTalking(currentDialoguer);
            }

            public void StopTalking(string code)
            {
                if (Dialoguers.TryGetValue(code, out Dialoguer dialoguer))
                {
                    dialoguer.StopTalking();
                }
            }

            public string GetDialoguerName()
            {
                return GetDialoguerName(currentDialoguer);
            }

            public string GetDialoguerName(string code)
            {
                if (names.TryGetValue(code, out string name))
                {
                    return name;
                }
                return "no_name";
            }

            public Dialoguer GetDialoguer(string code)
            {
                return Dialoguers[code];
            }
        }
    }
}