using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Vector2 padding;
    [SerializeField] private Vector2 spacing;

    [Space]
    [Header("Expand Duration")]
    [SerializeField] private float collapseDuration;
    [SerializeField] private float expandDuration;
    [SerializeField] private Ease expandEase;
    [SerializeField] private Ease collapseEase;

    [Space]
    [Header("Fade Duration")]
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;

    private Button settingButton;

    [SerializeField]
    private List<SettingListItem> settingItems = new List<SettingListItem>();
    private Vector2 mainButtonPos;
    private bool isExtended;

    private void Awake()
    {
        mainButtonPos = Vector2.zero;
        settingButton = GetComponent<Button>();
    }

    private void Start()
    {
        //settingButton.onClick.RemoveListener(Toggle);
        //settingButton.onClick.AddListener(Toggle);


        isExtended = false;
    }

    private void Toggle()
    {

        int itemCount = settingItems.Count;

        if (isExtended) // 열려있음. 닫는 부분
        {
            isExtended = false;

            for (int i = 0; i < itemCount; i++)
            {
                settingItems[i].rect.DOAnchorPos(mainButtonPos, collapseDuration).SetEase(collapseEase);
                settingItems[i].image.DOFade(0f, fadeOutDuration);
                transform.DORotate(new Vector3(0, 0, 0), collapseDuration);
            }
        }
        else
        {

            for (int i = 0; i < itemCount; i++)
            {
                settingItems[i].rect.DOAnchorPos(mainButtonPos + spacing * (i + 1) + padding, expandDuration).SetEase(expandEase);
                settingItems[i].image.DOFade(1f, fadeInDuration);
                transform.DORotate(new Vector3(0, 0, 90f), collapseDuration);
            }

            isExtended = true;
        }
    }
}
