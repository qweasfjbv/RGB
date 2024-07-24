using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TutorialManager : MonoBehaviour
{

    #region Singleton
    private static TutorialManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

    }

    public static TutorialManager Instance
    {
        get
        {
            if (null == instance) return null;
            return instance;
        }
    }

    #endregion


    [SerializeField] private GameObject hintBG;
    [SerializeField] private GameObject hintPanel;

    private bool isHintPop = false;

    private void Start()
    {
        PlayerPrefs.SetInt("C", -1);
    }

    public void PopupHint(bool isBlind, int idx)
    {
        if (isHintPop) return;

        hintBG.SetActive(true);
        hintPanel.GetComponent<HintUI>().SetLocalizeText((isBlind ? "B" : "C") + (idx-1).ToString());

        hintPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        hintPanel.GetComponent<RectTransform>().DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic).OnComplete(()=>isHintPop = true);
        hintPanel.SetActive(true);

    }

    private void Popdown()
    {
        hintPanel.SetActive(false);
        isHintPop = false;
        hintBG.SetActive(false);
        BoxController.UnlockInputBlock();
    }

    private void Update()
    {
        if (isHintPop && (Input.GetKey(KeyCode.Return) || Input.GetMouseButtonDown(0)))
                Popdown();
    }

}
