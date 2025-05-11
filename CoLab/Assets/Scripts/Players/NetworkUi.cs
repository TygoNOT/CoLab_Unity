using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class NetworkUi : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playersCountText;
    [SerializeField] private GameObject ButtonHolder;
    [SerializeField] private GameObject Players;

    private NetworkVariable<int> playersNumber = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    // IP-адрес хоста в сети Radmin
    private string hostIp = "26.187.193.230";
    private ushort port = 7777;

    public void HostButtonClick()
    {
        NetworkManager.Singleton.StartHost();
        ButtonHolder.SetActive(false);
        Players.SetActive(true);
    }

    public void ClientButtonClick()
    {
        // Устанавливаем адрес подключения вручную
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(hostIp, port);

        NetworkManager.Singleton.StartClient();
        ButtonHolder.SetActive(false);
        Players.SetActive(true);
    }

    private void Update()
    {
        playersCountText.text = "Players: " + playersNumber.Value.ToString();

        if (!IsServer)
            return;

        playersNumber.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}