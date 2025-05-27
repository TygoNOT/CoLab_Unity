using UnityEngine;
using Unity.Netcode;

public class ButtonTriggerVisible : NetworkBehaviour
{
    [SerializeField] private GameObject[] enableOnPress;
    [SerializeField] private GameObject[] disableOnPress;

    public AudioClip pressSound;
    private AudioSource audioSource;

    private bool playerInRange = false;
    private bool toggledState = false; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        foreach (var obj in enableOnPress)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PlayClickSound();
            ToggleObjectsServerRpc();
        }
    }

    private void PlayClickSound()
    {
        if (pressSound != null && audioSource != null)
            audioSource.PlayOneShot(pressSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleObjectsServerRpc()
    {
        toggledState = !toggledState;
        ToggleObjectsClientRpc(toggledState);
    }

    [ClientRpc]
    private void ToggleObjectsClientRpc(bool state)
    {
        foreach (var obj in enableOnPress)
        {
            if (obj != null)
                obj.SetActive(state);
        }

        foreach (var obj in disableOnPress)
        {
            if (obj != null)
                obj.SetActive(!state);
        }
    }
}
