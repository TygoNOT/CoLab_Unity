using UnityEngine;

public class SymbolCube : MonoBehaviour
{
    public Material currentMaterial;
    public Material correctMaterial;
    public int currentMaterialIndex;

    private Renderer rend;
    private Material[] allMaterials;

    [SerializeField] private Material[] fallbackMaterials;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        if ((allMaterials == null || allMaterials.Length == 0) && fallbackMaterials != null && fallbackMaterials.Length > 0)
        {
            allMaterials = fallbackMaterials;
            Debug.LogWarning($"{gameObject.name}: Using fallbackMaterials.");
        }

        if (rend == null)
        {
            Debug.LogError($"{gameObject.name}: Renderer component not found!");
        }
    }

    public void SetAllMaterials(Material[] materials)
    {
        allMaterials = materials;
    }

    public void SetSymbol(Material mat)
    {
        currentMaterial = mat;

        if (rend == null)
            rend = GetComponent<Renderer>();

        if (rend != null)
        {
            rend.sharedMaterial = mat; // Клонируем материал
            Debug.Log($"{gameObject.name}: Material set to {mat.name}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Renderer not found when setting material.");
        }
    }

    public void SetCorrectMaterial(Material mat)
    {
        correctMaterial = mat;

        if (allMaterials == null || allMaterials.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: allMaterials is empty when setting correctMaterial.");
            return;
        }

        currentMaterialIndex = System.Array.IndexOf(allMaterials, correctMaterial);
    }

    public bool IsCorrect()
    {
        return currentMaterial == correctMaterial;
    }

    public void NextSymbol()
    {
        if (allMaterials == null || allMaterials.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: allMaterials not set - cannot switch symbol.");
            return;
        }

        currentMaterialIndex = (currentMaterialIndex + 1) % allMaterials.Length;
        SetSymbol(allMaterials[currentMaterialIndex]);
    }
}