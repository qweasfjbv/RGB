using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{

    [SerializeField] private GameObject mapManagerPrefab;

    const int MAX_PLAYERS = 2;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(mapManagerPrefab);
            MapRestart();
        }

        if (GameManagerEx.Instance.CurGameType == GameType.MULTI)
        {
            if (Runner.ActivePlayers.Count() == MAX_PLAYERS)
            {
                if (Runner.IsSharedModeMasterClient)
                {
                    Runner.SessionInfo.IsOpen = false;
                    Runner.SessionInfo.IsVisible = false;
                }

                GameManagerEx.Instance.OnRoomFull();
            }
            else   // Waiting for someone...
            {
                GameManagerEx.Instance.OnWaiting();
            }
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {

        }
        else
        {
            GameManagerEx.Instance.GameEnd();
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

        if (GameManagerEx.Instance.CurGameType == GameType.MULTI && Runner.IsSharedModeMasterClient || GameManagerEx.Instance.CurGameType != GameType.MULTI)
            StartCoroutine(CreateMap());


        MapGenerator.Instance.SetStageName(GameManagerEx.Instance.CurGameType, GameManagerEx.Instance.CurLv, Runner);
        CameraController.Instance.SetQuaterView(Managers.Resource.GetCamPos(GameManagerEx.Instance.CurGameType, GameManagerEx.Instance.CurLv));


        PlayerSpawn();

    }

    public void PlayerSpawn()
    {

        currentPlayer.Add(Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        currentPlayer[0].GetComponent<BoxController>().SetBoxController(new Vector2Int(0, 0), 0);

        if (GameManagerEx.Instance.CurGameType == GameType.STAGE) // TUTO -> Set in TutorialManager after popup erase
            BoxController.UnlockInputBlock();                       // Multi -> Set in OnRoomFull

        if (GameManagerEx.Instance.CurGameType != GameType.MULTI)
            GameManagerEx.Instance.FinSceneShade();
    }

    public IEnumerator CreateMap()
    {
        while (!MapGenerator.Instance.IsSpawned)
        {
            yield return null;
        }
        MapGenerator.Instance.GenerateMap(GameManagerEx.Instance.CurGameType, GameManagerEx.Instance.CurLv, Runner);
    }



}

