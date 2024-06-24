using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;

public class UserData: DictionaryManager
{
    protected override void Awake() { Awake("UserInformation"); }

    public string Username
    {
        get
        {
            return Serializer.DataExists("username") ? Serializer.GetData("username") : "no_name";
        }
        set
        {
            Serializer.SetData("username", value);
            SaveData();
        }
    }

    public int Gender
    {
        /// Return:
        ///     0 - not specified
        ///     1 - women
        ///     2 - men
        get
        {
            try
            {
                return Serializer.DataExists("gender") ? Serializer.GetDataAsInt("gender") : 0;
            }
            catch
            {
                return 0;
            }
        }
        set
        {
            Serializer.SetData("gender", value > 2 ? 0 : (value < 0 ? 0 : value));
            SaveData();
        }
    }

    public int Age
    {
        get
        {
            try
            {
                return Serializer.DataExists("age") ? Serializer.GetDataAsInt("age") : 18;
            }
            catch
            {
                return 18;
            }
        }
        set
        {
            Serializer.SetData("age", value < 0 ? 0 : value);
            SaveData();
        }
    }
}