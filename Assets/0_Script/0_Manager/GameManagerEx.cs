using DG.Tweening;
using Fusion;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Localization.Settings;
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

    private string curSession;
    public string CurSession {  get => curSession; }

    private bool isInMultiGame = false;
    public bool IsInMultiGame { get { return isInMultiGame; }  set=>isInMultiGame = value; }

    private GameType curGameType;
    public GameType CurGameType { get => curGameType; set => curGameType = value; }

    [Header("Scene Conversion Effect")]
    [SerializeField] private float duration;
    [SerializeField] private RectTransform shade;

    [Header("Network")]
    [SerializeField] private GameObject _networkRunnerPrefab;
    [SerializeField] private GameObject waitPanel;
    [SerializeField] private Button cancelButton;

    [Header("TimerUIs")]
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private GameObject timerManagerPrefab;
    [SerializeField] private GameObject timerUI;
    [SerializeField] private TextMeshProUGUI timerText;

    private TimerManager timerManager;
    public NetworkRunner newRunner = null;

    private void Start()
    {
        shade.gameObject.SetActive(false);
        SoundManager.Instance.ChangeBGM(BGMClip.MAIN_BGM);
    }

    public void SetLocale(int idx)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[idx];
    }

    private void EraseNOs()
    {
        var runners = FindObjectsOfType<NetworkRunner>();
        foreach (var runner in runners)
        {
            Destroy(runner.gameObject);
        }
        newRunner = null;

        var linkers = FindObjectsOfType<FusionNetworkManager>();
        foreach (var linker in linkers)
        {
            Destroy(linker.gameObject);
        }

        if (timerManager != null)
            Destroy(timerManager);

    }

    // GameStart (Main->GameScene)
    public void GameStart(GameType type, int lvId, string sessionId="")
    {
        curGameType = type;
        currentLv = lvId;
        curSession = sessionId;
        StartCoroutine(GameStartCoroutine(lvId));

        return;
    }

    // GameEnd (Game->MainScene)
    public void GameEnd()
    {
        UnsetTimers();
        multiResultPanel.SetActive(false);
        StartCoroutine(GameEndCoroutine());

        return;
    }

    // GameFail (GameScene, same Lv)
    public void GameFail(bool isBlack = false)
    {
        if (isInMultiGame)
        {
            timerManager.RPC_QuickMultiResult(false, newRunner.LocalPlayer);
            return;
        }
        StartCoroutine(GameRestartCoroutine(currentLv, isBlack));
    }

    // GameFail (GameScene, next Lv)
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

        StartGame();

        SoundManager.Instance.ChangeBGM(BGMClip.GAME_BGM);

        if (curGameType != GameType.MULTI) 
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

    public IEnumerator GameRestartCoroutine(int idx, bool isBlack=false)
    {
        if (!isBlack)
        {
            yield return StartCoroutine(RevStartSceneShade());
        }
        else
        {
            yield return StartCoroutine(BlackRevStartSceneShade());
        }

        MapGenerator.Instance.EraseAllObject(spawner.Runner);
        spawner.LoadMap();

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
    private IEnumerator BlackRevStartSceneShade()
    {
        shade.GetComponent<Image>().color = ColorConstants.BLACK;
        SoundManager.Instance.CreateAudioSource(Vector3.zero, EffectClip.SHADE);
        shade.sizeDelta = Vector2.zero;
        shade.gameObject.SetActive(true);

        bool tmpB = false;
        shade.DOSizeDelta(new Vector2(1000, 1000), duration).OnComplete(() => tmpB = true);
        while (!tmpB) yield return null;
    }

    public void UnsetTimers() {
        if(timerManager != null)
        {
            timerManager.StopAllCoroutine();
        }
        counterText.gameObject.SetActive(false);
        timerUI.SetActive(false);
        scoreUI.SetActive(false);
    }


    public void FinSceneShade()
    {
        Color tmpColor = shade.GetComponent<Image>().color;
        tmpColor.a = 0f;
        shade.GetComponent<Image>().DOColor(tmpColor, duration).OnComplete(()=>shade.gameObject.SetActive(false));
    }

    private void RevFinSceneShade()
    {
        shade.DOSizeDelta(new Vector2(0, 0), duration).OnComplete(() => shade.gameObject.SetActive(false));
    }

    public async void StartGame()
    {

        newRunner = Instantiate(_networkRunnerPrefab).GetComponent<NetworkRunner>();

        var sceneManager = newRunner.GetComponent<INetworkSceneManager>();
        if (sceneManager == null)
        {
            Debug.Log($"NetworkRunner does not have any component implementing {nameof(INetworkSceneManager)} interface, adding {nameof(NetworkSceneManagerDefault)}.", newRunner);
            sceneManager = newRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        var objectProvider = newRunner.GetComponent<INetworkObjectProvider>();
        if (objectProvider == null)
        {
            Debug.Log($"NetworkRunner does not have any component implementing {nameof(INetworkObjectProvider)} interface, adding {nameof(NetworkObjectProviderDefault)}.", newRunner);
            objectProvider = newRunner.gameObject.AddComponent<NetworkObjectProviderDefault>();
        }

        var sceneInfo = new NetworkSceneInfo();


        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = (curGameType==GameType.MULTI ? GameMode.Shared : GameMode.Single),
            Scene = sceneInfo,
            SessionName = string.Empty,
            SceneManager = sceneManager,
            Updater = null,
            ObjectProvider = objectProvider,
        };

        StartGameResult result = await newRunner.StartGame(startGameArgs);

        if (result.Ok)
        {
            // StartGame
        }
        else
        {
            // Error Catch
        }

    }

    // Wait for someone...
    public void OnWaiting()
    {
        waitPanel.SetActive(true);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(GameEnd);
        cancelButton.onClick.AddListener(() => waitPanel.SetActive(false));
    }

    public void OnRoomFull()
    {
        waitPanel.SetActive(false);
        FinSceneShade();                // Fade Out Whiteboard

        if (newRunner.IsSharedModeMasterClient)
        {
            timerManager = newRunner.Spawn(timerManagerPrefab).GetComponent<TimerManager>();
            timerManager.StartCounter(5f);
        }
        
    }

    public void EndTimer()
    {
        counterText.gameObject.SetActive(false);

        if (newRunner.IsSharedModeMasterClient)
            timerManager.StartTimer(60f);
    }

    public void UpdateCounterUI(int sec, TimerManager timer)
    {
        if(timerManager == null) timerManager = timer;

        counterText.gameObject.SetActive(true);
        if (sec != 0)
        {
            counterText.text = sec.ToString();
        }
        else
        {
            counterText.text = "Start !";
            Invoke(nameof(EndTimer), 1f);
        }

    }

    public void UpdateTimerUI(int sec)
    {
        if (!isInMultiGame) return;


        timerUI.SetActive(true);
        scoreUI.SetActive(true);

        if (sec != 0)
        {
            timerText.text = sec.ToString();
        }
        else
        {
            PopupMultiResult(0, newRunner.LocalPlayer);
        }

    }

    public void OnStepBlack()
    {
        BoxController.RPC_LockInputBlock(); // Lock and Game End
        if (CurGameType == GameType.MULTI)
            timerManager.RPC_QuickMultiResult(true, newRunner.LocalPlayer);
        else
        {
            GameFail(true);
        }
    }

    public void AddMultiScore(int score)
    {
        if (curGameType != GameType.MULTI) return;

        if (timerManager == null)
        {
            timerManager = FindObjectOfType<TimerManager>();
        }
        timerManager.AddScore(newRunner.LocalPlayer, score);
    }


    [Header("ScoreUI")]
    [SerializeField] private GameObject scoreUI;
    [SerializeField] private TextMeshProUGUI scoreText1;
    [SerializeField] private TextMeshProUGUI scoreText2;

    // 1 : mine, 2 : opponent
    int maxScore1 = -1;
    int maxScore2 = -1;

    // Called in RPC
    public void UpdateScore(int target, int score)
    {
        switch (target) {
            case 1:
                scoreText1.text = score.ToString();
                maxScore1 = score;
                break;
            case 2:
                scoreText2.text = score.ToString();
                maxScore2 = score;
                break;
        }


    }


    [Header("MultiResult")]
    [SerializeField] private GameObject multiResultPanel;
    [SerializeField] private TextMeshProUGUI multiResultText;
    [SerializeField] private Button multiResultOkButton;

    private Color loseTextColor = new Color(207 / 255f, 14 / 255f, 0 / 255f, 255 / 255f);
    private Color winTextColor = new Color(5 / 255f, 192 / 255f, 5 / 255f, 255 / 255f);
    private Color blackTextColor = new Color(32 / 255f, 32 / 255f, 32 / 255f, 255 / 255f);

    // Should be called from all client
    public void PopupMultiResult(int isTrapped, PlayerRef player)
    {
        Debug.Log("SETACTIVE(TRU");
        BoxController.LockInputBlock();
        multiResultPanel.SetActive(true);

        SetResultText(isTrapped, player);

        multiResultOkButton.onClick.RemoveAllListeners();
        multiResultOkButton.onClick.AddListener(GameEnd);
    }

    // must be called from all client
    private void SetResultText(int isTrapped, PlayerRef player)
    {
        // 0: not trap, 1: fall, 2: blacked

        UnsetTimers();

        int isWin = 0;

        if (isTrapped == 0) {

            if (maxScore1 > maxScore2)
            {
                isWin = 1;
            }
            else if (maxScore1 < maxScore2)
            {
                isWin = -1;
            }
        }
        else
        {
            if (player == newRunner.LocalPlayer)
            {
                isWin = -1;
            }
            else isWin = 1;
        }





        switch (isWin) {
            case -1:
                multiResultText.text = (isTrapped==2) ? "Black" : "Lose";
                multiResultText.color = (isTrapped == 2) ? blackTextColor : loseTextColor;
                break;
            case 0:
                multiResultText.text = "Draw";
                multiResultText.color = loseTextColor;
                break;
            case 1:
                multiResultText.text = "Win";
                multiResultText.color = winTextColor;
                break;
        }

    }




}
