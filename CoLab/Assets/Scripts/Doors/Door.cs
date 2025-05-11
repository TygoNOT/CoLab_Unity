using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject solidPart;
    [SerializeField] private Collider doorCollider;
    [SerializeField] private Material solidMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Renderer doorRenderer;

    public bool isOpen = false;

    public void Open()
    {
        SetOpen(true);
    }

    public void Close()
    {
        SetOpen(false);
    }

    public void SetOpen(bool isOpen)
    {
        if (doorCollider != null)
            doorCollider.enabled = !isOpen;

        if (doorRenderer != null)
            doorRenderer.material = isOpen ? transparentMaterial : solidMaterial;

        if (solidPart != null)
            solidPart.SetActive(!isOpen);
    }
}
