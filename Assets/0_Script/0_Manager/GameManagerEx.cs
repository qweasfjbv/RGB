using DG.Tweening;
using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerEx : NetworkBehaviour, ISpawned
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

    public PlayerSpawner spawner;



    private int currentLv = -1;
    public int CurLv { get => currentLv; }

    private GameType curGameType;
    public GameType CurGameType { get => curGameType; set => curGameType = value; }

    [Header("Scene Conversion Effect")]
    [SerializeField] private float duration;
    [SerializeField] private RectTransform shade;


    [SerializeField] public bool IsColorBlind;

    private void Start()
    {
        shade.gameObject.SetActive(false);
        SoundManager.Instance.ChangeBGM(BGMClip.MAIN_BGM);
        IsColorBlind = false;
    }

    private void EraseNOs()
    {
        var runners = FindObjectsOfType<NetworkRunner>();
        foreach (var runner in runners)
        {
            Destroy(runner.gameObject);
        }

        var linkers = FindObjectsOfType<FusionBootstrap>();
        foreach (var runner in runners)
        {
            Destroy(runner.gameObject);
        }

    }

    public void GameStart(GameType type, int lvId)
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
        StartCoroutine(GameRestartCoroutine(currentLv));
    }

    public void GameSuccess()
    {
        if (currentLv == Managers.Resource.GetMapCount(curGameType))
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
        SoundManager.Instance.SilentBGM();
        yield return StartCoroutine(StartSceneShade());

        // LoadScene and wait-> MapGenerate
        AsyncOperation async = SceneManager.LoadSceneAsync(Constant.GAME_SCENE);
        yield return async;

        FusionNetworkManager linkers = FindObjectOfType<FusionNetworkManager>();
        if (curGameType == GameType.MULTI) linkers.StartSharedClient();
        else linkers.StartSinglePlay();

        SoundManager.Instance.ChangeBGM(BGMClip.GAME_BGM);

        FinSceneShade();

    }

    // Convert Scene Game -> MainMenu
    public IEnumerator GameEndCoroutine()
    {
        SoundManager.Instance.SilentBGM();
        yield return StartCoroutine(RevStartSceneShade());

        AsyncOperation async = SceneManager.LoadSceneAsync(Constant.MAIN_SCENE);
        yield return async;

        SoundManager.Instance.ChangeBGM(BGMClip.MAIN_BGM);

        CameraController.Instance.UnsetCamera();
        EraseNOs();
        RevFinSceneShade();

    }

    public IEnumerator GameRestartCoroutine(int idx)
    {
        yield return StartCoroutine(RevStartSceneShade());

        MapGenerator.Instance.EraseAllObject(spawner.Runner);
        // TODO : RESTART MAP;

        spawner.MapRestart();

        FinSceneShade();

    }
    // Scene Convert Eff
    
    private IEnumerator StartSceneShade()
    {

        SoundManager.Instance.CreateAudioSource(Vector3.zero, EffectClip.SHADE);
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
        SoundManager.Instance.CreateAudioSource(Vector3.zero, EffectClip.SHADE);
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
