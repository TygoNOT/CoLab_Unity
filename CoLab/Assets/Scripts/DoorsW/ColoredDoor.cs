using UnityEngine;
using static Colors;

public class ColoredDoor : MonoBehaviour
{
    public DoorColor doorColor;
    [SerializeField] private Material hiddenMaterial;

    private Renderer rend;
    private Collider col;
    private Material originalMaterial;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    public void SetColor(DoorColor color, Material mat)
    {
        doorColor = color;
        originalMaterial = mat;
        rend.material = mat;
    }

    public void SetOpen(bool open)
    {
        col.enabled = !open;
        rend.enabled = !open;
    }

    public void HideColor()
    {
        if (hiddenMaterial != null)
            rend.material = hiddenMaterial;
    }

    public void RestoreColor()
    {
        if (originalMaterial != null)
            rend.material = originalMaterial;
    }
}
