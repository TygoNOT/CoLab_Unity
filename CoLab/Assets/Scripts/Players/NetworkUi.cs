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
    [SerializeField]  public TMP_InputField ipInputField;
    [SerializeField]  public TMP_InputField portInputField;
    private NetworkVariable<int> playersNumber = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);


    public void HostButtonClick()
    {
        string ip = ipInputField.text;
        ushort.TryParse(portInputField.text, out ushort customPort);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, customPort);

        NetworkManager.Singleton.StartHost();
        ButtonHolder.SetActive(false);
        Players.SetActive(true);
    }

    public void ClientButtonClick()
    {
        string ip = ipInputField.text;
        ushort.TryParse(portInputField.text, out ushort customPort);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, customPort);

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