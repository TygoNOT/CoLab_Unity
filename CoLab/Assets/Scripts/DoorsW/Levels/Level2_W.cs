using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Colors;

public class Level2_W : MonoBehaviour
{
    public GameObject doorPrefab;
    public Transform[] doorPositions;
    public List<ColoredDoor> allDoors = new List<ColoredDoor>();
    public ColorButton[] allButtons;
    public Material[] doorMaterials;

    [SerializeField] private DoorColorHider colorHider;

    private void Start()
    {
        GenerateDoors();
        ConnectDoorsToButtons();
        if (colorHider != null)
        {
            allDoors.RemoveAll(door => door == null);
            colorHider.SetDoors(allDoors);
        }
    }

    void GenerateDoors()
    {
        foreach (Transform pos in doorPositions)
        {
            GameObject doorObj = Instantiate(doorPrefab, pos.position, Quaternion.Euler(0, 90, 0));
            ColoredDoor door = doorObj.GetComponent<ColoredDoor>();

            DoorColor color = (DoorColor)Random.Range(0, System.Enum.GetValues(typeof(DoorColor)).Length);
            door.doorColor = color;
            doorObj.GetComponent<Renderer>().material = doorMaterials[(int)color];

            allDoors.Add(door);
        }

        foreach (GameObject oldDoor in GameObject.FindGameObjectsWithTag("OldDoor"))
        {
            Destroy(oldDoor);
        }
    }

    void ConnectDoorsToButtons()
    {
        foreach (ColorButton button in allButtons)
        {
            foreach (ColoredDoor door in allDoors)
            {
                if (door.doorColor == button.buttonColor)
                {
                    button.RegisterDoor(door);
                }
            }
        }
    }
}
