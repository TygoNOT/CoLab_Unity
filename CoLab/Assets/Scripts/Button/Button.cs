using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Button : NetworkBehaviour
{
    public int buttonIndex;
    private Renderer buttonRenderer;
    public bool isPressed = false;

    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color defaultColor = Color.gray;

    public Transform buttonCube;
    public AudioClip pressSound;
    private AudioSource audioSource;
    public AudioClip incorrectPressSound;
    public float interactRadius = 2f;

    private void Start()
    {
        Transform buttonSphere = buttonCube.transform.GetChild(0);
        buttonRenderer = buttonSphere.GetComponent<Renderer>();
        buttonRenderer.material.color = defaultColor;

        audioSource = buttonCube.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = buttonCube.gameObject.AddComponent<AudioSource>();

        if (pressSound != null)
            audioSource.clip = pressSound;
    }

    private void Update()
    {
        if (isPressed) return;

        if (Vector3.Distance(transform.position, PlayerMovement.LocalInstance.transform.position) <= interactRadius)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckButtonRequestServerRpc(buttonIndex);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckButtonRequestServerRpc(int index, ServerRpcParams rpcParams = default)
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.CheckButtonServerRpc(index, rpcParams.Receive.SenderClientId);
    }

    public void SetState(bool correct)
    {
        if (correct)
        {
            isPressed = true;
            buttonRenderer.material.color = correctColor;
        }
        else
        {
            StartCoroutine(ShowIncorrectFeedback());
        }

        if (audioSource != null)
        {
            audioSource.clip = correct ? pressSound : incorrectPressSound;
            audioSource.Play();
        }
    }

    private IEnumerator ShowIncorrectFeedback()
    {
        buttonRenderer.material.color = incorrectColor;
        yield return new WaitForSeconds(1.5f);
        buttonRenderer.material.color = defaultColor;
        ResetPressed();
    }

    public void ResetPressed()
    {
        isPressed = false;
    }
}