using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button BackButton;
    [SerializeField] private Button RetryButton;

    private void Start()
    {
        BackButton.onClick.RemoveAllListeners();
        RetryButton.onClick.RemoveAllListeners();

        BackButton.onClick.AddListener(() => GameManagerEx.Instance.GameEnd());
    }

}
