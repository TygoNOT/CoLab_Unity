using Unity.Netcode;
using UnityEngine;
using Cinemachine;
public class CameraController : NetworkBehaviour
{
    [Header("Attributes")]
    [SerializeField] private CinemachineVirtualCamera vc;
    [SerializeField] private AudioListener listener;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            vc.Priority = 1;
        } else
        {
            vc.Priority = 0;
        }
    }
}