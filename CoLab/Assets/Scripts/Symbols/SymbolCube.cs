using UnityEngine;

public class SymbolCube : MonoBehaviour
{
    public Material currentMaterial;
    public Material correctMaterial;
    public int currentMaterialIndex;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // ������������� ������ � ��������
    public void SetSymbol(Material mat)
    {
        currentMaterial = mat;
        rend.material = mat;
    }

    // ������������� ���������� �������� ��� ��������
    public void SetCorrectMaterial(Material mat)
    {
        correctMaterial = mat;
        currentMaterialIndex = System.Array.IndexOf(allMaterials, correctMaterial);
    }

    // ��������, ��������� �� ���������� ��������
    public bool IsCorrect()
    {
        return currentMaterial == correctMaterial;
    }

    // ��������� ������ ���� ��������� ����������
    public void SetAllMaterials(Material[] materials)
    {
        allMaterials = materials;
    }

    // ��� ������������ �������
    public void NextSymbol()
    {
        if (allMaterials == null || allMaterials.Length == 0) return;

        currentMaterialIndex = (currentMaterialIndex + 1) % allMaterials.Length;
        SetSymbol(allMaterials[currentMaterialIndex]);
    }

    private Material[] allMaterials;
}