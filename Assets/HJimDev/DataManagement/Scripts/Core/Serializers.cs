using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace DataManagement
{
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
}