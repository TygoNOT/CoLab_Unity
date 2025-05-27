using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Level1_W : NetworkBehaviour
{
    [SerializeField] public List<PuzzleButton> allButtons;
    [SerializeField] public Door exitDoor;

    [SerializeField] private string buttonParentTag = "ButtonGroup";
    private PuzzleButton correctButton;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(DelayedInit()); // ensure everything has spawned
    }

    private IEnumerator DelayedInit()
    {
        // Wait 1 frame to make sure all NetworkObjects are spawned
        yield return null;

        Debug.Log("[Level1_W] DelayedInit running on server");

        GameObject[] parents = GameObject.FindGameObjectsWithTag(buttonParentTag);
        int totalFound = 0;

        foreach (GameObject parent in parents)
        {
            PuzzleButton[] foundButtons = parent.GetComponentsInChildren<PuzzleButton>(true);
            foreach (PuzzleButton button in foundButtons)
            {
                if (!allButtons.Contains(button))
                {
                    allButtons.Add(button);
                    totalFound++;
                }
            }
        }

        Debug.Log($"[Level1_W] Buttons added: {totalFound}. Total: {allButtons.Count}");

        if (allButtons.Count == 0 || exitDoor == null)
        {
            Debug.LogError("Level not set up correctly.");
            yield break;
        }

        int index = Random.Range(0, allButtons.Count);
        correctButton = allButtons[index];
        correctButton.SetAsCorrectButton(exitDoor);
        Debug.Log($"[Server] Correct button is: {correctButton.name}");

        // Inform clients
        SetCorrectButtonClientRpc(correctButton.NetworkObjectId, exitDoor.NetworkObjectId);
    }

    [ClientRpc]
    void SetCorrectButtonClientRpc(ulong buttonNetworkId, ulong doorNetworkId)
    {
        StartCoroutine(ApplyCorrectButtonAfterSpawn(buttonNetworkId, doorNetworkId));
    }

    private IEnumerator ApplyCorrectButtonAfterSpawn(ulong buttonId, ulong doorId)
    {
        int tries = 10;
        while (
            (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(buttonId) ||
             !NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(doorId)) &&
             tries-- > 0)
        {
            yield return null;
        }

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(buttonId, out var buttonObj) ||
            !NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(doorId, out var doorObj))
        {
            Debug.LogError("[ClientRpc] Could not find button or door even after waiting.");
            yield break;
        }

        var button = buttonObj.GetComponent<PuzzleButton>();
        var door = doorObj.GetComponent<Door>();

        if (button != null && door != null)
        {
            button.SetAsCorrectButton(door);
            Debug.Log($"[ClientRpc] Client applied correct button: {button.name}");
        }
        else
        {
            Debug.LogError("[ClientRpc] Missing components on button or door.");
        }
    }

}
