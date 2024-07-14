using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined{


    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            MapRestart();
        }
    }


    public GameObject PlayerPrefab;

    public NetworkObject currentPlayer;
    public int idx;

    public void MapRestart()
    {
        GameManagerEx.Instance.spawner = this;
        MapGenerator.Instance.SetStageName(GameManagerEx.Instance.CurLv);

        if (currentPlayer != null)
        {
            Runner.Despawn(currentPlayer);
        }

        if (Runner.IsSharedModeMasterClient)
            CreateMap();
        PlayerSpawn();

    }

    public void PlayerSpawn()
    {

        currentPlayer = Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        currentPlayer.GetComponent<BoxController>().SetBoxController(new Vector2Int(0, 0), 0);

    }

    public void CreateMap()
    {
        MapGenerator.Instance.GenerateMap(GameManagerEx.Instance.CurLv, Runner);
    }

}

