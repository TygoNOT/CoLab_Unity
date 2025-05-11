using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Colors;
using static DoorColors;

public class NewColorButton : MonoBehaviour
{
    public NewDoorColors buttonColor;
    private static List<NewColoredDoor> allDoors;
    private int playersOnButton = 0;

    private void Awake()
    {
        if (allDoors == null)
            allDoors = new List<NewColoredDoor>(FindObjectsOfType<NewColoredDoor>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playersOnButton++;
        if (playersOnButton == 1)
        {
            ToggleDoors(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playersOnButton = Mathf.Max(playersOnButton - 1, 0);
        if (playersOnButton == 0)
        {
            ToggleDoors(false);
        }
    }

    private void ToggleDoors(bool open)
    {
        foreach (var door in allDoors)
        {
            if (door != null && door.doorColor == buttonColor)
                door.SetOpen(open);
        }
    }
}
