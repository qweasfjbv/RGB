using Fusion;
using TMPro;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined {


    public GameObject PlayerPrefab;

    public NetworkObject currentPlayer;
    public int idx;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            if (Runner.IsSharedModeMasterClient) CreateMap();

            currentPlayer = Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            currentPlayer.GetComponent<BoxController>().SetBoxController(new Vector2Int(0, 0), 0);

        }
    }

    private void CreateMap()
    {
        MapGenerator.Instance.GenerateMap(1, GetComponent<NetworkRunner>());
    }

    public void MapRestart()
    {
        if (currentPlayer != null)
        {
            Debug.Log(currentPlayer.name);
            Runner.Despawn(currentPlayer);
        }

        if (Runner.IsSharedModeMasterClient) 
            CreateMap();

        currentPlayer = Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        currentPlayer.GetComponent<BoxController>().SetBoxController(new Vector2Int(0, 0), 0);

    }


}

