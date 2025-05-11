using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DoorColors;

public class NewColoredDoor : MonoBehaviour
{
    public NewDoorColors doorColor;
    private Collider col;
    private Renderer rend;

    private void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
    }

    public void SetOpen(bool isOpen)
    {
        if (col != null) col.enabled = !isOpen;
        if (rend != null) rend.enabled = !isOpen;
    }
}
