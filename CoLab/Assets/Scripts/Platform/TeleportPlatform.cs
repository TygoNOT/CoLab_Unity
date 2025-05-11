using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class TeleportPlatform : NetworkBehaviour
{
    private List<ulong> playersOnPlatform = new List<ulong>();

    [SerializeField] private string nextSceneName;
    [SerializeField] private TextMesh playerCountText;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        playerCount.OnValueChanged += OnPlayerCountChanged;
        UpdateUI(playerCount.Value);
    }

    private void OnDestroy()
    {
        playerCount.OnValueChanged -= OnPlayerCountChanged;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && !playersOnPlatform.Contains(networkObject.OwnerClientId))
        {
            playersOnPlatform.Add(networkObject.OwnerClientId);
            playerCount.Value = playersOnPlatform.Count;
            CheckPlayersOnPlatform();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        var networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && playersOnPlatform.Contains(networkObject.OwnerClientId))
        {
            playersOnPlatform.Remove(networkObject.OwnerClientId);
            playerCount.Value = playersOnPlatform.Count;
        }
    }

    private void CheckPlayersOnPlatform()
    {
        if (playersOnPlatform.Count == 2)
        {
            GameManager.Instance.TeleportPlayersToScene(playersOnPlatform.ToArray(), nextSceneName);
            playersOnPlatform.Clear();
            playerCount.Value = 0;
        }
    }

    private void OnPlayerCountChanged(int oldValue, int newValue)
    {
        UpdateUI(newValue);
    }

    private void UpdateUI(int count)
    {
        if (playerCountText != null)
        {
            playerCountText.text = $"Player : {count}/2";
        }
    }
}