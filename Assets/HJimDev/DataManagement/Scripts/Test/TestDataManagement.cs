using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDataManagement : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] UserDataManager userData;
    [SerializeField] AchievementDataManager achievementData;

    [Header("UI Elements")]
    [SerializeField] Text label;

    private void Start()
    {
        string text = "";
        text += "Username: " + userData.Username + "\n\n";
        text += "Achievements to unlock\n";
        text += "----------------------\n";
        foreach (string achievement in achievementData.GetLockedAchievements())
        {
            text += "+" + achievement + "\n";
        }
        label.text = text;
    }
}
