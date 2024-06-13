using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Base
    {
        namespace Core
        {
            public class DialoguerCluster : MonoBehaviour
            {
                [Header("Dependencies")]
                [SerializeField] DialogueManager dialogueManager;

                [Header("Dialoguers")]
                [SerializeField] Entities.MainCharacter mainCharacter;
                [SerializeField] Entities.Storyteller storyteller;
                [SerializeField] Entities.Dialoguer[] dialoguers;

                public Entities.MainCharacter MainCharacter { get { return mainCharacter; } }
                public Entities.Storyteller Storyteller { get { return storyteller; } }
                public Entities.Dialoguer CurrentDialoguer { get; private set; }
                public string CurrentTalker { get; private set; }

                Dictionary<string, Entities.Dialoguer> _dialoguers;
                Dictionary<string, string> names;


                protected virtual void Awake()
                {
                    names = new Dictionary<string, string>();
                    _dialoguers = new Dictionary<string, Entities.Dialoguer>();
                }

                protected virtual void Start()
                {
                    names.Add(MainCharacter.Code, dialogueManager.Data.User.Username);
                    names.Add(Storyteller.Code, dialogueManager.TextManager.GetText("Characters", Storyteller.Code));
                    foreach (Entities.Dialoguer dialoguer in dialoguers)
                    {
                        names.Add(dialoguer.Code, dialogueManager.TextManager.GetText("Characters", dialoguer.Code));
                        _dialoguers.Add(dialoguer.Code, dialoguer);
                    }
                }

                public virtual void SetEnable(string code, bool value)
                {
                    if (code == MainCharacter.Code)
                    {
                        MainCharacter.SetEnable(value);
                    }
                    else if (code == Storyteller.Code)
                    {
                        Storyteller.SetEnable(value);
                    }
                    else if (code != null && _dialoguers.TryGetValue(code, out Entities.Dialoguer dialoguer))
                    {
                        dialoguer.SetEnable(value);
                    }
                }

                public virtual void SetTalker(string code)
                {
                    if (CurrentTalker != code)
                    {
                        StopTalking();
                        CurrentTalker = code;
                        if (code == null || code == MainCharacter.Code || code == Storyteller.Code)
                        {
                            CurrentDialoguer = null;
                        }
                        else
                        {
                            CurrentDialoguer = _dialoguers.GetValueOrDefault(code, null);
                        }
                    }
                }

                public void Talk(string dialogueID, string audioName = null, string text = null)
                {
                    Talk(CurrentTalker, dialogueID, audioName, text);
                }

                public virtual void Talk(string code, string dialogueID, string audioName = null, string text = null)
                {
                    AudioClip audio = audioName == null ? null : Resources.Load<AudioClip>("Voices/" + dialogueID + "/" + code + "/" + audioName);
                    if (code == MainCharacter.Code)
                    {
                        MainCharacter.Talk(audio, text);
                    }
                    else if (code == Storyteller.Code)
                    {
                        Storyteller.Talk(audio, text);
                    }
                    else if (code != null && _dialoguers.TryGetValue(code, out Entities.Dialoguer dialoguer))
                    {
                        dialoguer.Talk(audio, text);
                    }
                }

                public void StopTalking()
                {
                    StopTalking(CurrentTalker);
                }

                public void StopTalking(string code)
                {
                    if (code == MainCharacter.Code)
                    {
                        MainCharacter.StopTalking();
                    }
                    else if (code == Storyteller.Code)
                    {
                        Storyteller.StopTalking();
                    }
                    else if (code != null && _dialoguers.TryGetValue(code, out Entities.Dialoguer dialoguer))
                    {
                        dialoguer.StopTalking();
                    }
                }

                public string GetDialoguerName()
                {
                    return GetDialoguerName(CurrentTalker);
                }

                public string GetDialoguerName(string code)
                {
                    if (names.TryGetValue(code, out string name))
                    {
                        return name;
                    }
                    return "";
                }

                public Entities.Dialoguer GetDialoguer(string code)
                {
                    return _dialoguers.GetValueOrDefault(code, null);
                }
            }
        }
    }
}