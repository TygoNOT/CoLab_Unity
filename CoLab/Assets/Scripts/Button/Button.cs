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
        {
            audioSource = buttonCube.gameObject.AddComponent<AudioSource>();
        }

        if (pressSound != null)
        {
            audioSource.clip = pressSound;
        }
    }

    private void Update()
    {
        if (IsOwner && !isPressed)
        {
            if (Vector3.Distance(transform.position, PlayerMovement.LocalInstance.transform.position) <= interactRadius)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PressButton();
                }
            }
        }
    }

    public void PressButton()
    {
        bool isCorrect = GameManager.Instance.CheckButtonSequence(buttonIndex);
        if (isCorrect)
        {
            buttonRenderer.material.color = correctColor;
            isPressed = true;
        }
        else
        {
            buttonRenderer.material.color = incorrectColor;
            if (incorrectPressSound != null)
            {
                audioSource.clip = incorrectPressSound;  
                audioSource.Play(); 
            }
            StartCoroutine(ResetButtonColor());
            GameManager.Instance.ResetButtonSequence();
            ButtonManager.Instance.ResetButtons();
        }

        if (pressSound != null && isCorrect)
        {
            audioSource.clip = pressSound;
            audioSource.Play();
        }
    }

    private IEnumerator ResetButtonColor()
    {
        yield return new WaitForSeconds(1.5f);
        buttonRenderer.material.color = defaultColor;  
        ResetPressed();  
    }

    public void ResetPressed()  
    {
        isPressed = false;
    }
}