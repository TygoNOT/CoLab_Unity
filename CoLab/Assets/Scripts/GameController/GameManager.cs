using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public List<int> buttonSequence = new List<int>();
    public List<int> playerSequence = new List<int>();
    private int currentButtonIndex = 0;

    public GameObject objectToReveal;

    [Header("Teleport Settings")]
    public bool teleportOnCorrectSequence = true;

    private List<Transform> spawnPoints = new List<Transform>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        GenerateRandomSequence();
    }

    public void GenerateRandomSequence()
    {
        buttonSequence.Clear();
        List<int> allButtons = new List<int> { 0, 1, 2, 3, 4, 5 };

        while (allButtons.Count > 0)
        {
            int randomIndex = Random.Range(0, allButtons.Count);
            buttonSequence.Add(allButtons[randomIndex]);
            allButtons.RemoveAt(randomIndex);
        }

        Debug.Log("Generated Button Sequence: " + string.Join(", ", buttonSequence));
    }

    public void TeleportPlayersToScene(ulong[] clientIds, string sceneName)
    {
        if (!IsServer) return;

        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        NetworkManager.SceneManager.OnSceneEvent += (sceneEvent) =>
        {
            if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
            {
                LoadSpawnPoints();

                for (int i = 0; i < clientIds.Length; i++)
                {
                    var clientId = clientIds[i];

                    if (NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
                    {
                        var playerObj = client.PlayerObject;

                        if (playerObj != null)
                        {
                            Vector3 teleportPosition = spawnPoints.Count > i
                                ? spawnPoints[i].position
                                : GetSpawnPosition();

                            // Телепортируем на сервере
                            var controller = playerObj.GetComponent<CharacterController>();
                            if (controller != null) controller.enabled = false;

                            playerObj.transform.position = teleportPosition;

                            if (controller != null) controller.enabled = true;

                            // Отправляем клиенту позицию
                            TeleportClientRpc(teleportPosition, clientId);
                        }
                    }
                }
            }
        };
    }

    private Vector3 GetSpawnPosition()
    {
        return new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
    }

    [ClientRpc]
    private void TeleportClientRpc(Vector3 position, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (player != null)
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.position = position;

            if (controller != null) controller.enabled = true;
        }
    }

    private void LoadSpawnPoints()
    {
        spawnPoints.Clear();
        var allPoints = GameObject.FindObjectsOfType<SpawnPoint>();
        foreach (var point in allPoints)
        {
            spawnPoints.Add(point.transform);
        }

        spawnPoints = spawnPoints.OrderBy(p => p.GetComponent<SpawnPoint>().spawnIndex).ToList();

        Debug.Log($"Loaded {spawnPoints.Count} spawn points");
    }

    public bool CheckButtonSequence(int buttonIndex)
    {
        if (buttonIndex == buttonSequence[currentButtonIndex])
        {
            playerSequence.Add(buttonIndex);
            currentButtonIndex++;

            if (playerSequence.Count == buttonSequence.Count)
            {
                objectToReveal.SetActive(true);

                if (teleportOnCorrectSequence && IsServer)
                {
                    TeleportAllPlayersServerRpc();
                }

                return true;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TeleportAllPlayersServerRpc()
    {
        ulong[] allClients = NetworkManager.ConnectedClientsIds.ToArray();
        TeleportPlayersToScene(allClients, SceneManager.GetActiveScene().name);
    }

    public void ResetButtonSequence()
    {
        playerSequence.Clear();
        currentButtonIndex = 0;
        objectToReveal.SetActive(false);
    }
}