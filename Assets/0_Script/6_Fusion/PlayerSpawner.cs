using Fusion;
using TMPro;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public TextMeshProUGUI TMP;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            TMP.text = Runner.SessionInfo.Name;
        }
    }
}