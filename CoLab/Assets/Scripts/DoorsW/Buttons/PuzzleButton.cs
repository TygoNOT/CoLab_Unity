using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleButton : MonoBehaviour
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
                if (linkedDoor != null)
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
                if (linkedDoor != null)
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
        linkedDoor = door;
        isCorrectButton = true;
        isActiveButton = true;
    }


}
