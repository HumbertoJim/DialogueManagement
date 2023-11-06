using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    public class Dialoguer : MonoBehaviour
    {
        [SerializeField] string code;
        public string Code { get { return code; } }

        public virtual void Talk(AudioClip audio=null, string text=null)
        {

        }

        public virtual void StopTalking()
        {

        }


    }

    namespace Core
    {
        public class Module : MonoBehaviour
        {
            public virtual string ModuleName { get { throw new NotImplementedException(); } }

            protected DialogueManager dialogueManager;

            public void SetInformation(DialogueManager dialogueManager)
            {
                this.dialogueManager = dialogueManager;
            }

            public virtual void HandleCommand(string command)
            {
                throw new NotImplementedException();
            }
        }

        public class Dialogue
        {
            public string ID { get; }

            public int CurrentLine { private set; get; }

            public string CurrentDialoguer { private set; get; }

            readonly string[] content;

            public int Length
            {
                get
                {
                    if (content == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return content.Length;
                    }
                }
            }

            public Dialogue(string dialogueID, string[] content, int startAt = 0)
            {
                ID = dialogueID;
                CurrentLine = startAt;
                CurrentDialoguer = "";
                this.content = content;
            }

            public Line Next(bool ignoreDialoguer = false)
            {
                if (IsEnded())
                {
                    return new Line();
                }
                else
                {
                    string line = content[CurrentLine].Trim();
                    if (!ignoreDialoguer)
                    {
                        if (line != "" && line[line.Length - 1] == ':')
                        {
                            CurrentDialoguer = line.Substring(0, line.Length - 1).Trim();
                        }
                    }
                    CurrentLine++;
                    return new Line(line);
                }
            }

            public bool IsEnded()
            {
                if (content != null && CurrentLine < content.Length)
                {
                    return false;
                }
                return true;
            }

            public class Line
            {
                public bool NotNull { get; }
                public string Text { get; }

                public Line(string text = null)
                {
                    if (text == null)
                    {
                        NotNull = false;
                    }
                    else
                    {
                        NotNull = true;
                        Text = text.Trim();
                    }
                }
            }
        }
    }

    namespace Handlers
    {
        public class ApplicationHandler: MonoBehaviour
        {
            public virtual void SetEnable(string element, bool value) { }

            public virtual void HandleEvent(string eventID, string parameters = "") { }

            public virtual void NotifyDialogueEnded(string dialogueID) { }
        }

        public class DialoguerHandler : MonoBehaviour
        {
            [Header("Dependencies")]
            [SerializeField] TextManager textManager;

            [Header("Dialoguers")]
            [SerializeField] Dialoguer[] dialoguers;

            public Dictionary<string, Dialoguer> Dialoguers { get; private set; }

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
                }
                currentDialoguer = code;
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
        }
    }

    namespace Tools
    {
        public class StringExtension
        {
            public static string KeepVowals(string text)
            {
                text = ReplaceForeingVowals(text);
                string vowels = "AEIOU";
                string text_vowels = "";
                text = text.ToUpper();
                foreach (char c in text)
                {
                    if (vowels.Contains(c.ToString()))
                    {
                        text_vowels += c;
                    }
                }
                return text_vowels;
            }

            public static string ReplaceForeingVowals(string text)
            {
                text = text.Replace('\u0430', 'a');
                text = text.Replace('\u0435', 'e');
                text = text.Replace('\u0454', 'e');
                text = text.Replace('\u0438', 'u');
                text = text.Replace('\u0456', 'i');
                text = text.Replace('\u0457', 'i');
                text = text.Replace('\u043E', 'o');
                text = text.Replace('\u0443', 'i');
                text = text.Replace('\u044E', 'u');
                text = text.Replace('\u044F', 'a');
                return text;
            }

            public static bool TextStartsWith(string text, string start)
            {
                if (text.Length < start.Length)
                {
                    return false;
                }
                for (int i = 0; i < start.Length; i++)
                {
                    if (text[i] != start[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}