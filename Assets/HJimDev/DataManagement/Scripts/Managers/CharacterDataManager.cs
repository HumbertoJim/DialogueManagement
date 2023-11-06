using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;

public class CharacterDataManager : BooleanDictionaryManager
{
    protected override void Awake()
    {
        Awake("Characters");
    }

    public bool CharacterExists(string characterID)
    {
        return Serializer.DataExists(characterID);
    }

    public bool IsCharacterUnlocked(string characterID)
    {
        if (CharacterExists(characterID)) return Serializer.GetDataAsBool(characterID);
        return false;
    }

    public void SetCharacterUnlocked(string characterID, bool value)
    {
        Serializer.SetData(characterID, value);
        SaveData();
    }

    public Dictionary<string, bool> GetCharacters()
    {
        List<string> characters = Serializer.GetKeys();
        Dictionary<string, bool> list = new Dictionary<string, bool>();
        foreach (string character in characters)
        {
            list.Add(character, Serializer.GetDataAsBool(character));
        }
        return list;
    }

    public List<string> GetUnlockedCharacters()
    {
        List<string> characters = Serializer.GetKeys();
        List<string> list = new List<string>();
        foreach (string character in characters)
        {
            if (Serializer.GetDataAsBool(character))
            {
                list.Add(character);
            }
        }
        return list;
    }

    public List<string> GetLockedCharacters()
    {
        List<string> characters = Serializer.GetKeys();
        List<string> list = new List<string>();
        foreach (string character in characters)
        {
            if (!Serializer.GetDataAsBool(character))
            {
                list.Add(character);
            }
        }
        return list;
    }
}