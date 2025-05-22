using UnityEngine;
using Unity.Netcode;

public class JumpPad : NetworkBehaviour
{
    public float upwardForce = 12f;
    public float forwardForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerMovement.IsOwner)
        {
            playerMovement.RequestJumpServerRpc(transform.forward * forwardForce + Vector3.up * upwardForce);
        }
    }

    [ServerRpc]
    private void RequestJumpServerRpc(ulong playerId)
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerId];
        if (playerObject != null)
        {
            ApplyJumpPad(playerObject);
        }
    }

    private void ApplyJumpPad(NetworkObject playerObject)
    {
        var playerMovement = playerObject.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            Vector3 launchDirection = transform.forward * forwardForce + Vector3.up * upwardForce;
            playerMovement.ApplyJumpPadClientRpc(launchDirection);
        }
    }
}