using UnityEngine;
using System.Collections.Generic;
using static Colors;

public class ColorButton : MonoBehaviour
{
    public DoorColor buttonColor;
    private List<ColoredDoor> linkedDoors = new();

    public void RegisterDoor(ColoredDoor door)
    {
        if (!linkedDoors.Contains(door))
            linkedDoors.Add(door);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ToggleDoors(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            ToggleDoors(false);
    }

    private void ToggleDoors(bool open)
    {
        foreach (var door in linkedDoors)
            door.SetOpen(open);
    }
}
