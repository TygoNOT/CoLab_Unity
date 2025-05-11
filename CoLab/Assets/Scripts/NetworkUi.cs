using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class NetworkUi : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playersCountText;
    [SerializeField] private GameObject ButtonHolder;
    [SerializeField] private GameObject Players;

    private NetworkVariable<int> playersNumber = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            hostButton.enabled = false;
            clientButton.enabled = false;
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            clientButton.enabled = false;
            hostButton.enabled = false;
        });

    public void HostButtonClick()
    {
        NetworkManager.Singleton.StartHost();
        ButtonHolder.SetActive(false);
        Players.SetActive(true);
    }
    public void ClientButtonClick()
    {
        NetworkManager.Singleton.StartClient();
        ButtonHolder.SetActive(false);
        Players.SetActive(true);
    }

    private void Update()
    {
        playersCountText.text = "Players: " + playersNumber.Value.ToString();
        if (!IsServer)
        {
            return;
        }
        playersNumber.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}
