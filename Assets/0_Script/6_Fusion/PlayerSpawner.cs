using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined{

    [SerializeField] private GameObject mapManagerPrefab;



    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(mapManagerPrefab);
            MapRestart();
        }
    }


    public GameObject PlayerPrefab;

    public List<NetworkObject> currentPlayer;
    public int idx;

    public void MapRestart()
    {

        GameManagerEx.Instance.spawner = this;
        
        if (currentPlayer != null)
        {
            for (int i = 0; i < currentPlayer.Count; i++)
            {
                if (currentPlayer[i] != null) Runner.Despawn(currentPlayer[i]);
            }
        }
        currentPlayer.Clear();

        if (GameManagerEx.Instance.CurGameType== GameType.MULTI && Runner.IsSharedModeMasterClient || GameManagerEx.Instance.CurGameType!= GameType.MULTI)
            StartCoroutine(CreateMap());


        MapGenerator.Instance.SetStageName(GameManagerEx.Instance.CurGameType, GameManagerEx.Instance.CurLv, Runner);
        CameraController.Instance.SetQuaterView(Managers.Resource.GetCamPos(GameManagerEx.Instance.CurGameType, GameManagerEx.Instance.CurLv));


        PlayerSpawn();

    }

    public void PlayerSpawn()
    {

        currentPlayer.Add(Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        currentPlayer[0].GetComponent<BoxController>().SetBoxController(new Vector2Int(0, 0), 0);

        //currentPlayer.Add(Runner.Spawn(PlayerPrefab, new Vector3(4, 0, 4) * Constant.GRID_SIZE, Quaternion.identity));
        //currentPlayer[1].GetComponent<BoxController>().SetBoxController(new Vector2Int(4, 4), 0);

        if (GameManagerEx.Instance.CurGameType != GameType.TUTO) // TUTO -> Set in TutorialManager after popup erase
            BoxController.UnlockInputBlock();

        GameManagerEx.Instance.FinSceneShade();
    }

    public IEnumerator CreateMap()
    {
        while (!MapGenerator.Instance.IsSpawned)
        {
            Debug.Log("Waiting");
            yield return null;
        }
        MapGenerator.Instance.GenerateMap(GameManagerEx.Instance.CurGameType, GameManagerEx.Instance.CurLv, Runner);
    }

}

