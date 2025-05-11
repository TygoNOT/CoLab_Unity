using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public SymbolRotator[] rotators;
    public string[] correctCombination;

    public void CheckCombination()
    {
        for (int i = 0; i < rotators.Length; i++)
        {
            if (rotators[i].GetCurrentSymbol() != correctCombination[i])
            {
                return;
            }
        }

    }
}