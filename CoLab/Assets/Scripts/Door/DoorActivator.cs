using UnityEngine;

public class DoorActivator : MonoBehaviour
{
    public GameObject door;
    private void Start()
    {
        door.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!door.activeSelf)
            {
                door.SetActive(true);
            }
        }
    }
}