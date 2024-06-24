using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;

public class AchievementData : BooleanDictionaryManager
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
        if (AchievementExists(achievementID)) return GetData(achievementID);
        return false;
    }

    public void SetAchievement(string achievementID, bool value)
    {
        SetData(achievementID, value);
        SaveData();
    }

    public Dictionary<string, bool> GetAchievements()
    {
        List<string> achievements = Serializer.GetKeys();
        Dictionary<string, bool> list = new Dictionary<string, bool>();
        foreach(string achievement in achievements)
        {
            list.Add(achievement, GetData(achievement));
        }
        return list;
    }

    public List<string> GetUnlockedAchievements()
    {
        List<string> achievements = Serializer.GetKeys();
        List<string> list = new List<string>();
        foreach (string achievement in achievements)
        {
            if(GetData(achievement))
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
            if (!GetData(achievement))
            {
                list.Add(achievement);
            }
        }
        return list;
    }
}