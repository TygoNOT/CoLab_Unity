using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorColorHider : MonoBehaviour
{
    private List<ColoredDoor> targetDoors;

    public void SetDoors(List<ColoredDoor> doors)
    {
        targetDoors = doors;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetDoors == null || targetDoors.Count == 0) return;

            foreach (ColoredDoor door in targetDoors)
            {
                door.HideColor();
            }
        }
    }
}
