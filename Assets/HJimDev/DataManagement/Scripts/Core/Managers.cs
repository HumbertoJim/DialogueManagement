using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace DataManagement
{
    namespace Managers
    {
        public class BaseManager : MonoBehaviour
        {
            protected Serializers.BaseSerializer Serializer { get; set; }

            protected virtual void Awake() { throw new NotImplementedException(); }

            public virtual void SaveData() { Serializer.SaveData(); }

            public virtual void ResetData() { Serializer.ResetData(); }

            public byte[] ComputeHash(byte[] source)
            {
                using SHA256 sha256 = SHA256.Create();
                return sha256.ComputeHash(source);
            }
        }

        public class VariableManager : BaseManager
        {
            [Header("Validation File")]
            [SerializeField] private TextAsset file;

            protected new Serializers.ValidationExtension.VariableSerializer Serializer
            {
                get { return (Serializers.ValidationExtension.VariableSerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected void Awake(string dataName)
            {
                Serializer = new Serializers.ValidationExtension.VariableSerializer(dataName);
                Serializer.Initialize();
                byte[] hash = ComputeHash(file.bytes);
                if (!Serializer.CompareHash(hash))
                {
                    Serializer.CheckDataConsistensy(hash);
                    SaveData();
                }
            }

            protected string ValidatedValue()
            {
                return file.ToString().Trim();
            }
        }

        public class DictionaryManager : BaseManager
        {
            [Header("Validation File")]
            [SerializeField] private TextAsset file;
            [SerializeField] private string equalKey = ":";

            protected new Serializers.ValidationExtension.DictionarySerializer Serializer
            {
                get { return (Serializers.ValidationExtension.DictionarySerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected void Awake(string dataName)
            {
                Serializer = new Serializers.ValidationExtension.DictionarySerializer(dataName);
                Serializer.Initialize();
                byte[] hash = ComputeHash(file.bytes);
                if (!Serializer.CompareHash(hash))
                {
                    Serializer.CheckDataConsistensy(hash, ValidatedElements());
                    SaveData();
                }
            }

            protected Dictionary<string, string> ValidatedElements()
            {
                Dictionary<string, string> validated_elements = new Dictionary<string, string>();
                string[] lines = file.ToString().Replace('\r', '\n').Split('\n');
                string line, key;
                foreach (string _line_ in lines)
                {
                    line = _line_.Trim();
                    if (line != "" && line[0] != '#' && line.Contains(equalKey))
                    {
                        key = line.Split(new string[] { equalKey }, StringSplitOptions.None)[0];
                        validated_elements.Add(key.Trim(), line.Substring(key.Length + equalKey.Length).Trim());
                    }
                }
                return validated_elements;
            }
        }

        public class DefaultValueDictionaryManager : BaseManager
        {
            [Header("Validation File")]
            [SerializeField] private TextAsset file;

            protected new Serializers.ValidationExtension.DictionarySerializer Serializer
            {
                get { return (Serializers.ValidationExtension.DictionarySerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected void Awake(string dataName, string defaultValue)
            {
                Serializer = new Serializers.ValidationExtension.DictionarySerializer(dataName);
                Serializer.Initialize();
                byte[] hash = ComputeHash(file.bytes);
                if (!Serializer.CompareHash(hash))
                {
                    Serializer.CheckDataConsistensy(hash, ValidatedElements(defaultValue));
                    SaveData();
                }
            }

            protected Dictionary<string, string> ValidatedElements(string defaultValue)
            {
                Dictionary<string, string> validated_elements = new Dictionary<string, string>();
                string[] lines = file.ToString().Replace('\r', '\n').Split('\n');
                string line;
                foreach (string _line_ in lines)
                {
                    line = _line_.Trim();
                    if (line != "" && line[0] != '#') validated_elements.Add(line, defaultValue);
                }
                return validated_elements;
            }
        }

        public class StringDictionaryManager : DefaultValueDictionaryManager
        {
            protected void Awake(string dataName) { Awake(dataName + "String", ""); }

            protected void SetData(string id, string value) { Serializer.SetData(id, value); }

            protected string GetData(string id) { return Serializer.GetData(id); }
        }

        public class IntegerDictionaryManager : DefaultValueDictionaryManager
        {
            protected void Awake(string dataName) { Awake(dataName + "Integer", "0"); }

            protected void SetData(string id, int value) { Serializer.SetData(id, value); }

            protected int GetData(string id) { return Serializer.GetDataAsInt(id); }
        }

        public class BooleanDictionaryManager : DefaultValueDictionaryManager
        {
            protected void Awake(string dataName) { Awake(dataName + "Boolean", "false"); }

            protected void SetData(string id, bool value){ Serializer.SetData(id, value); }

            protected bool GetData(string id) { return Serializer.GetDataAsBool(id); }
        }

        public class TableManager : BaseManager
        {
            [Header("Validation File")]
            [Tooltip("A file containing a list of allowed rows")]
            [SerializeField] private TextAsset rows;
            [Tooltip("A file containing a list of fields with default values")]
            [SerializeField] private TextAsset fields;
            [SerializeField] private string equalKey = ":";

            protected new Serializers.ValidationExtension.TableSerializer Serializer
            {
                get { return (Serializers.ValidationExtension.TableSerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected void Awake(string dataName)
            {
                Serializer = new Serializers.ValidationExtension.TableSerializer(dataName);
                Serializer.Initialize();
                byte[] hash = ComputeHash(fields.bytes).Concat(ComputeHash(rows.bytes)).ToArray();
                if (!Serializer.CompareHash(hash))
                {
                    Serializer.CheckDataConsistensy(hash, ValidatedFields(), ValidatedRows());
                    SaveData();
                }
            }

            protected Dictionary<string, string> ValidatedFields()
            {
                Dictionary<string, string> validated_fields = new Dictionary<string, string>();
                string[] lines = fields.ToString().Replace('\r', '\n').Split('\n');
                string line, key;
                foreach (string _line_ in lines)
                {
                    line = _line_.Trim();
                    if (line != "" && line[0] != '#' && line.Contains(equalKey))
                    {
                        key = line.Split(new string[] { equalKey }, StringSplitOptions.None)[0];
                        validated_fields.Add(key.Trim(), line.Substring(key.Length + equalKey.Length).Trim());
                    }
                }
                return validated_fields;
            }

            List<string> ValidatedRows()
            {
                List<string> validated_rows = new List<string>();
                string[] lines = fields.ToString().Replace('\r', '\n').Split('\n');
                foreach (string line in lines)
                {
                    validated_rows.Add(line.Trim());
                }
                return validated_rows;
            }
        }

        public class DictionaryCollectionManager : BaseManager
        {
            [Header("Validation File")]
            [SerializeField] private Structs.Document[] files;
            [SerializeField] private string equalKey = ":";

            protected new Serializers.ValidationExtension.DictionaryCollectionSerializer Serializer
            {
                get { return (Serializers.ValidationExtension.DictionaryCollectionSerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected void Awake(string dataName)
            {
                Serializer = new Serializers.ValidationExtension.DictionaryCollectionSerializer(dataName);
                Serializer.Initialize();
                byte[] hash = new byte[] { };
                foreach(Structs.Document document in files)
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
                foreach (Structs.Document document in files)
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
        }
    }
}