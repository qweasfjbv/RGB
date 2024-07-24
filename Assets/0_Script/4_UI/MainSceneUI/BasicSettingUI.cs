using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicSettingUI : MonoBehaviour
{
    [SerializeField] private List<string> locales;
    [SerializeField] private List<Sprite> checkboxSprites;
    [SerializeField] private TextMeshProUGUI localeText;
    [SerializeField] private Image checkBox;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button checkButton;
    [SerializeField] private Button okButton;

    private int curLocale = 0;
    private bool isChecked = false;

    private void Start()
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        checkButton.onClick.RemoveAllListeners();
        okButton.onClick.RemoveAllListeners();

        leftButton.onClick.AddListener(OnLeftButton);
        rightButton.onClick.AddListener(OnRightButton);
        checkButton.onClick.AddListener(OnCheckButton);
        okButton.onClick.AddListener(OnOkButton);

        curLocale = Managers.Data.GetBasicSetting().locale;
        isChecked = Managers.Data.GetBasicSetting().isBlind;

        UpdateCheckbox();
        UpdateLocale();
    }

    private void OnOkButton()
    {
        Managers.Data.SaveBasicSetting(new BasicSettingData(isChecked, curLocale));
        GameManagerEx.Instance.SetLocale(curLocale);
        gameObject.SetActive(false);
    }

    private void OnLeftButton()
    {
        curLocale++;
        if (curLocale == locales.Count) curLocale = 0;
        UpdateLocale();
    }
    private void OnRightButton()
    {
        curLocale--;
        if (curLocale == -1) curLocale = locales.Count - 1;
        UpdateLocale();
    }

    private void OnCheckButton()
    {
        isChecked = !isChecked;
        UpdateCheckbox();
    }

    private void UpdateLocale()
    {
        localeText.text = locales[curLocale];
    }

    private void UpdateCheckbox()
    {
        if (isChecked)
            checkBox.sprite = checkboxSprites[1];
        else
            checkBox.sprite = checkboxSprites[0];

    }

}
