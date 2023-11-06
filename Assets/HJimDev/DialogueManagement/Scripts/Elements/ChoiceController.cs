using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DialogueManagement;

public class ChoiceController : MonoBehaviour
{
    //[SerializeField] PlaySound soundController;
    [SerializeField] TMP_Text choiceText;
    DialogueManager dialogueManager;
    string choiceID;
    int choice;

    public void Choice()
    {
        dialogueManager.SelectChoice(choiceID, choice);
    }

    public void SetInformation(DialogueManager dialogueManager, string choiceID, int choice, string text)
    {
        this.dialogueManager = dialogueManager;
        this.choiceID = choiceID;
        this.choice = choice;
        choiceText.text = text;
        // soundController.SetInformation(dialogueManager.SettingData);
    }
}