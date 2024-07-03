using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerEx : MonoBehaviour
{

    #region Singleton
    private static GameManagerEx instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public static GameManagerEx Instance
    {
        get
        {
            if (null == instance) return null;
            return instance;
        }
    }
    #endregion

    private int currentLv = -1;

    [Header("Scene Conversion Effect")]
    [SerializeField] private float duration;
    [SerializeField] private RectTransform shade;

    private void Start()
    {
        shade.gameObject.SetActive(false);
    }

    public void GameStart(int lvId)
    {
        currentLv = lvId;
        StartCoroutine(GameStartCoroutine(lvId));

        return;
    }

    public void GameEnd()
    {
        StartCoroutine(GameEndCoroutine());

        return;
    }

    public void GameFail()
    {
        MapGenerator.Instance.ResetAndInit(currentLv);
    }

    public void GameSuccess()
    {
        if (currentLv == Managers.Resource.GetMapCount())
        {
            // All Clear!
        }
        else
        {
            // TODO : DataManager에 접근 필요
            StartCoroutine(GameRestartCoroutine(++currentLv));
        }

    }


    public IEnumerator GameStartCoroutine(int idx)
    {
        yield return StartCoroutine(StartSceneShade());

        // LoadScene and wait-> MapGenerate
        AsyncOperation async = SceneManager.LoadSceneAsync(Constant.GAME_SCENE);
        yield return async;



        FinSceneShade();
        MapGenerator.Instance.GenerateMap(idx);

    }

    // Convert Scene Game -> MainMenu
    public IEnumerator GameEndCoroutine()
    {
        yield return StartCoroutine(RevStartSceneShade());

        AsyncOperation async = SceneManager.LoadSceneAsync(Constant.MAIN_SCENE);
        yield return async;

        RevFinSceneShade();
        
    }

    public IEnumerator GameRestartCoroutine(int idx)
    {
        yield return StartCoroutine(RevStartSceneShade());

        MapGenerator.Instance.EraseAllObject();
        FinSceneShade();
        MapGenerator.Instance.GenerateMap(idx);
    }
    // Scene Convert Eff
    
    private IEnumerator StartSceneShade()
    {
        shade.sizeDelta = new Vector2(0, 0);
        shade.gameObject.SetActive(true);
        bool tmpB = false;
        shade.DOSizeDelta(new Vector2(1000, 1000), duration).OnComplete(()=>tmpB=true);

        while (!tmpB)
        {
            yield return null;
        }

    }

    private IEnumerator RevStartSceneShade()
    {
        shade.sizeDelta = new Vector2(1000, 1000);
        shade.GetComponent<Image>().color = Color.clear;
        shade.gameObject.SetActive(true);

        bool tmpB = false;
        shade.GetComponent<Image>().DOColor(ColorConstants.WHITE, duration).OnComplete(() => tmpB = true);
        while (!tmpB) yield return null;
    }

    private void FinSceneShade()
    {
        shade.GetComponent<Image>().DOColor(Color.clear, duration).OnComplete(()=>shade.gameObject.SetActive(false));
    }

    private void RevFinSceneShade()
    {
        shade.DOSizeDelta(new Vector2(0, 0), duration).OnComplete(() => shade.gameObject.SetActive(false));
    }
}
