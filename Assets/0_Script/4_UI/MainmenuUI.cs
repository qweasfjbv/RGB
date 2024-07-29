using DG.Tweening;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainmenuUI : MonoBehaviour
{

    [Header("StageButtonPivot")]
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button stageRightButton;
    [SerializeField] private Button stageLeftButton;
    [SerializeField] private TextMeshProUGUI stageTMP;
    [SerializeField] private DisplayBoxController disBox;

    [Header("PlayModePivot")]
    [SerializeField] private List<TextMeshProUGUI> GameModeTMPs;
    [SerializeField] private Button modeRightButton;
    [SerializeField] private Button modeLeftButton;
    [SerializeField] private float modeFadeDuration;
    [SerializeField, ReadOnly(true)] private GameType curGameType;

    [Header("SettingUIs")]
    [SerializeField] private GameObject basicSettingUI;

    private int currentSelectedIdx = 1;
    private Color alphaColor;

    private void EraseAllListener()
    {
        gameStartButton.onClick.RemoveAllListeners();
        stageRightButton.onClick.RemoveAllListeners();
        stageLeftButton.onClick.RemoveAllListeners();
        modeRightButton.onClick.RemoveAllListeners();
        modeLeftButton.onClick.RemoveAllListeners();
    }
    private void Start()
    {
        alphaColor = new Color(1, 1, 1, 0);
        currentSelectedIdx = 1;
        UpdateStageTMP();
        EraseAllListener();

        // Stage Change Buttons
        gameStartButton.onClick.AddListener(() => OnStartButtonClicked());
        stageRightButton.onClick.AddListener(() => OnStageRightButtonClicked());
        stageLeftButton.onClick.AddListener(() => OnStageLeftButtonClicked());

        curGameType = GameType.TUTO;
        GameModeTMPs[0].gameObject.SetActive(true);
        for (int i = 1; i < GameModeTMPs.Count; i++)
        {
            GameModeTMPs[i].gameObject.SetActive(false);
        }

        // Mode Change Buttons
        modeRightButton.onClick.AddListener(() => OnModeRightButtonClicked());
        modeLeftButton.onClick.AddListener(() => OnModeLeftButtonClicked());

    }

    private void OnStartButtonClicked()
    {
        GameManagerEx.Instance.GameStart(curGameType, currentSelectedIdx);
    }

    private void OnStageRightButtonClicked()
    {
        if (Managers.Resource.GetMapCount(curGameType) - 1 == currentSelectedIdx) return;
        currentSelectedIdx++;
        UpdateStageTMP();
    }

    private void OnStageLeftButtonClicked()
    {
        if (currentSelectedIdx == 1) return;
        currentSelectedIdx--;
        UpdateStageTMP();
    }

    private void UpdateStageTMP()
    {
        stageTMP.text = currentSelectedIdx.ToString();
        disBox.RotateDisBox(currentSelectedIdx);
    }

    private void OnModeRightButtonClicked()
    {
        currentSelectedIdx = 1;
        UpdateStageTMP();

        int curMode = (int)curGameType;
        int nextMode = (curMode + 1) % GameModeTMPs.Count;

        GameModeTMPs[curMode].DOColor(alphaColor, modeFadeDuration);
        GameModeTMPs[curMode].GetComponent<RectTransform>().DOAnchorPosY(-50, modeFadeDuration);

        GameModeTMPs[nextMode].color = alphaColor;
        GameModeTMPs[nextMode].GetComponent<RectTransform>().anchoredPosition = new Vector3(GameModeTMPs[curMode].GetComponent<RectTransform>().anchoredPosition.x, 50, 0);
        GameModeTMPs[nextMode].gameObject.SetActive(true);

        GameModeTMPs[nextMode].DOColor(Color.white, modeFadeDuration);
        GameModeTMPs[nextMode].GetComponent<RectTransform>().DOAnchorPosY(0, modeFadeDuration);

        curGameType = (GameType)nextMode;
        GameManagerEx.Instance.CurGameType = curGameType;
    }

    private void OnModeLeftButtonClicked()
    {
        currentSelectedIdx = 1;
        UpdateStageTMP();

        int curMode = (int)curGameType;
        int nextMode = (curMode - 1) < 0 ? curMode - 1 + GameModeTMPs.Count : curMode - 1;

        GameModeTMPs[curMode].DOColor(alphaColor, modeFadeDuration);
        GameModeTMPs[curMode].GetComponent<RectTransform>().DOAnchorPosY(50, modeFadeDuration);

        GameModeTMPs[nextMode].color = alphaColor;
        GameModeTMPs[nextMode].GetComponent<RectTransform>().anchoredPosition = new Vector3(GameModeTMPs[curMode].GetComponent<RectTransform>().anchoredPosition.x, -50, 0);
        GameModeTMPs[nextMode].gameObject.SetActive(true);

        GameModeTMPs[nextMode].DOColor(Color.white, modeFadeDuration);
        GameModeTMPs[nextMode].GetComponent<RectTransform>().DOAnchorPosY(0, modeFadeDuration);

        curGameType = (GameType)nextMode;
        GameManagerEx.Instance.CurGameType = curGameType;
    }

    public void OnBasicSettingButton()
    {
        basicSettingUI.SetActive(true);
    }
}
