using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_W : MonoBehaviour
{
    [SerializeField] public List<PuzzleButton> allButtons;
    [SerializeField] public Door exitDoor;

    private PuzzleButton correctButton;

    void Start()
    {
        if (allButtons.Count == 0 || exitDoor == null)
        {
            Debug.LogError("OneTrueButtonRoom is not configured correctly.");
            return;
        }

        // Randomly pick the correct button
        int index = Random.Range(0, allButtons.Count);
        correctButton = allButtons[index];
        correctButton.SetAsCorrectButton(exitDoor);
    }
}
