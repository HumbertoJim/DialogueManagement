using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using ScheduleManagement.Schedulers;
using ScheduleManagement.Schedulables;


namespace DialogueManagement
{
    namespace Core
    {
        public class DialogueManager: MonoBehaviour
        {
            [Serializable]
            public class DataSet {
                [SerializeField] UserDataManager user;
                [SerializeField] DialogueData dialogue;
                [SerializeField] SettingDataManager settings;
                public UserDataManager User { get { return user; } }
                public DialogueData Dialogue { get { return dialogue; } }
                public SettingDataManager Settings { get { return settings; } }
            }
            [Serializable]
            public class KeyWords
            {
                [SerializeField] string command = "run";
                [SerializeField] string enter = "?n?";
                [SerializeField] string line_separator = "?;?";
                [SerializeField] string audio_fixer_separator = "::"; // Primero el nombre del audio, luego el texto. Ejemplo. audio532::Hola, como estas.
                [SerializeField] string ini_if = "if";
                [SerializeField] string end_if = "end_if";
                [SerializeField] string condition_separator = "&";
                [SerializeField] string replace_ini = "{"; // Para reemplazar una id por su string correspondiente
                [SerializeField] string replace_end = "}";

                public string Command { get { return command; } }
                public string Enter { get { return enter; } }
                public string LineSeparator { get { return line_separator; } }
                public string AudioFixerSeparator { get { return audio_fixer_separator; } }
                public string If { get { return ini_if; } }
                public string EndIf { get { return end_if; } }
                public string ConditionSeparator { get { return condition_separator; } }
                public string ReplaceIni { get { return replace_ini; } }
                public string ReplaceEnd { get { return replace_end; } }
            }

            [Header("Dependencies")]
            [SerializeField] TextManager textManager;
            [SerializeField] ApplicationHandler applicationHandler;
            [SerializeField] DialoguerCluster dialoguerCluster;
            [SerializeField] DataSet data;

            [Header("Modules")]
            [SerializeField] Module[] modules;
            [SerializeField] KeyWords keyWords;

            [Header("UI Elements")]
            [SerializeField] TMP_Text nameLabel;
            [SerializeField] TMP_Text textLabel;
            [SerializeField] Image imageLabel;
            [SerializeField] TMP_Text textImage;
            [SerializeField] TMP_Text errorMessages;
            [SerializeField] Image fadeLabel;

            [Header("UI Animators")]
            [SerializeField] Animator textLabelAnimator;
            bool showingTextLabel; // animating showing
            bool isTextLabelShown;

            [Header("Fade Settings")]
            [SerializeField] float fadeTime = 3f;

            [Header("Text Settings")]
            [SerializeField] float characterWrittingTime = 0.05f;
            [SerializeField] float sleepTimeAfterWriting = 0.5f;
            string textToWrite;

            [Header("Dialogue Settings")]
            [SerializeField] string playerCode = "Player";
            Dialogue currentDialogue;
            string currentDialoguer = "";

            [Header("Choices")]
            [SerializeField] Transform choiceContainer;
            [SerializeField] GameObject choicePrefab;

            bool executingCommand;

            public Scheduler Scheduler { get; private set; }

            public string ErrorMessages { set { errorMessages.text = value; } }
            public Transform ChoiceContainer { get { return choiceContainer; } }
            public GameObject ChoicePrefab { get { return choicePrefab; } }
            public bool IsChoicing { set; get; }
            public bool OnFadeIn { get; set; }
            public string InitialText { set; private get; }
            public Image ImageLabel { get { return imageLabel; } }
            public bool IsImageShown { set; get; }
            public bool IsPlaying { get { return currentDialogue != null; } }
            public ApplicationHandler ApplicationHandler { get { return applicationHandler; } }
            public DialoguerCluster DialoguerCluster { get { return dialoguerCluster; } }
            public DataSet Data { get { return data; } }

            protected void Awake()
            {
                Scheduler = new Scheduler();
                executingCommand = false;
            }

            private void Start()
            {
                Scheduler.Add(
                    "Sleep",
                    new AdditiveSchedulable(
                        () =>
                        {
                            // add later the stop loading animation
                        }
                    )
                );
                Scheduler.Add(
                    "FadeIn",
                    new Schedulable(
                        0.1f,
                        () =>
                        {
                            float alpha = fadeLabel.color.a + 1f / fadeTime * 0.1f;
                            alpha = alpha > 1 ? 1 : alpha;
                            fadeLabel.color = new Color(fadeLabel.color.r, fadeLabel.color.g, fadeLabel.color.b, alpha);
                            if (alpha < 1)
                            {
                                Scheduler.Start("FadeIn");
                            }
                        }
                    )
                );
                Scheduler.Add(
                    "FadeOut",
                    new Schedulable(
                        0.1f,
                        () =>
                        {
                            float alpha = fadeLabel.color.a - 1f / fadeTime * 0.1f;
                            alpha = alpha < 0 ? 0 : alpha;
                            fadeLabel.color = new Color(fadeLabel.color.r, fadeLabel.color.g, fadeLabel.color.b, alpha);
                            if (alpha > 0)
                            {
                                Scheduler.Start("FadeOut");
                            }
                        }
                    )
                );
                Scheduler.Add(
                    "WriteText",
                    new Schedulable(
                        characterWrittingTime,
                        () =>
                        {
                            if (textToWrite.Length > 0)
                            {
                                textLabel.text += textToWrite[0];
                                textToWrite = textToWrite[1..]; // PROBAR ESTO, QUE SI SE ADMITE CUANDO LA CADENA ES DE LONGITUD 1
                                Scheduler.Start("WriteText");
                            }
                            else
                            {
                                InitialText = "";
                                if (IsChoicing)
                                {
                                    choiceContainer.gameObject.SetActive(true);
                                    ApplicationHandler.SetEnable("Mouse", true);
                                }
                            }
                        }
                    )
                );
                Scheduler.Add(
                    "NotifyTextLabelIsShown",
                    new Schedulable(1f, () => { showingTextLabel = false; isTextLabelShown = true; Next(); })
                );
                Scheduler.Add(
                    "ShowTextLabel",
                    new Schedulable(1f, () => { textLabelAnimator.SetTrigger("Show"); Scheduler.Start("NotifyTextLabelIsShown"); })
                );

                ResetDefault();
            }

            void ResetDefault()
            {
                ApplicationHandler.SetEnable("Mouse", false);

                Scheduler.Cancel("Sleep");

                Scheduler.Cancel("WriteText");
                IsChoicing = false;
                foreach (Transform child in choiceContainer)
                {
                    Destroy(child.gameObject);
                }
                choiceContainer.gameObject.SetActive(false);

                currentDialoguer = "";

                InitialText = "";
                textToWrite = "";
            }

            public Dialogue InstantiateDialogue(string dialogueID, int startAt = 0)
            {
                string[] content = ReadDialogueContent(dialogueID);
                Dialogue dialogue = new(dialogueID, content, startAt);
                return dialogue;
            }

            string[] ReadDialogueContent(string dialogueID)
            {
                return textManager.GetText("Dialogues", dialogueID).Split(new string[] { keyWords.LineSeparator}, StringSplitOptions.None);
            }

            private void Update()
            {
                Scheduler.Process();
            }

            public void PlayDialogue(string dialogueID)
            {
                Dialogue dialogue = InstantiateDialogue(dialogueID);
                PlayDialogue(dialogue);
            }


            public void PlayDialogue(Dialogue dialogue)
            {
                // dialogue is valid, not executing a command, not Sleeping, not chocing, not showing an image,
                // not on fade in and textLabel is not animating (to show or hide)
                bool play = dialogue != null && (
                    currentDialogue == null || (!executingCommand && !Scheduler.IsActive("Sleep") && !IsChoicing && !IsImageShown && !OnFadeIn && !showingTextLabel)
                );
                if (play)
                {
                    ResetDefault();

                    currentDialogue = dialogue;
                    currentDialoguer = currentDialogue.CurrentDialoguer;

                    nameLabel.text = dialoguerCluster.GetDialoguerName(currentDialoguer);

                    // once a dialogue is called to play, animate labels to set as shown and when animation finish,
                    // automatically invoke the Next() function
                    ShowTextLabel();
                }
            }

            void ShowTextLabel()
            {
                showingTextLabel = true;
                if (isTextLabelShown)
                {
                    isTextLabelShown = false;
                    textLabelAnimator.SetTrigger("Hide");
                    Scheduler.Start("ShowTextLabel");
                }
                else
                {
                    textLabelAnimator.SetTrigger("Show");
                    Scheduler.Start("NotifyTextLabelIsShown");
                }
            }

            public void Next()
            {
                if (Scheduler.IsActive("Sleep"))
                {
                    return;
                }

                if (currentDialogue == null)
                {
                    return;
                }

                if (currentDialogue.IsEnded())
                {
                    HideDialogue();
                    return;
                }

                if (showingTextLabel)
                {
                    return;
                }

                if (Scheduler.IsActive("WriteText"))
                {
                    Scheduler.Cancel("WriteText");
                    textLabel.text += textToWrite;
                    InitialText = "";
                    if (IsChoicing)
                    {
                        choiceContainer.gameObject.SetActive(true);
                        ApplicationHandler.SetEnable("Mouse", true);
                    }
                    return;
                }

                if (IsChoicing)
                {
                    return;
                }

                if (currentDialoguer != playerCode)
                {
                    DialoguerCluster.StopTalking();
                }

                Dialogue.Line line = null;
                executingCommand = true;
                while (executingCommand)
                {
                    line = currentDialogue.Next();
                    if (line.NotNull)
                    {
                        executingCommand = ExecuteLine(line.Text);
                    }
                    else
                    {
                        executingCommand = false;
                    }
                }
                executingCommand = false;

                if (!line.NotNull) // (currentLine >= dialogue.Length)
                {
                    HideDialogue();
                    return;
                }
            }

            void HideDialogue()
            {
                if (currentDialogue != null)
                {
                    isTextLabelShown = false;
                    textLabelAnimator.SetTrigger("Hide");

                    ResetDefault();

                    string dialogueEnded = currentDialogue.ID;

                    currentDialogue = null;

                    ApplicationHandler.NotifyDialogueEnded(dialogueEnded);
                    DialoguerCluster.SetTalker(null);
                }
            }

            bool ExecuteLine(string line)
            {
                line = line.Trim();
                if (line == "")
                {
                    return true;
                }
                else if (line[^1] == ':')
                {
                    if (currentDialoguer != currentDialogue.CurrentDialoguer)
                    {
                        currentDialoguer = currentDialogue.CurrentDialoguer;
                        if (currentDialoguer == playerCode)
                        {
                            dialoguerCluster.SetTalker(null);
                            nameLabel.text = Data.User.Username;
                        }
                        else
                        {
                            dialoguerCluster.SetTalker(currentDialogue.CurrentDialoguer);
                            nameLabel.text = dialoguerCluster.GetDialoguerName();
                        }
                    }
                    return true;
                }
                else if (Tools.StringExtensions.TextStartsWith(line, keyWords.Command))
                {
                    line = line[keyWords.Command.Length..].Trim();
                    foreach (Module module in modules)
                    {
                        if (Tools.StringExtensions.TextStartsWith(line, module.ModuleName))
                        {
                            line = line[module.ModuleName.Length..].Trim();
                            module.HandleCommand(line);
                            return true;
                        }
                    }
                    ErrorMessages = "MODULE NAME ERROR: invalid MODULE NAME in \"" + line + "\"";
                    return true;
                }
                else if (Tools.StringExtensions.TextStartsWith(line, keyWords.If))
                {
                    ApplyIf(line[keyWords.If.Length..].Trim());
                    return true;
                }
                else if (Tools.StringExtensions.TextStartsWith(line, keyWords.EndIf))
                {
                    return true;
                }

                string text;
                string audioName;
                if (line.Contains(keyWords.AudioFixerSeparator))
                {
                    string[] audioAndText = line.Split(new string[] { keyWords.AudioFixerSeparator }, StringSplitOptions.None);
                    text = audioAndText[1].Trim().Replace(keyWords.Enter, "\n");
                    audioName = audioAndText[0].Trim();
                }
                else
                {
                    text = line.Replace(keyWords.Enter, "\n");
                    audioName = null;
                }

                if (text.Contains(keyWords.ReplaceIni))
                {
                    text = ReplaceString(text);
                }

                if (currentDialoguer != playerCode)
                {
                    dialoguerCluster.Talk(currentDialoguer, currentDialogue.ID, audioName: audioName, text: text);
                }

                Sleep(sleepTimeAfterWriting);
                WriteText(text);

                return false;
            }

            void WriteText(string text)
            {
                Scheduler.Cancel("WriteText");
                textToWrite = text;
                textLabel.text = InitialText;
                Scheduler.Start("WriteText");
            }

            public void FadeIn()
            {
                if (!OnFadeIn)
                {
                    OnFadeIn = true;
                    Scheduler.Start("FadeIn");
                    ApplicationHandler.SetEnable("CameraRotation", false);
                    ApplicationHandler.SetEnable("Interactions", false);
                }
            }

            public void FadeOut()
            {
                if (OnFadeIn)
                {
                    OnFadeIn = false;
                    Scheduler.Start("FadeOut");
                    if (!IsImageShown && !IsChoicing)
                    {
                        ApplicationHandler.SetEnable("CameraRotation", true);
                        ApplicationHandler.SetEnable("Interactions", true);
                    }
                }
            }

            public void SetFadeIn(bool value)
            {
                Scheduler.Cancel("FadeIn");
                Scheduler.Cancel("FadeOut");
                OnFadeIn = value;
                if (value)
                {
                    fadeLabel.color = new Color(fadeLabel.color.r, fadeLabel.color.g, fadeLabel.color.b, 1);
                    ApplicationHandler.SetEnable("CameraRotation", false);
                    ApplicationHandler.SetEnable("Interactions", false);
                }
                else
                {
                    fadeLabel.color = new Color(fadeLabel.color.r, fadeLabel.color.g, fadeLabel.color.b, 0);
                    if (!IsImageShown && !IsChoicing)
                    {
                        ApplicationHandler.SetEnable("CameraRotation", true);
                        ApplicationHandler.SetEnable("Interactions", true);
                    }
                }
            }

            public void SelectChoice(string choiceID, int choice)
            {
                if (IsChoicing)
                {
                    IsChoicing = false;

                    Data.Dialogue.SetChoice(choiceID, choice);

                    foreach (Transform child in choiceContainer)
                    {
                        Destroy(child.gameObject);
                    }
                    choiceContainer.gameObject.SetActive(false);

                    if (!IsImageShown && !OnFadeIn)
                    {
                        ApplicationHandler.SetEnable("CameraRotation", true);
                        ApplicationHandler.SetEnable("Interactions", true);
                    }
                    ApplicationHandler.SetEnable("Mouse", false);

                    Next();
                }
            }


            // -------------------- Past Methos --------------------

            void ApplyIf(string line)
            {
                string[] sentences = line.Split(new string[] { keyWords.ConditionSeparator }, StringSplitOptions.None);
                bool flag = true;
                foreach (string sentence in sentences)
                {
                    flag = flag && EvaluateConditional(sentence);
                }
                if (!flag)
                {
                    Dialogue.Line ignore_line = currentDialogue.Next(ignoreDialoguer: true);
                    int nEndIf = 0;
                    while (ignore_line.NotNull && !(nEndIf == 0 && Tools.StringExtensions.TextStartsWith(ignore_line.Text, keyWords.EndIf)))
                    {
                        if (Tools.StringExtensions.TextStartsWith(ignore_line.Text, keyWords.If))
                            nEndIf++;
                        if (Tools.StringExtensions.TextStartsWith(ignore_line.Text, keyWords.EndIf))
                            nEndIf--;
                        ignore_line = currentDialogue.Next(ignoreDialoguer: true);
                    }
                }
            }

            bool EvaluateConditional(string sentence)
            {
                bool result;
                string[] sentenceStruct = Regex.Replace(sentence.Trim(), @"\s+", " ").Split(' ');
                sentenceStruct[0] = sentenceStruct[0].Trim(); // variable type
                sentenceStruct[1] = sentenceStruct[1].Trim(); // dictionaryIndex
                sentenceStruct[2] = sentenceStruct[2].Trim(); // conditional
                sentenceStruct[3] = sentenceStruct[3].Trim(); // value
                                                              // int dictionaryIndex condition value // aplica todas las comparaciones como operacion, condicion = {<, >, ==, !=, <=, >=}
                                                              // bool dictionaryIndex conditional value // condicion = {==, !=}
                                                              // choice dictionaryIndex condition value // condicion = {<, >, ==, !=, <=, >=}
                                                              // string dictionaryIndex condition value // condicion = {==, !=}
                                                              // relationship npcID condition value // condicion = {<, >, <=, >=}
                                                              // good_relationship npcID condition value // condicion = {<, >, <=, >=}
                                                              // bad_relationship npcID condition value // condicion = {<, >, <=, >=}
                                                              // interaction_count npcID condition value // condicion = {<, >, <=, >=}
                if (sentenceStruct[0] == "choice")
                {
                    if (sentenceStruct[2] == ">")
                    {
                        result = Data.Dialogue.GetChoice(sentenceStruct[1]) > Convert.ToInt16(sentenceStruct[3]);
                    }
                    else if (sentenceStruct[2] == ">=")
                    {
                        result = Data.Dialogue.GetChoice(sentenceStruct[1]) >= Convert.ToInt16(sentenceStruct[3]);
                    }
                    else if (sentenceStruct[2] == "<")
                    {
                        result = Data.Dialogue.GetChoice(sentenceStruct[1]) < Convert.ToInt16(sentenceStruct[3]);
                    }
                    else if (sentenceStruct[2] == "<=")
                    {
                        result = Data.Dialogue.GetChoice(sentenceStruct[1]) <= Convert.ToInt16(sentenceStruct[3]);
                    }
                    else if (sentenceStruct[2] == "==")
                    {
                        result = Data.Dialogue.GetChoice(sentenceStruct[1]) == Convert.ToInt16(sentenceStruct[3]);
                    }
                    else if (sentenceStruct[2] == "!=")
                    {
                        result = Data.Dialogue.GetChoice(sentenceStruct[1]) != Convert.ToInt16(sentenceStruct[3]);
                    }
                    else
                    {
                        Debug.Log("EvaluateConditional ERROR: invalid choice comparison in \"" + sentence + "\"");
                        result = false;
                    }
                }
                else if (sentenceStruct[0] == "flag")
                {
                    if (sentenceStruct[2] == "==")
                    {
                        if (sentenceStruct[3].ToLower() == "true")
                        {
                            result = Data.Dialogue.GetFlag(sentenceStruct[1]) == true;
                        }
                        else
                        {
                            result = Data.Dialogue.GetFlag(sentenceStruct[1]) == false;
                        }
                    }
                    else if (sentenceStruct[2] == "!=")
                    {
                        if (sentenceStruct[3].ToLower() == "true")
                        {
                            result = Data.Dialogue.GetFlag(sentenceStruct[1]) != true;
                        }
                        else
                        {
                            result = Data.Dialogue.GetFlag(sentenceStruct[1]) != false;
                        }
                    }
                    else
                    {
                        ErrorMessages = "EvaluateConditional ERROR: invalid flag comparison in \"" + sentence + "\"";
                        result = false;
                    }
                }
                else
                {
                    Debug.Log("EvaluateConditional ERROR: invalid type in \"" + sentence + "\"");
                    result = false;
                }
                return result;
            }

            public void Sleep(float time)
            {
                if (time > 0)
                {
                    Scheduler.Start("Sleep", time);
                    // SleepLoading.SetActive(true);
                }
            }

            string ReplaceString(string originalText)
            {
                string newText = originalText;
                int n_ini = keyWords.ReplaceIni.Length;
                int n_fin = keyWords.ReplaceEnd.Length;
                string stringID = "";
                for (int i = 0; i < (originalText.Length - n_ini - n_fin); i++)
                {
                    if (originalText.Substring(i, n_ini) == keyWords.ReplaceIni)
                    {
                        i += n_ini;
                        for (; i < (originalText.Length - n_fin + 1); i++)
                        {
                            if (originalText[i] == ' ' || originalText[i] == '\n')
                            {
                                stringID = "";
                                break;
                            }
                            if (originalText.Substring(i, n_fin) == keyWords.ReplaceEnd)
                            {
                                if (stringID == playerCode)
                                {
                                    newText = newText.Replace(keyWords.ReplaceIni + stringID + keyWords.ReplaceEnd, "\"" + Data.User.Username + "\"");
                                }
                                else
                                {
                                    newText = newText.Replace(keyWords.ReplaceIni + stringID + keyWords.ReplaceEnd, "\"unknown_key\"");
                                }
                                i += n_fin - 1;
                                stringID = "";
                                break;
                            }
                            stringID += originalText[i];
                        }
                    }
                }
                return newText;
            }
        }
    }
}