using UnityEngine;

public class SymbolsManager : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    [SerializeField] private SymbolCube[] trueSymbols;
    [SerializeField] private SymbolCube[] changeableSymbols;

    private Material[] correctSequence;

    void Start()
    {
        GenerateSequence();
        ApplySequence();
    }
    public bool IsSequenceCorrect()
    {
        for (int i = 0; i < changeableSymbols.Length; i++)
        {
            // Логируем текущие индексы для проверки
            int correctMaterialIndex = System.Array.IndexOf(materials, changeableSymbols[i].correctMaterial);
            Debug.Log($"Символ {i}: {changeableSymbols[i].currentMaterialIndex} == {correctMaterialIndex}");

            if (changeableSymbols[i].currentMaterialIndex != correctMaterialIndex)
                return false;
        }
        return true;
    }
    void GenerateSequence()
    {
        correctSequence = new Material[trueSymbols.Length];
        for (int i = 0; i < correctSequence.Length; i++)
        {

            int randomIndex = Random.Range(0, materials.Length);
            correctSequence[i] = materials[randomIndex];
        }
    }
    public Material[] GetMaterials()
    {
        return materials;
    }
    void ApplySequence()
    {
        for (int i = 0; i < trueSymbols.Length; i++)
        {
            trueSymbols[i].SetAllMaterials(materials);
            trueSymbols[i].SetSymbol(correctSequence[i]);
            trueSymbols[i].SetCorrectMaterial(correctSequence[i]);
        }

        Material[] scrambledSequence = (Material[])correctSequence.Clone();
        Shuffle(scrambledSequence);

        for (int i = 0; i < changeableSymbols.Length; i++)
        {
            changeableSymbols[i].SetAllMaterials(materials);
            changeableSymbols[i].SetSymbol(scrambledSequence[i]);
            changeableSymbols[i].SetCorrectMaterial(correctSequence[i]);
        }
    }

    void Shuffle(Material[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rnd = Random.Range(i, array.Length);
            Material temp = array[i];
            array[i] = array[rnd];
            array[rnd] = temp;
        }
    }
}