using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class HintUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentTMP;

    private LocalizeStringEvent localizedStringEvent;

    int count = 0;

    private void Start()
    {
        localizedStringEvent = contentTMP.GetComponent<LocalizeStringEvent>();
        SetLocalizeText("");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetLocalizeText("C" + count.ToString());
            count++;
        }
    }

    private void SetLocalizeText(string key)
    {

        localizedStringEvent.StringReference.SetReference(Constant.HINT_TABLE, key);
        //contentTMP.GetComponent<LocalizeStringEvent>().StringReference.
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];

    }





}
