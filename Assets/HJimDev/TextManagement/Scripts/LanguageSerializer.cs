using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DataManagement.Serializers;


public class LanguageSerializer : DictionaryCollectionSerializer
{
    public LanguageSerializer(string name) : base(name) { }

    public void CheckDataConsistensy(byte[] hash, Dictionary<string, Dictionary<string, string>> validated_dictionaries)
    {
        CheckDataConsistensy(hash, new List<string>(validated_dictionaries.Keys));

        foreach (string dictionary in validated_dictionaries.Keys)
        {
            // remove rows that does not belong to validated data
            List<string> elements = new List<string>(data.GetDictionary(dictionary).Keys);
            foreach (string element in elements)
            {
                if (!validated_dictionaries[dictionary].ContainsKey(element)) data.RemoveData(dictionary, element);
            }

            // set validated rows to data
            foreach (string element in validated_dictionaries[dictionary].Keys)
            {
                data.SetData(dictionary, element, validated_dictionaries[dictionary][element]);
            }
        }
    }
}