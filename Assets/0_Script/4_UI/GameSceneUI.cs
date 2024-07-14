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

    public void UpdateStageText(int idx)
    {
        stageText.text = "Stage " + idx;
    }

}
