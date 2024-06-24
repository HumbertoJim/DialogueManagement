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
                const string CHOICES_KEY = "Choices";
                const string FLAGS_KEY = "Flags";
                const string INTERACTIONS_KEY = "Interactions";
                const string RELATIONSHIPS_KEY = "Relationships";

                protected override void Awake()
                {
                    Awake("Dialogue");
                }

                // Choices
                public int GetChoice(string choiceID)
                {
                    return Serializer.GetDataAsInt(CHOICES_KEY, choiceID);
                }

                public void SetChoice(string choiceID, int value)
                {
                    Serializer.SetData(CHOICES_KEY, choiceID, value.ToString());
                }

                public bool ChoiceExists(string choiceID)
                {
                    return Serializer.DataExists(CHOICES_KEY, choiceID);
                }

                // Flags
                public bool GetFlag(string flagID)
                {
                    return Serializer.GetDataAsBool(FLAGS_KEY, flagID);
                }

                public void SetFlag(string flagID, bool value)
                {
                    Serializer.SetData(FLAGS_KEY, flagID, value);
                }

                public bool FlagExists(string flagID)
                {
                    return Serializer.DataExists(FLAGS_KEY, flagID);
                }

                // Character Interactions
                public int CountInteractions(string characterID)
                {
                    return Serializer.GetDataAsInt(INTERACTIONS_KEY, characterID);
                }

                public void RegisterInteraction(string characterID)
                {
                    Serializer.SetData(INTERACTIONS_KEY, characterID, CountInteractions(characterID) + 1);
                }

                // Character Relationships
                public int GetRelationship(string characterID)
                {
                    return Serializer.GetDataAsInt(RELATIONSHIPS_KEY, characterID);
                }

                public void ImproveRelationship(string characterID, int value)
                {
                    Serializer.SetData(RELATIONSHIPS_KEY, characterID, GetRelationship(characterID) + value);
                }

                public void DecreaseRelationship(string characterID, int value)
                {
                    Serializer.SetData(RELATIONSHIPS_KEY, characterID, GetRelationship(characterID) - value);
                }
            }
        }
    }
}