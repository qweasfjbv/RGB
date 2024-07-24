using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class HintUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentTMP;

    private LocalizeStringEvent localizedStringEvent;


    private void Start()
    {
        localizedStringEvent = contentTMP.GetComponent<LocalizeStringEvent>();
    }

    public void SetLocalizeText(string key)
    {
        contentTMP.GetComponent<LocalizeStringEvent>().StringReference.SetReference(Constant.HINT_TABLE, key);
    }


}
