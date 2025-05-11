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

    // Устанавливаем символ и материал
    public void SetSymbol(Material mat)
    {
        currentMaterial = mat;
        rend.material = mat;
    }

    // Устанавливаем правильный материал для проверки
    public void SetCorrectMaterial(Material mat)
    {
        correctMaterial = mat;
        currentMaterialIndex = System.Array.IndexOf(allMaterials, correctMaterial);
    }

    // Проверка, правильно ли установлен материал
    public bool IsCorrect()
    {
        return currentMaterial == correctMaterial;
    }

    // Установим массив всех доступных материалов
    public void SetAllMaterials(Material[] materials)
    {
        allMaterials = materials;
    }

    // Для переключения символа
    public void NextSymbol()
    {
        if (allMaterials == null || allMaterials.Length == 0) return;

        currentMaterialIndex = (currentMaterialIndex + 1) % allMaterials.Length;
        SetSymbol(allMaterials[currentMaterialIndex]);
    }

    private Material[] allMaterials;
}