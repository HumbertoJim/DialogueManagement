using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;

public class AchievementDataManager : BooleanDictionaryManager
{
    protected override void Awake()
    {
        Awake("Achievements");
    }

    public bool AchievementExists(string achievementID)
    {
        return Serializer.DataExists(achievementID);
    }

    public bool GetAchievement(string achievementID)
    {
        if (AchievementExists(achievementID)) return Serializer.GetDataAsBool(achievementID);
        return false;
    }

    public void SetAchievement(string achievementID, bool value)
    {
        Serializer.SetData(achievementID, value ? "true" : "false");
        SaveData();
    }

    public Dictionary<string, bool> GetAchievements()
    {
        List<string> achievements = Serializer.GetKeys();
        Dictionary<string, bool> list = new Dictionary<string, bool>();
        foreach(string achievement in achievements)
        {
            list.Add(achievement, Serializer.GetDataAsBool(achievement));
        }
        return list;
    }

    public List<string> GetUnlockedAchievements()
    {
        List<string> achievements = Serializer.GetKeys();
        List<string> list = new List<string>();
        foreach (string achievement in achievements)
        {
            if(Serializer.GetDataAsBool(achievement))
            {
                list.Add(achievement);
            }
        }
        return list;
    }

    public List<string> GetLockedAchievements()
    {
        List<string> achievements = Serializer.GetKeys();
        List<string> list = new List<string>();
        foreach (string achievement in achievements)
        {
            if (!Serializer.GetDataAsBool(achievement))
            {
                list.Add(achievement);
            }
        }
        return list;
    }
}