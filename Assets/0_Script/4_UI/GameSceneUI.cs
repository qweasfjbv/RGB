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
        BackButton.onClick.RemoveAllListeners();
        RetryButton.onClick.RemoveAllListeners();

        BackButton.onClick.AddListener(() => GameManagerEx.Instance.GameEnd());
        RetryButton.onClick.AddListener(() => GameManagerEx.Instance.GameFail());
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
