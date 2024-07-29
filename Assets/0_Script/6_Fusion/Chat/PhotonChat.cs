using Fusion;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class PhotonChat : NetworkBehaviour
{
    public TMP_InputField inputField;   // 메시지를 입력할 UI InputField
    public TextMeshProUGUI chatDisplay;        // 채팅 메시지를 표시할 UI Text




    private bool chatDelay = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(inputField.text.Trim().Length > 0 && !chatDelay)
            {
                OnSendButtonClicked();
                chatDelay = true;
                Invoke(nameof(DelayChat), 5f);
            }
        }
    }

    public void OnSendButtonClicked()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            RPC_SendMessage(inputField.text);
            inputField.text = string.Empty;
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SendMessage(string message, RpcInfo info = default)
    {
        string sender =  $"{info.Source}";
        string formattedMessage = $"{sender} : {message}\n";

        chatDisplay.text += formattedMessage;
        chatDisplay.text = CheckLongMessage(chatDisplay.text);
    }

    private string CheckLongMessage(string s)
    {
        int newlineCount = s.Count(c => c == '\n');

        if (newlineCount < 5) return s;

        int newlineIndex = s.IndexOf('\n');
        string result = s.Substring(newlineIndex + 1);
        return result;
    }

    private void DelayChat()
    {
        chatDelay = false;
    }
}