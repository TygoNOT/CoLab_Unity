using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Colors;

public class ColorButton : MonoBehaviour
{
    public DoorColor buttonColor;
    private bool isPressed;
    private List<ColoredDoor> connectedDoors = new List<ColoredDoor>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = true;
            ToggleDoors(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = false;
            ToggleDoors(false);
        }
    }

    public void RegisterDoor(ColoredDoor door)
    {
        connectedDoors.Add(door);
    }

    private void ToggleDoors(bool open)
    {
        foreach (var door in connectedDoors)
        {
            door.SetOpen(open);
        }
    }
}
