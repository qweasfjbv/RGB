using System;
using System.IO;
using UnityEngine;
using UnityEngine.Localization.Settings;

[Serializable]
public class BasicSettingData {

    public BasicSettingData(bool isblind, int loc)
    {
        isBlind= isblind;
        locale = loc;
    }

    public bool isBlind;
    public int locale;
}


public class DataManager
{

    // PATHS
    private string basicSettingPath = Application.dataPath + "/basicSetting";

    // DATAS
    private BasicSettingData basicSettingData = null;



    public void Init()
    {
        LoadBasicSetting();
    }

    public void SaveBasicSetting(BasicSettingData settingData)
    {
        string json = JsonUtility.ToJson(settingData, true);
        File.WriteAllText(basicSettingPath, json);

        // Update Current Data
        LoadBasicSetting();
    }

    public void LoadBasicSetting()
    {
        if (File.Exists(basicSettingPath))
        {
            string json = File.ReadAllText(basicSettingPath);

            basicSettingData = JsonUtility.FromJson<BasicSettingData>(json);   
        }
        else
        {
            // Basic Setting
            basicSettingData = new BasicSettingData(false, 0);
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[basicSettingData.locale];
    }

    public BasicSettingData GetBasicSetting()
    {
        return basicSettingData;
    }

}
