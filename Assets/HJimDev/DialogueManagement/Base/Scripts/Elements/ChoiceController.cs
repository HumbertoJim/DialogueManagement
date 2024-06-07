using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



namespace DialogueManagement
{
    namespace Elements
    {
        public class ChoiceController : MonoBehaviour
        {
            //[SerializeField] PlaySound soundController;
            [SerializeField] TMP_Text choiceText;
            Core.DialogueManager dialogueManager;
            string choiceID;
            int choice;

            public void Choice()
            {
                dialogueManager.SelectChoice(choiceID, choice);
            }

            public void SetInformation(Core.DialogueManager dialogueManager, string choiceID, int choice, string text)
            {
                this.dialogueManager = dialogueManager;
                this.choiceID = choiceID;
                this.choice = choice;
                choiceText.text = text;
                // soundController.SetInformation(dialogueManager.SettingData);
            }
        }
    }
}