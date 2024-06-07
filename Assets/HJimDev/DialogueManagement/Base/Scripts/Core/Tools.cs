using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Tools
    {
        public class StringExtensions
        {
            public static string KeepVowals(string text)
            {
                text = ReplaceForeingVowals(text);
                string vowels = "AEIOU";
                string text_vowels = "";
                text = text.ToUpper();
                foreach (char c in text)
                {
                    if (vowels.Contains(c.ToString()))
                    {
                        text_vowels += c;
                    }
                }
                return text_vowels;
            }

            public static string ReplaceForeingVowals(string text)
            {
                text = text.Replace('\u0430', 'a');
                text = text.Replace('\u0435', 'e');
                text = text.Replace('\u0454', 'e');
                text = text.Replace('\u0438', 'u');
                text = text.Replace('\u0456', 'i');
                text = text.Replace('\u0457', 'i');
                text = text.Replace('\u043E', 'o');
                text = text.Replace('\u0443', 'i');
                text = text.Replace('\u044E', 'u');
                text = text.Replace('\u044F', 'a');
                return text;
            }

            public static bool TextStartsWith(string text, string start)
            {
                if (text.Length < start.Length)
                {
                    return false;
                }
                for (int i = 0; i < start.Length; i++)
                {
                    if (text[i] != start[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}