using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using DataManagement.Managers;
using ScheduleManagement.Schedulers;
using ScheduleManagement.Schedulables;

using DialogueManagement.Core;
using DialogueManagement.Handlers;
using DialogueManagement.Tools;
using System.Text.RegularExpressions;

public class DialogueManager : DictionaryCollectionManager
{
    [Header("Dependencies")]
    [SerializeField] TextManager textManager;
    [SerializeField] UserDataManager userData;
    [SerializeField] ApplicationHandler applicationHandler;
    [SerializeField] DialoguerHandler dialoguerHandler;
    [SerializeField] SettingDataManager settingData;

    [Header("Modules")]
    [SerializeField] Module[] modules;
    const string KEY_COMMAND = "run";
    const string KEY_ENTER = "?n?";
    const string KEY_LINE_SEPARATOR = "?;?";
    const string KEY_AUDIO_FIXER_SEPARATOR = "::"; // Primero el nombre del audio, luego el texto. Ejemplo. audio532::Hola, como estas.
    const string KEY_IF = "if";
    const string KEY_END_IF = "end_if";
    const string KEY_CONDITION_SEPARATOR = "&";
    const string KEY_REPLACE_INI = "{"; // Para reemplazar una id por su string correspondiente
    const string KEY_REPLACE_FIN = "}";

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
    Coroutine showingTextLabelCoroutine;

    [Header("Fade Settings")]
    [SerializeField] float fadeTime = 3f;

    [Header("Text Settings")]
    [SerializeField] float characterWrittingTime = 0.05f;
    [SerializeField] float sleepTimeAfterWriting = 0.5f;
    string textToWrite;

    [Header("Scene Settings")]
    Dialogue currentDialogue;
    
    string currentDialoguer = "";

    [Header("Choices")]
    [SerializeField] Transform choiceContainer;
    [SerializeField] GameObject choicePrefab;

    bool executingCommand;

    public Scheduler Scheduler { get; private set; }

    public string ErrorMessages { set { errorMessages.text = value; } }
    public Dialogue CurrentDialogue { get; }
    public Transform ChoiceContainer { get; }
    public GameObject ChoicePrefab { get; }
    public bool IsChoicing { set; get; }
    public bool OnFadeIn { get; set; }
    public string InitialText { set; private get; }
    public Image ImageLabel { get { return imageLabel; } }
    public bool IsImageShown { set; get; }
    public ApplicationHandler ApplicationHandler { get { return applicationHandler; } }
    public DialoguerHandler DialoguerHandler { get { return dialoguerHandler; } }
    public SettingDataManager SettingData { get { return settingData; } }

    protected override void Awake()
    {
        Awake("Dialogue");
        Scheduler = new Scheduler();
        executingCommand = false;
    }

    private void Start()
    {
        ResetDefault();

        Scheduler.Add(
            "Sleep",
            new AdditiveSchedulable(
                () => {
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
                    if(textToWrite.Length > 0)
                    {
                        textLabel.text += textToWrite[0];
                        textToWrite = textToWrite[1..]; // PROBAR ESTO, QUE SI SE ADMITE CUANDO LA CADENA ES DE LONGITUD 1
                        if (IsChoicing)
                        {
                            choiceContainer.gameObject.SetActive(true);
                            ApplicationHandler.SetEnable("Mouse", true);
                        }
                    }
                    else
                    {
                        InitialText = "";
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
    }

    void ResetDefault()
    {
        ApplicationHandler.SetEnable("Mouse", false);

        Scheduler.Cancel("Sleep");

        Scheduler.Cancel("WriteText");
        IsChoicing= false;
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
        Dialogue dialogue = new Dialogue(dialogueID, content, startAt);
        return dialogue;
    }

    string[] ReadDialogueContent(string dialogueID)
    {
        return textManager.GetText("Dialogue", dialogueID).Split(new string[] { KEY_LINE_SEPARATOR }, StringSplitOptions.None);
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
        if (dialogue != null && (currentDialogue == null ||
            (!executingCommand && !Scheduler.IsActive("Sleep") && !IsChoicing && !IsImageShown && !OnFadeIn && !showingTextLabel)
            ))
        {
            Debug.Log("Dialogue: " + dialogue.ID);

            ResetDefault();

            currentDialogue = dialogue;
            currentDialoguer = currentDialogue.CurrentDialoguer;

            nameLabel.text = dialoguerHandler.GetDialoguerName(currentDialoguer);

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

        if (currentDialoguer != "Player")
        {
            DialoguerHandler.StopTalking();
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
            DialoguerHandler.SetTalker(null);
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
                if(currentDialoguer == "Player")
                {
                    dialoguerHandler.SetTalker(null);
                    nameLabel.text = userData.Username;
                }
                else
                {
                    dialoguerHandler.SetTalker(currentDialogue.CurrentDialoguer);
                    nameLabel.text = dialoguerHandler.GetDialoguerName();
                }
            }
            return true;
        }
        else if (StringExtension.TextStartsWith(line, KEY_COMMAND))
        {
            line = line[KEY_COMMAND.Length..].Trim();
            foreach(Module module in modules)
            {
                if(StringExtension.TextStartsWith(line, module.ModuleName))
                {
                    line = line[module.ModuleName.Length..].Trim();
                    module.HandleCommand(line);
                    return true;
                }
            }
            ErrorMessages = "MODULE NAME ERROR: invalid MODULE NAME in \"" + line + "\"";
            return true;
        }
        else if (StringExtension.TextStartsWith(line, KEY_IF))
        {
            ApplyIf(line[KEY_IF.Length..].Trim());
            return true;
        }
        else if (StringExtension.TextStartsWith(line, KEY_END_IF))
        {
            return true;
        }

        string text;
        string audioName;
        if (line.Contains(KEY_AUDIO_FIXER_SEPARATOR))
        {
            string[] audioAndText = line.Split(new string[] { KEY_AUDIO_FIXER_SEPARATOR }, StringSplitOptions.None);
            text = audioAndText[1].Trim().Replace(KEY_ENTER, "\n");
            audioName = audioAndText[0].Trim();
        }
        else
        {
            text = line.Replace(KEY_ENTER, "\n");
            audioName = null;
        }

        if (text.Contains(KEY_REPLACE_INI))
        {
            text = ReplaceString(text);
        }

        if (currentDialoguer != "Player")
        {
            dialoguerHandler.Talk(currentDialoguer, currentDialogue.ID, audioName: audioName, text: text);
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
            IsChoicing= false;

            SetChoice(choiceID, choice);

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
        string[] sentences = line.Split(new string[] { KEY_CONDITION_SEPARATOR }, StringSplitOptions.None);
        bool flag = true;
        foreach (string sentence in sentences)
        {
            flag = flag && EvaluateConditional(sentence);
        }
        if (!flag)
        {
            Dialogue.Line ignore_line = currentDialogue.Next(ignoreDialoguer: true);
            int nEndIf = 0;
            while (ignore_line.NotNull && !(nEndIf == 0 && StringExtension.TextStartsWith(ignore_line.Text, KEY_END_IF)))
            {
                if (StringExtension.TextStartsWith(ignore_line.Text, KEY_IF))
                    nEndIf++;
                if (StringExtension.TextStartsWith(ignore_line.Text, KEY_END_IF))
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
                result = GetChoice(sentenceStruct[1]) > Convert.ToInt16(sentenceStruct[3]);
            }
            else if (sentenceStruct[2] == ">=")
            {
                result = GetChoice(sentenceStruct[1]) >= Convert.ToInt16(sentenceStruct[3]);
            }
            else if (sentenceStruct[2] == "<")
            {
                result = GetChoice(sentenceStruct[1]) < Convert.ToInt16(sentenceStruct[3]);
            }
            else if (sentenceStruct[2] == "<=")
            {
                result = GetChoice(sentenceStruct[1]) <= Convert.ToInt16(sentenceStruct[3]);
            }
            else if (sentenceStruct[2] == "==")
            {
                result = GetChoice(sentenceStruct[1]) == Convert.ToInt16(sentenceStruct[3]);
            }
            else if (sentenceStruct[2] == "!=")
            {
                result = GetChoice(sentenceStruct[1]) != Convert.ToInt16(sentenceStruct[3]);
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
                    result = GetFlag(sentenceStruct[1]) == true;
                }
                else
                {
                    result = GetFlag(sentenceStruct[1]) == false;
                }
            }
            else if (sentenceStruct[2] == "!=")
            {
                if (sentenceStruct[3].ToLower() == "true")
                {
                    result = GetFlag(sentenceStruct[1]) != true;
                }
                else
                {
                    result = GetFlag(sentenceStruct[1]) != false;
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
        int n_ini = KEY_REPLACE_INI.Length;
        int n_fin = KEY_REPLACE_FIN.Length;
        string stringID = "";
        for (int i = 0; i < (originalText.Length - n_ini - n_fin); i++)
        {
            if (originalText.Substring(i, n_ini) == KEY_REPLACE_INI)
            {
                i += n_ini;
                for (; i < (originalText.Length - n_fin + 1); i++)
                {
                    if (originalText[i] == ' ' || originalText[i] == '\n')
                    {
                        stringID = "";
                        break;
                    }
                    if (originalText.Substring(i, n_fin) == KEY_REPLACE_FIN)
                    {
                        if (stringID == "player")
                        {
                            newText = newText.Replace(KEY_REPLACE_INI + stringID + KEY_REPLACE_FIN, "\"" + userData.Username + "\"");
                        }
                        else
                        {
                            newText = newText.Replace(KEY_REPLACE_INI + stringID + KEY_REPLACE_FIN, "\"unknown_key\"");
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

    // --------------------- DialogueData Methods --------------

    public int GetChoice(string choiceID)
    {
        return Serializer.GetDataAsInt("choice", choiceID);
    }

    public void SetChoice(string choiceID, int value)
    {
        Serializer.SetData("choice", choiceID, value.ToString());
    }

    public bool ChoiceExists(string choiceID)
    {
        return Serializer.DataExists("choice", choiceID);
    }

    public bool GetFlag(string flagID)
    {
        return Serializer.GetDataAsBool("flag", flagID);
    }

    public void SetFlag(string flagID, bool value)
    {
        Serializer.SetData("flag", flagID, value);
    }

    public bool FlagExists(string flagID)
    {
        return Serializer.DataExists("flag", flagID);
    }
}