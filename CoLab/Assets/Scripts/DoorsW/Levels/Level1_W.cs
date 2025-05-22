using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_W : MonoBehaviour
{
    [SerializeField] public List<PuzzleButton> allButtons;
    [SerializeField] public Door exitDoor;

    [SerializeField] private string buttonParentTag = "ButtonGroup";

    private PuzzleButton correctButton;

    void Start()
    {
        GameObject[] parents = GameObject.FindGameObjectsWithTag(buttonParentTag);

        int totalFound = 0;

        foreach (GameObject parent in parents)
        {
            PuzzleButton[] foundButtons = parent.GetComponentsInChildren<PuzzleButton>(true);
            foreach (PuzzleButton button in foundButtons)
            {
                if (!allButtons.Contains(button))
                {
                    allButtons.Add(button);
                    totalFound++;
                }
            }
        }

        Debug.Log($"[Level1_W] Всего добавлено кнопок: {totalFound}. Всего в списке: {allButtons.Count}");

        if (allButtons.Count == 0 || exitDoor == null)
        {
            Debug.LogError("OneTrueButtonRoom is not configured correctly.");
            return;
        }

        int index = Random.Range(0, allButtons.Count);
        correctButton = allButtons[index];
        correctButton.SetAsCorrectButton(exitDoor);
        Debug.Log(correctButton.name);
    }
}