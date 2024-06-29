using UnityEngine;
using UnityEngine.UI;

public class MainmenuUI : MonoBehaviour
{
    [SerializeField] private Button gameStartButton;


    private void Start()
    {
        // TODO : Idx fix needed
        gameStartButton.onClick.AddListener(() => GameManagerEx.Instance.GameStart(0));   
    }

    

}
