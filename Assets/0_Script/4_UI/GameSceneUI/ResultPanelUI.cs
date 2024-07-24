using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject grayBG;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI clearStageText;

    public void SetResultPanel(GameType type, int curIdx)
    {
        grayBG.SetActive(true);

        switch (type)
        {
            case GameType.TUTO:
                clearStageText.text = "Tutorial " + curIdx.ToString();
                break;
            case GameType.STAGE:
                clearStageText.text = "Stage " + curIdx.ToString();
                break;
        }

        BoxController.LockInputBlock();

        menuButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();

        menuButton.gameObject.SetActive(true);
        menuButton.onClick.AddListener(GameManagerEx.Instance.GameEnd);
        menuButton.onClick.AddListener(unsetUI);

        if (Managers.Resource.GetMapCount(GameManagerEx.Instance.CurGameType) -1 == curIdx)
        {
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            menuButton.gameObject.SetActive(true);
            nextButton.onClick.AddListener(GameManagerEx.Instance.GameSuccess);
            nextButton.onClick.AddListener(unsetUI);
        }
    }   

    private void unsetUI()
    {
        grayBG.SetActive(false);
        gameObject.SetActive(false);
    }
}
