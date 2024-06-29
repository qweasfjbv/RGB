using System.Collections;
using System.Collections.Generic;
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


    // TODO : 게임 시작 시 (임시 : Enter) -> 레벨 저장 및 맵 생성 함수 호출
    // 씬이동도 해야됨
    public void GameStart(int lvId)
    {
        StartCoroutine(GameStartCoroutine(lvId));

        return;
    }
   
    public IEnumerator GameStartCoroutine(int idx)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(Constant.GAME_SCENE);

        yield return async;
        yield return new WaitForSeconds(0.5f);

        MapGenerator.Instance.TestInit(idx);

    }

    // TODO : (임시 : ESC)-> 게임 재시작

    
    // TODO : (

}
