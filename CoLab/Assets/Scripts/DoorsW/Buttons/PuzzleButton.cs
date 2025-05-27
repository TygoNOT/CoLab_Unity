using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class PuzzleButton : NetworkBehaviour
{
    [SerializeField] public Door linkedDoor;
    [SerializeField] public Renderer buttonRenderer;
    [SerializeField] public Color defaultColor = Color.gray;
    [SerializeField] public Color glowColor = Color.red;
    public bool isCorrectButton = false;
    public bool isActiveButton = false;

    public int playersOnButton = 0;

    private void Start()
    {
        SetGlow(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersOnButton++;
            if (playersOnButton == 1)
            {
                Debug.Log($"[PuzzleButton] Player stepped on button '{gameObject.name}'. Correct: {isCorrectButton}");

                if (isCorrectButton && linkedDoor != null)
                {
                    linkedDoor.SetOpen(true);
                }
                SetGlow(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersOnButton--;
            if (playersOnButton <= 0)
            {
                if (isCorrectButton && linkedDoor != null)
                {
                    linkedDoor.SetOpen(false);
                }
                SetGlow(false);
            }
        }
    }


    private void SetGlow(bool active)
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = active ? glowColor : defaultColor;
        }
    }

    public void SetAsCorrectButton(Door door)
    {
        if (isCorrectButton) return; // Avoid setting twice

        linkedDoor = door;
        isCorrectButton = true;
        isActiveButton = true;
        Debug.Log($"[PuzzleButton] {gameObject.name} marked as correct button. Door: {door?.gameObject.name}");
    }


}
