using Fusion;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button BackButton;
    [SerializeField] private Button RetryButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI stageText;

    private void Start()
    {
        if (GameManagerEx.Instance.CurGameType == GameType.MULTI)
            RetryButton.gameObject.SetActive(false);
        else
            RetryButton.gameObject.SetActive(true);

        BackButton.onClick.RemoveAllListeners();
        RetryButton.onClick.RemoveAllListeners();

        BackButton.onClick.AddListener(() => GameManagerEx.Instance.GameEnd());
        RetryButton.onClick.AddListener(() => GameManagerEx.Instance.GameFail());

        if (GameManagerEx.Instance.CurGameType == GameType.MULTI) stageText.gameObject.SetActive(false);
    }

    public void UpdateStageText(GameType type, int idx, NetworkRunner runner)
    {
        switch (type)
        {
            case GameType.TUTO:
                stageText.text = "Tutorial " + idx;
                break;
            case GameType.STAGE:
                stageText.text = "Stage " + idx;
                break;
            case GameType.MULTI:
                stageText.text = "Multi " + idx;
                break;
            default:
                stageText.text = "ERROR";
                break;
        }


    }

}
