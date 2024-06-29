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


    public void GameStart(int lvId)
    {
        StartCoroutine(GameStartCoroutine(lvId));

        return;
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
