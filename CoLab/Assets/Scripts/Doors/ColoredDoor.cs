using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Colors;

public class ColoredDoor : MonoBehaviour
{
    public DoorColor doorColor;
    private Renderer rend;
    private Collider col;
    private Material originalMaterial;
    [SerializeField] private Material invisibleMaterial;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        originalMaterial = rend.material;
    }

    public void SetOpen(bool isOpen)
    {
        if (this == null || col == null || rend == null) return;

        col.enabled = !isOpen;  // Disable collider to let player pass
        rend.enabled = !isOpen; // Hide mesh if desired (optional)
    }

    public void HideColor()
    {
        if (this == null || rend == null)
            return;

        if (invisibleMaterial != null)
        {
            rend.material = invisibleMaterial;
        }
    }

    public void RestoreColor()
    {
        if (this == null || rend == null)
            return;

        rend.material = originalMaterial;
    }
}
