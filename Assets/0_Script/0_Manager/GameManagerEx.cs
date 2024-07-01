using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void GameStart(int lvId)
    {
        currentLv = lvId;
        StartCoroutine(GameStartCoroutine(lvId));

        return;
    }

    public void GameFail()
    {
        MapGenerator.Instance.ResetAndInit(currentLv);
    }

    public void GameSuccess()
    {
        // TODO : DataManager에 접근 필요
        MapGenerator.Instance.ResetAndInit(++currentLv);
    }


    public IEnumerator GameStartCoroutine(int idx)
    {
        // LoadScene and wait-> MapGenerate
        AsyncOperation async = SceneManager.LoadSceneAsync(Constant.GAME_SCENE);

        yield return async;
        yield return new WaitForSeconds(0.5f);

        MapGenerator.Instance.GenerateMap(idx);

    }
    
}
