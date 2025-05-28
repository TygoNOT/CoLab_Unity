using UnityEngine;
using System.Collections.Generic;
using static Colors;

public class PuzzleManager : MonoBehaviour
{
    [Header("References")]
    public ColoredDoor[] doorsInCorridor; // Pre-placed in the Inspector
    public ColorButton[] buttons;         // Pre-placed buttons
    public Collider hideTriggerZone;      // Trigger that hides door colors
    public Material[] colorMaterials;     // Indexed by DoorColor enum

    private void Start()
    {
        AssignRandomColors();
        ConnectButtons();
        if (hideTriggerZone.TryGetComponent(out DoorColorHider hider))
            hider.SetDoors(new List<ColoredDoor>(doorsInCorridor));
    }

    private void AssignRandomColors()
    {
        foreach (var door in doorsInCorridor)
        {
            DoorColor randomColor = (DoorColor)Random.Range(0, colorMaterials.Length);
            door.SetColor(randomColor, colorMaterials[(int)randomColor]);
        }
    }

    private void ConnectButtons()
    {
        foreach (var button in buttons)
        {
            foreach (var door in doorsInCorridor)
            {
                if (door.doorColor == button.buttonColor)
                    button.RegisterDoor(door);
            }
        }
    }
}
