using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public List<int> buttonSequence = new List<int>();  
    public List<int> playerSequence = new List<int>();  
    private int currentButtonIndex = 0;

    public GameObject objectToReveal;

    [Header("Teleport Settings")]
    public bool teleportOnCorrectSequence = true; 
    public Transform spawnCenter; 
    public float spawnRadius = 5f;

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
            if (sceneEvent.SceneEventType == Unity.Netcode.SceneEventType.LoadComplete)
            {
                foreach (var clientId in clientIds)
                {
                    if (NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
                    {
                        var playerObj = client.PlayerObject;

                        if (playerObj != null)
                        {
                            var controller = playerObj.GetComponent<CharacterController>();
                            if (controller != null)
                                controller.enabled = false;

                            playerObj.transform.position = GetSpawnPosition();

                            if (controller != null)
                                controller.enabled = true;
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
        foreach (var clientPair in NetworkManager.ConnectedClients)
        {
            var clientId = clientPair.Key;
            var playerObject = clientPair.Value.PlayerObject;

            if (playerObject != null)
            {
                var controller = playerObject.GetComponent<CharacterController>();
                if (controller != null)
                    controller.enabled = false;

                Vector3 teleportPosition = spawnCenter != null
                    ? spawnCenter.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius))
                    : GetSpawnPosition(); 

                playerObject.transform.position = teleportPosition;

                if (controller != null)
                    controller.enabled = true;
            }
        }
    }

    public void ResetButtonSequence()
    {
        playerSequence.Clear();  
        currentButtonIndex = 0;  
        objectToReveal.SetActive(false);  
    }
}