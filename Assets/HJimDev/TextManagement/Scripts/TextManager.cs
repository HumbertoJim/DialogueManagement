using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DataManagement.Managers;
using DataManagement.Serializers.ValidationExtension;


public class TextManager : BaseManager
{
    [Header("TextManager")]
    [SerializeField] LanguageManager[] languageManagers;
    [SerializeField] string enterKey = "?n?";
    [SerializeField] string defaultText = "{{text}}";

    Dictionary<string, LanguageManager> languages;

    protected new VariableSerializer Serializer
    {
        get { return (VariableSerializer)base.Serializer; }
        set { base.Serializer = value; }
    }

    protected sealed override void Awake()
    {
        Serializer = new VariableSerializer("TextManager");
        Serializer.Initialize();

        languages = new Dictionary<string, LanguageManager>();
        bool validLanguage = false;
        foreach (LanguageManager manager in languageManagers)
        {
            if (manager.Language == Serializer.Get()) validLanguage = true;
            languages.Add(manager.Language, manager);
        }
        if (!validLanguage)
        {
            Serializer.Set(ValidatedValue());
            Serializer.SaveData();
        }
    }

    protected string ValidatedValue()
    {
        if (languageManagers.Length > 0) return languageManagers[0].Language;
        return "";
    }

    public List<string> GetLanguages()
    {
        return new List<string>(languages.Keys);
    }

    public void SetLanguage(string language)
    {
        if (languages.ContainsKey(language))
        {
            Serializer.Set(language);
            Serializer.SaveData();
        }
    }

    public bool TextExists(string type, string textID)
    {
        LanguageManager manager;
        if (languages.TryGetValue(Serializer.Get(), out manager))
        {
            return manager.TextExists(type, textID);
        }
        return false;
    }

    public string GetText(string type, string textID, bool replaceEnter = true)
    {
        LanguageManager manager;
        if (languages.TryGetValue(Serializer.Get(), out manager))
        {
            if (manager.TextExists(type, textID))
            {
                string text = manager.GetText(type, textID);
                return replaceEnter ? text.Replace(enterKey, "\n") : text;
            }
        }
        return defaultText;
    }
}