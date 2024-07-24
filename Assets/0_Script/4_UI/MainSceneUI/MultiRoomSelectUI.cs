using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiRoomSelectUI : MonoBehaviour
{
    [SerializeField] private Button joinButton;

    [SerializeField] private TMP_InputField inputField;

    private int curIdx = -1;

    private void Start()
    {
        inputField.onValueChanged.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();

        inputField.onValueChanged.AddListener(inputValueChanged);
        joinButton.onClick.AddListener(OnJoinButton);
    }

    public void SetPrefix(int idx)
    {
        curIdx = idx;
    }

    private void OnJoinButton()
    {

        string result = FormatString(inputField.text, curIdx);
        GameManagerEx.Instance.GameStart(GameType.MULTI, curIdx, result);
        gameObject.SetActive(false);
    }
    static string CleanInput(string strIn)
    {
        return Regex.Replace(strIn,
              @"[^a-zA-Z0-9]", "");
    }

    void inputValueChanged(string attemptedVal)
    {
        inputField.text = CleanInput(attemptedVal);
    }

    string FormatString(string input, int number)
    {
        string numberString = number.ToString();

        int maxLength = 15;

        string formattedString = numberString + input;

        int currentLength = formattedString.Length;

        if (currentLength < maxLength)
        {
            int zerosToAdd = maxLength - currentLength;
            formattedString = formattedString.PadRight(maxLength, '0');
        }

        return formattedString;
    }
}
