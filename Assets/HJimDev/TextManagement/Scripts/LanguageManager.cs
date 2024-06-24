using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;
using DataManagement.Managers;
using DataManagement.Structs;


public class LanguageManager : BaseManager
{
    [Header("Language")]
    [SerializeField] private string language;

    [Header("Validation File")]
    [SerializeField] private Document[] documents;
    [SerializeField] private string equalKey = ":";

    public string Language { get { return language; } private set { language = value; } }

    protected new LanguageSerializer Serializer
    {
        get { return (LanguageSerializer)base.Serializer; }
        set { base.Serializer = value; }
    }

    protected sealed override void Awake()
    {
        Serializer = new LanguageSerializer(language + "Language");
        Serializer.Initialize();
        byte[] hash = new byte[] { };
        foreach (Document document in documents)
        {
            hash = hash.Concat(ComputeHash(document.file.bytes)).ToArray();
        }
        if (!Serializer.CompareHash(hash))
        {
            Serializer.CheckDataConsistensy(hash, ValidatedDictionaries());
            SaveData();
        }
    }

    protected Dictionary<string, Dictionary<string, string>> ValidatedDictionaries()
    {
        Dictionary<string, Dictionary<string, string>> dictionaries = new Dictionary<string, Dictionary<string, string>>();
        string[] lines;
        string line, key;
        foreach (Document document in documents)
        {
            dictionaries.Add(document.name, new Dictionary<string, string>());
            lines = document.file.ToString().Replace('\r', '\n').Split('\n');
            foreach (string _line_ in lines)
            {
                line = _line_.Trim();
                if (line != "" && line[0] != '#' && line.Contains(equalKey))
                {
                    key = line.Split(new string[] { equalKey }, StringSplitOptions.None)[0];
                    dictionaries[document.name].Add(key.Trim(), line.Substring(key.Length + equalKey.Length).Trim());
                }
            }
        }
        return dictionaries;
    }

    public bool TextExists(string type, string textID)
    {
        if (Serializer.DictionaryExists(type)) return Serializer.DataExists(type, textID);
        return false;
    }

    public string GetText(string type, string textID)
    {
        return Serializer.GetData(type, textID);
    }
}