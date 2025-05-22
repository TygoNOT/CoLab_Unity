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
    [SerializeField] public TMP_InputField ipInputField;
    [SerializeField] public TMP_InputField portInputField;
    [SerializeField] private TextMeshProUGUI timerText;
    public float TimerValue => timer.Value;
    private NetworkVariable<int> playersNumber = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> timer = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone); // таймер теперь идёт вперёд

    private void Update()
    {
        if (!Players.activeSelf)
            return;
        playersCountText.text = "Players: " + playersNumber.Value.ToString();

        int minutes = Mathf.FloorToInt(timer.Value / 60f);
        int seconds = Mathf.FloorToInt(timer.Value % 60f);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";

        if (!IsServer)
            return;

        playersNumber.Value = NetworkManager.Singleton.ConnectedClients.Count;

        timer.Value += Time.deltaTime;
    }

    public void HostButtonClick()
    {
        string ip = ipInputField.text;
        ushort.TryParse(portInputField.text, out ushort customPort);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, customPort);

        NetworkManager.Singleton.StartHost();
        ButtonHolder.SetActive(false);
        Players.SetActive(true);
        timer.Value = 0f;
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
}