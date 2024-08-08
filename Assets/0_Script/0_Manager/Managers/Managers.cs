using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{

    static Managers s_instance;
    public static Managers Instance { get { return s_instance; } }


    ResourceManager _resource = new ResourceManager();
    DataManager _data = new DataManager();
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static DataManager Data { get { return Instance._data; } }


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Screen.SetResolution(540, 960, FullScreenMode.Windowed);

        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        s_instance._resource.Init();
        s_instance._data.Init();

    }

}