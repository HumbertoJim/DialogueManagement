using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TextUpdater : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] TextManager textManager;

    [Header("Updates")]
    [SerializeField] string textType = "UIElements";
    [SerializeField] TextID[] texts;


    private void Start()
    {
        foreach (TextID text in texts)
        {
            TMP_Text t = text.GetComponent<TMP_Text>();
            if (t)
            {
                t.text = textManager.GetText(textType, text.textID);
            }
            else
            {
                Text tt = text.GetComponent<Text>();
                if (tt)
                {
                    tt.text = textManager.GetText(textType, text.textID);
                }
            }
        }
    }
}
