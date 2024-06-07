using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;


namespace DialogueManagement
{
    namespace Core
    {
        public class DialogueData : DictionaryCollectionManager
        {
            protected override void Awake()
            {
                Awake("Dialogue");
            }

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
    }
}