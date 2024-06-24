using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace DataManagement
{
    namespace Serializables
    {
        [Serializable]
        public class BaseSerializable
        {
            public byte[] hash;

            public virtual bool CheckConsistensy()
            {
                if (hash == null)
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
        public class Variable : BaseSerializable
        {
            public string data;

            public override bool CheckConsistensy()
            {
                bool consistensy = base.CheckConsistensy();
                if (data == null)
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
        public class Dictionary : BaseSerializable
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
        public class Table : BaseSerializable
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
        public class DictionaryCollection : BaseSerializable
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
}