using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;


namespace DialogueManagement
{
    namespace Base
    {
        namespace Data
        {
            public class DialogueData : DictionaryCollectionManager
            {
                protected override void Awake()
                {
                    Awake("Dialogue");
                }

                public int GetChoice(string choiceID)
                {
                    return Serializer.GetDataAsInt("Choices", choiceID);
                }

                public void SetChoice(string choiceID, int value)
                {
                    Serializer.SetData("Choices", choiceID, value.ToString());
                }

                public bool ChoiceExists(string choiceID)
                {
                    return Serializer.DataExists("Choices", choiceID);
                }

                public bool GetFlag(string flagID)
                {
                    return Serializer.GetDataAsBool("Flags", flagID);
                }

                public void SetFlag(string flagID, bool value)
                {
                    Serializer.SetData("Flags", flagID, value);
                }

                public bool FlagExists(string flagID)
                {
                    return Serializer.DataExists("Flags", flagID);
                }
            }
        }
    }
}