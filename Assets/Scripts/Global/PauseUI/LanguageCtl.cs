using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageCtl : MonoBehaviour
{
    private TMP_Text currentLanguageText;
    private string[] languages = { "zh", "en" };
    private int index;

    private void Awake()
    {
        if (!currentLanguageText)
        {
            currentLanguageText = GetComponent<TMP_Text>();
        }
    }
}
