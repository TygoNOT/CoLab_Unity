using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Unity.Collections;

public class SymbolsManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Material[] materials;
    [SerializeField] private SymbolCube[] trueSymbols;
    [SerializeField] private SymbolCube[] changeableSymbols;

    [Header("Sync")]
    private NetworkList<int> correctIndexes;
    private NetworkVariable<int> randomSeed = new NetworkVariable<int>();
    private NetworkVariable<bool> isInitialized = new NetworkVariable<bool>(false);

    private void Awake()
    {
        correctIndexes = new NetworkList<int>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InitializeServer();
        }

        randomSeed.OnValueChanged += OnSeedChanged;
        isInitialized.OnValueChanged += OnInitializedChanged;

        // ���� ������ ��� ������ �� ��������
        if (isInitialized.Value)
        {
            InitializeClient();
        }
    }

    private void InitializeServer()
    {
        // ���������� seed ������ ���� ���
        if (randomSeed.Value == 0)
        {
            randomSeed.Value = Random.Range(1, int.MaxValue);
        }

        // ���������� ������������������
        GenerateSequence(randomSeed.Value);
        isInitialized.Value = true;
    }

    private void OnSeedChanged(int oldSeed, int newSeed)
    {
        if (!IsServer && newSeed != 0)
        {
            Random.InitState(newSeed);
            GenerateSequence(newSeed);
        }
    }

    private void OnInitializedChanged(bool oldValue, bool newValue)
    {
        if (newValue && !IsServer)
        {
            InitializeClient();
        }
    }

    private void GenerateSequence(int seed)
    {
        Random.InitState(seed);
        correctIndexes.Clear();

        // ��������� ���������� ������������������
        for (int i = 0; i < trueSymbols.Length; i++)
        {
            correctIndexes.Add(Random.Range(0, materials.Length));
        }
    }

    private void InitializeClient()
    {
        StartCoroutine(ClientInitializeRoutine());
    }

    private IEnumerator ClientInitializeRoutine()
    {
        // ���� ���� ��� NetworkVariables ����������������
        yield return new WaitUntil(() => correctIndexes.Count == trueSymbols.Length);

        ApplyMaterials();
    }

    private void ApplyMaterials()
    {
        // 1. ��������� ���������� ���������
        for (int i = 0; i < trueSymbols.Length; i++)
        {
            if (i < correctIndexes.Count)
            {
                Material mat = materials[correctIndexes[i]];
                trueSymbols[i].SetAllMaterials(materials);
                trueSymbols[i].SetSymbol(mat);
                trueSymbols[i].SetCorrectMaterial(mat);
            }
        }

        // 2. ������������ � ��������� � ���������� ��������
        int[] shuffledIndexes = new int[correctIndexes.Count];
        for (int i = 0; i < correctIndexes.Count; i++)
        {
            shuffledIndexes[i] = correctIndexes[i];
        }

        // ������������ � ��� �� seed
        Random.InitState(randomSeed.Value);
        ShuffleArray(shuffledIndexes);

        for (int i = 0; i < changeableSymbols.Length; i++)
        {
            if (i < shuffledIndexes.Length)
            {
                Material mat = materials[shuffledIndexes[i]];
                Material correctMat = materials[correctIndexes[i]];
                changeableSymbols[i].SetAllMaterials(materials);
                changeableSymbols[i].SetSymbol(mat);
                changeableSymbols[i].SetCorrectMaterial(correctMat);
            }
        }
    }

    private void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    public Material[] GetMaterials()
    {
        return materials;
    }
    public bool IsSequenceCorrect()
    {
        // ���������, ��� ��� changeableSymbols ����� ���������� ���������
        for (int i = 0; i < changeableSymbols.Length; i++)
        {
            if (changeableSymbols[i] == null)
            {
                Debug.LogError($"Changeable symbol {i} is null!");
                return false;
            }

            // �������� ������� ������ ���������
            int currentIndex = changeableSymbols[i].currentMaterialIndex;

            // �������� ���������� ������ �� correctIndexes
            if (i >= correctIndexes.Count)
            {
                Debug.LogError($"Index {i} out of correctIndexes range!");
                return false;
            }
            int correctIndex = correctIndexes[i];

            // ���������� �������
            if (currentIndex != correctIndex)
            {
                Debug.Log($"Symbol {i} is incorrect. Current: {currentIndex}, Expected: {correctIndex}");
                return false;
            }
        }
        return true;
    }
}