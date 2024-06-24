using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;

public class SettingData : DictionaryManager
{
    protected override void Awake() { Awake("Settings"); }

    public float MusicVolume
    {
        get
        {
            try
            {
                return Serializer.DataExists("music_volume") ? Serializer.GetDataAsFloat("music_volume") : 1;
            }
            catch
            {
                return 1;
            }
        }
        set
        {
            Serializer.SetData("music_volume", value > 1f ? 1f : (value < 0f ? 0f : value));
            SaveData();
        }
    }

    public float SoundVolume
    {
        get
        {
            try
            {
                return Serializer.DataExists("sound_volume") ? Serializer.GetDataAsFloat("sound_volume") : 1;
            }
            catch
            {
                return 1;
            }
        }
        set
        {
            Serializer.SetData("sound_volume", value > 1f ? 1f : (value < 0f ? 0f : value));
            SaveData();
        }
    }
}