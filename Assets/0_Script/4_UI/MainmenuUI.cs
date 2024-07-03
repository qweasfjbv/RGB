using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainmenuUI : MonoBehaviour
{
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private TextMeshProUGUI stageTMP;
    [SerializeField] private DisplayBoxController disBox;

    private int currentSelectedIdx = 1;


    private void Start()
    {
        currentSelectedIdx = 1;
        UpdateStageTMP();

        gameStartButton.onClick.AddListener(() => OnStartButtonClicked());
        rightButton.onClick.AddListener(() => OnRightButtonClicked());
        leftButton.onClick.AddListener(() => OnLeftButtonClicked());
    }

    void OnStartButtonClicked()
    {
        GameManagerEx.Instance.GameStart(currentSelectedIdx);
    }

    void OnRightButtonClicked()
    {
        if (Managers.Resource.GetMapCount()-1 == currentSelectedIdx) return;
        currentSelectedIdx++;
        UpdateStageTMP();
    }

    void OnLeftButtonClicked()
    {
        if(currentSelectedIdx == 1) return;
        currentSelectedIdx--;
        UpdateStageTMP();
    }

    void UpdateStageTMP()
    {
        stageTMP.text = "Stage " + currentSelectedIdx;
        disBox.RotateDisBox(currentSelectedIdx);
    }

}
