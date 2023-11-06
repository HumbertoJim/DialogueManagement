using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
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
            private const string equalKey = ":";

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

        public class BooleanDictionaryManager : BaseManager
        {
            [Header("Validation File")]

            [SerializeField] private TextAsset file;

            protected new Serializers.ValidationExtension.DictionarySerializer Serializer
            {
                get { return (Serializers.ValidationExtension.DictionarySerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected void Awake(string dataName)
            {
                Serializer = new Serializers.ValidationExtension.DictionarySerializer(dataName + "Boolean");
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
                string line;
                foreach (string _line_ in lines)
                {
                    line = _line_.Trim();
                    if (line != "" && line[0] != '#') validated_elements.Add(line, "false");
                }
                return validated_elements;
            }
        }

        public class TableManager : BaseManager
        {
            [Header("Validation File")]

            [Tooltip("A file containing a list of allowed rows")]
            [SerializeField] private TextAsset rows;
            [Tooltip("A file containing a list of fields with default values")]
            [SerializeField] private TextAsset fields;
            private const string equalKey = ":";

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
            [SerializeField] private TextAsset[] files;
            private const string equalKey = ":";

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
                foreach(TextAsset file in files)
                {
                    hash = hash.Concat(ComputeHash(file.bytes)).ToArray();
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
                foreach (TextAsset file in files)
                {
                    dictionaries.Add(file.name, new Dictionary<string, string>());
                    lines = file.ToString().Replace('\r', '\n').Split('\n');
                    foreach (string _line_ in lines)
                    {
                        line = _line_.Trim();
                        if (line != "" && line[0] != '#' && line.Contains(equalKey))
                        {
                            key = line.Split(new string[] { equalKey }, StringSplitOptions.None)[0];
                            dictionaries[file.name].Add(key.Trim(), line.Substring(key.Length + equalKey.Length).Trim());
                        }
                    }
                }
                return dictionaries;
            }
        }
    }

    namespace Serializers
    {
        public class BaseSerializer
        {
            public bool Ready { get; protected set; }
            public bool Created { get; protected set; }

            public virtual void SaveData() { throw new NotImplementedException(); }

            public virtual void ResetData() { throw new NotImplementedException(); }
        }

        public class BaseSerializer<T> : BaseSerializer where T : Serializables.BaseSerializable
        {
            private Structs.CoreAttribute<string> name;
            private Structs.CoreAttribute<string> filePath;
            private BinaryFormatter bf;
            protected T data;

            public BaseSerializer(string name)
            {
                this.name.Value = name;
            }

            public void Initialize()
            {
                filePath.Value = Application.persistentDataPath + "/" + name + "Data.dat";
                bf = new BinaryFormatter();
                if (!File.Exists(filePath.Value))
                {
                    Debug.Log("A new " + name + "Data file will be made");
                    ResetData();
                    Created = true;
                }
                else
                {
                    try
                    {
                        data = Deserialize();
                        if (data.CheckConsistensy()) SaveData();
                    }
                    catch
                    {
                        Debug.Log("A new " + name + "Data file will be made due to serialization errors");
                        ResetData();
                    }
                }
                Ready = true;
            }

            public sealed override void SaveData()
            {
                Serialize(data);
            }

            protected void ResetData(T data)
            {
                using (FileStream file = File.Create(filePath.Value)) file.Close();
                this.data = data;
                this.data.CheckConsistensy();
                SaveData();
            }

            private T Deserialize()
            {
                using FileStream file = File.Open(filePath.Value, FileMode.Open);
                T data = (T)bf.Deserialize(file);
                file.Close();
                return data;
            }

            private void Serialize(T data)
            {
                using FileStream file = File.Open(filePath.Value, FileMode.Open);
                bf.Serialize(file, data);
                file.Close();
            }

            public bool CompareHash(byte[] hash)
            {
                return data.CompareHash(hash);
            }

            public void CheckDataConsistensy(byte[] hash)
            {
                data.SetHash(hash);
            }
        }

        public class VariableSerializer : BaseSerializer<Serializables.Variable>
        {
            public VariableSerializer(string name) : base(name + "Variable") { }

            public sealed override void ResetData()
            {
                data = new Serializables.Variable();
                ResetData(data);
            }

            public void Set(string value)
            {
                data.Set(value);
            }

            public void Set(int value)
            {
                Set(value.ToString());
            }

            public void Set(float value)
            {
                Set(value.ToString());
            }

            public void Set(bool value)
            {
                Set(value ? "true" : "false");
            }

            public string Get()
            {
                return data.Get();
            }

            public int GetAsInt()
            {
                return int.Parse(Get());
            }

            public float GetAsFloat()
            {
                return float.Parse(Get());
            }

            public bool GetAsBool()
            {
                return Get() == "true";
            }
        }

        public class DictionarySerializer : BaseSerializer<Serializables.Dictionary>
        {
            public DictionarySerializer(string name) : base(name + "Dictionary") { }

            public sealed override void ResetData()
            {
                data = new Serializables.Dictionary();
                ResetData(data);
            }

            public List<string> GetKeys()
            {
                return data.GetKeys();
            }

            public bool DataExists(string id)
            {
                return data.DataExists(id);
            }

            public void SetData(string id, string value)
            {
                data.SetData(id, value);
            }

            public void SetData(string id, int value)
            {
                SetData(id, value.ToString());
            }

            public void SetData(string id, float value)
            {
                SetData(id, value.ToString());
            }

            public void SetData(string id, bool value)
            {
                SetData(id, value ? "true" : "false");
            }

            public string GetData(string id)
            {
                return data.GetData(id);
            }

            public int GetDataAsInt(string id)
            {
                return int.Parse(GetData(id));
            }

            public float GetDataAsFloat(string id)
            {
                return float.Parse(GetData(id));
            }

            public bool GetDataAsBool(string id)
            {
                return GetData(id) == "true";
            }
        }

        public class TableSerializer : BaseSerializer<Serializables.Table>
        {
            public TableSerializer(string name) : base(name + "Table") { }

            public sealed override void ResetData()
            {
                data = new Serializables.Table();
                ResetData(data);
            }

            public void CheckDataConsistensy(byte[] hash, Dictionary<string, string> fields)
            {
                CheckDataConsistensy(hash);
                data.CheckDataConsistensy(fields);
            }

            public bool FieldExists(string field)
            {
                return data.FieldExists(field);
            }

            public List<string> GetFields()
            {
                return data.GetFields();
            }

            public List<string> GetRows()
            {
                return data.GetRows();
            }

            public bool RowExists(string row)
            {
                return data.RowExists(row);
            }

            public void SetData(string row, string field, string value)
            {
                data.SetData(row, field, value);
            }

            public void SetData(string row, string field, int value)
            {
                SetData(row, field, value.ToString());
            }

            public void SetData(string row, string field, float value)
            {
                SetData(row, field, value.ToString());
            }

            public void SetData(string row, string field, bool value)
            {
                SetData(row, field, value ? "true" : "false");
            }

            public string GetData(string row, string field)
            {
                return data.GetData(row, field);
            }

            public int GetDataAsInt(string row, string field)
            {
                return int.Parse(GetData(row, field));
            }

            public float GetDataAsFloat(string row, string field)
            {
                return float.Parse(GetData(row, field));
            }

            public bool GetDataAsBool(string row, string field)
            {
                return GetData(row, field) == "true";
            }
        }

        public class DictionaryCollectionSerializer : BaseSerializer<Serializables.DictionaryCollection>
        {
            public DictionaryCollectionSerializer(string name) : base(name + "DictionaryCollection") { }

            public sealed override void ResetData()
            {
                data = new Serializables.DictionaryCollection();
                ResetData(data);
            }

            public void CheckDataConsistensy(byte[] hash, List<string> dictionaries)
            {
                CheckDataConsistensy(hash);
                data.CheckDataConsistensy(dictionaries);
            }

            public bool DictionaryExists(string dictionary)
            {
                return data.DictionaryExists(dictionary);
            }

            public bool DataExists(string dictionary, string id)
            {
                return data.DataExists(dictionary, id);
            }

            public List<string> GetDictionaries()
            {
                return data.GetDictionaries();
            }

            public List<string> GetKeys(string dictionary)
            {
                return data.GetKeys(dictionary);
            }

            public void SetData(string dictionary, string id, string value)
            {
                data.SetData(dictionary, id, value);
            }

            public void SetData(string dictionary, string id, int value)
            {
                SetData(dictionary, id, value.ToString());
            }

            public void SetData(string dictionary, string id, float value)
            {
                SetData(dictionary, id, value.ToString());
            }

            public void SetData(string dictionary, string id, bool value)
            {
                SetData(dictionary, id, value ? "true" : "false");
            }

            public string GetData(string dictionary, string id)
            {
                return data.GetData(dictionary, id);
            }

            public int GetDataAsInt(string dictionary, string id)
            {
                return int.Parse(GetData(dictionary, id));
            }

            public float GetDataAsFloat(string dictionary, string id)
            {
                return float.Parse(GetData(dictionary, id));
            }

            public bool GetDataAsBool(string dictionary, string id)
            {
                return GetData(dictionary, id) == "true";
            }
        }

        namespace ValidationExtension
        {
            public class VariableSerializer : Serializers.VariableSerializer
            {
                public VariableSerializer(string name) : base(name) { }

                public void CheckDataConsistensy(byte[] hash, string validated_value)
                {
                    CheckDataConsistensy(hash);
                    if (data.Get() == "")
                    {
                        data.Set(validated_value);
                    }
                }
            }

            public class DictionarySerializer : Serializers.DictionarySerializer
            {
                public DictionarySerializer(string name) : base(name) { }

                public void CheckDataConsistensy(byte[] hash, Dictionary<string, string> validated_elements)
                {
                    CheckDataConsistensy(hash);
                    // remove keys that does not belong to validated data
                    List<string> currentKeys = data.GetKeys();
                    foreach (string key in currentKeys)
                    {
                        if (!validated_elements.ContainsKey(key)) data.RemoveData(key);
                    }

                    // add new keys to data
                    foreach (string element in validated_elements.Keys)
                    {
                        if (!data.DataExists(element)) data.SetData(element, validated_elements[element]);
                    }
                }
            }

            public class TableSerializer : Serializers.TableSerializer
            {
                public TableSerializer(string name) : base(name) { }

                public void CheckDataConsistensy(byte[] hash, Dictionary<string, string> validated_fields, List<string> validated_rows)
                {
                    CheckDataConsistensy(hash, validated_fields);

                    // remove rows that does not belong to validated data
                    List<string> rows = data.GetRows();
                    foreach (string row in rows)
                    {
                        if (!validated_rows.Contains(row)) data.RemoveRow(row);
                    }

                    // add new rows to data
                    foreach (string row in validated_rows)
                    {
                        if (!data.RowExists(row)) data.SetRow(row);
                    }
                }
            }

            public class DictionaryCollectionSerializer : Serializers.DictionaryCollectionSerializer
            {
                public DictionaryCollectionSerializer(string name) : base(name) { }

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

                        // add new rows to data
                        foreach (string element in validated_dictionaries[dictionary].Keys)
                        {
                            if (!data.DataExists(dictionary, element)) data.SetData(dictionary, element, validated_dictionaries[dictionary][element]);
                        }
                    }
                }
            }
        }
    }

    namespace Serializables
    {
        [Serializable]
        public class BaseSerializable
        {
            public byte[] hash;

            public virtual bool CheckConsistensy()
            {
                if( hash == null)
                {
                    hash = new byte[] { };
                    return false;
                }
                return false;
            }

            public bool CompareHash(byte[] hash)
            {
                return this.hash.SequenceEqual(hash);
            }

            public void SetHash(byte[] hash)
            {
                this.hash = hash;
            }
        }

        [Serializable]
        public class Variable: BaseSerializable
        {
            public string data;

            public override bool CheckConsistensy()
            {
                bool consistensy = base.CheckConsistensy();
                if( data == null )
                {
                    data = "";
                    consistensy = false;
                }
                return consistensy;
            }

            public void Set(string value)
            {
                data = value;
            }

            public string Get()
            {
                return data;
            }
        }

        [Serializable]
        public class Dictionary: BaseSerializable
        {
            public Dictionary<string, string> data;

            public override bool CheckConsistensy()
            {
                bool consistensy = base.CheckConsistensy();
                if (data == null)
                {
                    data = new Dictionary<string, string>();
                    consistensy = false;
                }
                return consistensy;
            }

            public bool DataExists(string key)
            {
                return data.ContainsKey(key);
            }

            public string GetData(string key)
            {
                return data[key];
            }

            public void SetData(string key, string value)
            {
                data[key] = value;
            }

            public void RemoveData(string key)
            {
                data.Remove(key);
            }

            public List<string> GetKeys()
            {
                return new List<string>(data.Keys);
            }
        }

        [Serializable]
        public class Table: BaseSerializable
        {
            Dictionary<string, Dictionary<string, string>> data; // each element represents a row, and a row correspond to a field (field-value)
            Dictionary<string, string> fields; // each element represents a dictionaries with its defaul value

            public override bool CheckConsistensy()
            {
                bool consistensy = base.CheckConsistensy();
                if (fields == null)
                {
                    fields = new Dictionary<string, string>();
                    data = new Dictionary<string, Dictionary<string, string>>();
                    consistensy = false;
                }
                if (data == null)
                {
                    data = new Dictionary<string, Dictionary<string, string>>();
                    consistensy = false;
                }
                return consistensy;
            }

            public void CheckDataConsistensy(Dictionary<string, string> fields)
            {
                this.fields = fields;

                foreach (string row in data.Keys)
                {
                    List<string> rowDictionarys = new List<string>(data[row].Keys);
                    foreach (string field in rowDictionarys)
                    {
                        if (!fields.ContainsKey(field)) data[row].Remove(field);
                    }

                    foreach (string field in fields.Keys)
                    {
                        if (!data[row].ContainsKey(field)) data[row].Add(field, fields[field]);
                    }
                }
            }

            public bool FieldExists(string field)
            {
                return fields.ContainsKey(field);
            }

            public bool RowExists(string row)
            {
                return data.ContainsKey(row);
            }

            public void SetRow(string row)
            {
                Dictionary<string, string> new_data = new Dictionary<string, string>();
                foreach (string field in fields.Keys)
                {
                    new_data.Add(field, fields[field]);
                }
                data[row] = new_data;
            }

            public void SetRow(string row, Dictionary<string, string> data)
            {
                Dictionary<string, string> new_data = new Dictionary<string, string>();
                Dictionary<string, string> defaultFields = this.data.ContainsKey(row) ? this.data[row] : fields;
                foreach (string field in defaultFields.Keys)
                {
                    new_data.Add(field, data.ContainsKey(field) ? data[field] : defaultFields[field]);
                }
                this.data[row] = new_data;
            }

            public Dictionary<string, string> GetRow(string row)
            {
                return data[row];
            }

            public void RemoveRow(string row)
            {
                data.Remove(row);
            }

            public List<string> GetRows()
            {
                return new List<string>(data.Keys);
            }

            public List<string> GetFields()
            {
                return new List<string>(fields.Keys);
            }

            public string GetData(string row, string field)
            {
                return data[row][field];
            }

            public void SetData(string row, string field, string value)
            {
                if (fields.ContainsKey(field)) data[row][field] = value;
            }
        }

        [Serializable]
        public class DictionaryCollection: BaseSerializable
        {
            Dictionary<string, Dictionary<string, string>> data; // data[dictionary_id][data_id] = value;
            List<string> dictionaries;

            public override bool CheckConsistensy()
            {
                bool consistensy = base.CheckConsistensy();
                if (dictionaries == null)
                {
                    dictionaries = new List<string>();
                    data = new Dictionary<string, Dictionary<string, string>>();
                    consistensy = false;
                }
                if (data == null)
                {
                    data = new Dictionary<string, Dictionary<string, string>>();
                    consistensy = false;
                }
                return consistensy;
            }

            public void CheckDataConsistensy(List<string> dictionaries)
            {
                this.dictionaries = dictionaries;
                List<string> currentDictionaries = new List<string>(data.Keys);
                foreach (string dictionary in currentDictionaries)
                {
                    if (!dictionaries.Contains(dictionary)) data.Remove(dictionary);
                }
                foreach (string dictionary in dictionaries)
                {
                    if (!data.ContainsKey(dictionary)) data.Add(dictionary, new Dictionary<string, string>());
                }
            }

            public List<string> GetDictionaries()
            {
                return new List<string>(dictionaries);
            }

            public List<string> GetKeys(string dictionary)
            {
                return new List<string>(data[dictionary].Keys);
            }

            public bool DictionaryExists(string dictionary)
            {
                return data.ContainsKey(dictionary);
            }

            public Dictionary<string, string> GetDictionary(string dictionary)
            {
                return new Dictionary<string, string>(data[dictionary]);
            }

            public void SetDictionary(string dictionary, Dictionary<string, string> value)
            {
                data[dictionary] = value;
            }

            public bool DataExists(string dictionary, string id)
            {
                return data[dictionary].ContainsKey(id);
            }

            public string GetData(string dictionary, string id)
            {
                return data[dictionary][id];
            }

            public void SetData(string dictionary, string id, string value)
            {
                data[dictionary][id] = value;
            }

            public void RemoveData(string dictionary, string id)
            {
                data[dictionary].Remove(id);
            }
        }
    }

    namespace Structs
    {
        public struct CoreAttribute<T>
        {
            private T value;
            private bool isFixed;

            public T Value
            {
                get
                {
                    if (isFixed) return value;
                    else throw new Exceptions.NullAttributeException();
                }
                set
                {
                    if (!isFixed)
                    {
                        this.value = value;
                        isFixed = true;
                    }
                    else throw new Exceptions.MultipleSetAttributeException();
                }
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        [Serializable]
        public struct Document
        {
            public string name;
            public TextAsset file;

            public override string ToString()
            {
                return name;
            }
        }
    }

    namespace Exceptions
    {
        public class NullAttributeException : Exception
        {
            public NullAttributeException() : base("Attribute was not fixed.") { }
            public NullAttributeException(string name) : base("Attribute " + name + " was not fixed.") { }
        }
        public class MultipleSetAttributeException : Exception
        {
            public MultipleSetAttributeException() : base("Trying to set Attribute more than once.") { }
            public MultipleSetAttributeException(string name) : base("Trying to set the Attribute " + name + " more than once.") { }
        }
    }
}