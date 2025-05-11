using UnityEngine;

public class SymbolRotator : MonoBehaviour
{
    public string[] symbols;
    private int currentIndex = 0;

    public Mesh[] symbolMeshes;
    public MeshFilter meshFilter;

    public void RotateNext()
    {
        currentIndex = (currentIndex + 1) % symbols.Length;
        UpdateVisual();
    }

    public void RotatePrevious()
    {
        currentIndex = (currentIndex - 1 + symbols.Length) % symbols.Length;
        UpdateVisual();
    }

    public string GetCurrentSymbol()
    {
        return symbols[currentIndex];
    }

    private void UpdateVisual()
    {
        if (meshFilter != null && symbolMeshes.Length > currentIndex)
        {
            meshFilter.mesh = symbolMeshes[currentIndex];
        }
    }
}