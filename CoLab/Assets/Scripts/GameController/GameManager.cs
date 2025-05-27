using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private bool ObjectToReveal = false;
    public List<int> buttonSequence = new List<int>();
    public List<int> playerSequence = new List<int>();
    private int currentButtonIndex = 0;

    public GameObject objectToReveal;

    [Header("Teleport Settings")]
    public bool teleportOnCorrectSequence = true;

    private List<Transform> spawnPoints = new List<Transform>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.OnLoadComplete += OnLoadComplete;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (IsServer)
        {
            // После того как все игроки загрузили сцену
            UpdateButtonSequenceForCurrentScene();
        }
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (IsServer && NetworkManager != null)
        {
            NetworkManager.SceneManager.OnLoadComplete -= OnLoadComplete;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (objectToReveal == null)
            objectToReveal = GameObject.FindWithTag("RevealObject");

        if (IsServer)
        {
            LoadSpawnPoints();
            UpdateButtonSequenceForCurrentScene();
            ObjectToRevealCheckServer(); // Хост скрывает объект
        }
        else
        {
            ObjectToRevealCheckClient(); // Клиенты тоже скрывают объект

        }
    }

    private void Start()
    {
        if (objectToReveal == null)
            objectToReveal = GameObject.FindWithTag("RevealObject");
        ObjectToRevealCheckServer();
        ObjectToRevealCheckClient();
    }

    public void ObjectToRevealCheckServer()
    {
        if (objectToReveal != null && ObjectToReveal)
        {
            objectToReveal.GetComponent<Renderer>().enabled = false;
            HideRevealObjectClientRpc();
        }
    }

    private void ObjectToRevealCheckClient()
    {
        if (objectToReveal != null && ObjectToReveal)
        {
            objectToReveal.GetComponent<Renderer>().enabled = false;
        }
    }

    [ClientRpc]
    private void HideRevealObjectClientRpc()
    {
        if (objectToReveal == null)
            objectToReveal = GameObject.FindWithTag("RevealObject");

        if (objectToReveal != null)
            objectToReveal.GetComponent<Renderer>().enabled = false;
    }

    [ClientRpc]
    private void RevealObjectClientRpc()
    {
        if (objectToReveal == null)
            objectToReveal = GameObject.FindWithTag("RevealObject");

        if (objectToReveal != null)
            objectToReveal.GetComponent<Renderer>().enabled = true;
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

        Debug.Log("Generated Button Sequence (Host): " + string.Join(", ", buttonSequence));
    }

    private void UpdateButtonSequenceForCurrentScene()
    {
        var buttons = FindObjectsOfType<Button>();

        // Назначаем индекс сервера
        ButtonManager.Instance.ClearAndReassignButtons(buttons);

        Dictionary<int, ulong> buttonIdMap = new Dictionary<int, ulong>();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].buttonIndex = i;
            buttonIdMap[i] = buttons[i].GetComponent<NetworkObject>().NetworkObjectId;
        }

        // Рассылаем клиентам, какие кнопки какие индексы
        SendButtonIndicesClientRpc(buttonIdMap.Keys.ToArray(), buttonIdMap.Values.ToArray());

        // Генерируем случайную последовательность
        buttonSequence = buttonIdMap.Keys.OrderBy(x => Random.value).ToList();

        Debug.Log("Updated Button Sequence: " + string.Join(", ", buttonSequence));

        playerSequence.Clear();
        currentButtonIndex = 0;

        SendSequenceToClientsClientRpc(buttonSequence.ToArray());
    }

    [ClientRpc]
    private void SendButtonIndicesClientRpc(int[] indices, ulong[] networkIds)
    {
        StartCoroutine(AssignIndicesWhenReady(indices, networkIds));
    }

    private IEnumerator AssignIndicesWhenReady(int[] indices, ulong[] networkIds)
    {
        while (ButtonManager.Instance == null)
            yield return null;

        Dictionary<int, Button> indexToButton = new();

        for (int i = 0; i < indices.Length; i++)
        {
            var netObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkIds[i]];
            if (netObj.TryGetComponent<Button>(out var button))
            {
                indexToButton[indices[i]] = button;
            }
        }

        ButtonManager.Instance.AssignButtonIndicesFromServer(indexToButton);
    }

    [ClientRpc]
    private void SendSequenceToClientsClientRpc(int[] sequence)
    {
        StartCoroutine(WaitAndApplySequence(sequence));

    }
    private IEnumerator WaitAndApplySequence(int[] sequence)
    {
        // Ждём, пока ButtonManager и кнопки инициализируются
        while (ButtonManager.Instance == null || ButtonManager.Instance.GetButtonByIndex(0) == null)
        {
            yield return null;
        }

        buttonSequence = new List<int>(sequence);
        Debug.Log("[ClientRpc] Received sequence: " + string.Join(", ", sequence));
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckButtonServerRpc(int buttonIndex, ulong senderClientId)
    {
        bool isCorrect = buttonIndex == buttonSequence[currentButtonIndex];
        Debug.Log($"[ServerRpc] Player {senderClientId} pressed {buttonIndex}. Expected: {buttonSequence[currentButtonIndex]}");

        if (isCorrect)
        {
            playerSequence.Add(buttonIndex);
            currentButtonIndex++;

            if (playerSequence.Count == buttonSequence.Count)
            {
                if (objectToReveal != null)
                    objectToReveal.GetComponent<Renderer>().enabled = true;
                
                RevealObjectClientRpc();

                if (teleportOnCorrectSequence)
                {
                    TeleportAllPlayersServerRpc();
                    RevealObjectClientRpc();

                }
            }

            UpdateButtonClientRpc(buttonIndex, true);
        }
        else
        {
            playerSequence.Clear();
            currentButtonIndex = 0;

            UpdateButtonClientRpc(buttonIndex, false);
            ButtonManager.Instance.ResetButtonsClientRpc();
        }
    }

    [ClientRpc]
    private void UpdateButtonClientRpc(int buttonIndex, bool correct)
    {
        var button = ButtonManager.Instance.GetButtonByIndex(buttonIndex);
        if (button != null)
        {
            button.SetState(correct);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TeleportAllPlayersServerRpc()
    {
        ulong[] allClients = NetworkManager.ConnectedClientsIds.ToArray();
        TeleportPlayersToScene(allClients, SceneManager.GetActiveScene().name);
    }

    public void TeleportPlayersToScene(ulong[] clientIds, string sceneName)
    {
        if (!IsServer) return;

        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        StartCoroutine(WaitAndTeleport(clientIds));
    }

    private IEnumerator WaitAndTeleport(ulong[] clientIds)
    {
        yield return new WaitForSeconds(0.5f); // Подождать, пока сцена загрузится
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

                    var controller = playerObj.GetComponent<CharacterController>();
                    if (controller != null) controller.enabled = false;

                    playerObj.transform.position = teleportPosition;

                    if (controller != null) controller.enabled = true;

                    TeleportClientRpc(teleportPosition, clientId);
                    SceneManager.sceneLoaded += OnSceneLoaded;

                }
            }
        }
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

    private Vector3 GetSpawnPosition()
    {
        return new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
    }
}