using Fusion;
using TMPro;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public GameObject tmp;
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            tmp = Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity).gameObject;
            tmp.GetComponent<BoxController>().SetBoxController(new Vector2Int(0, 0), 0);

            if (Runner.IsSharedModeMasterClient) CreateMap();
        }

    }

    private void CreateMap()
    {
        MapGenerator.Instance.GenerateMap(1, GetComponent<NetworkRunner>());
    }


}