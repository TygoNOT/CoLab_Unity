using UnityEngine;

public class PlatformVerticalMover : MonoBehaviour
{
    [Header("Attributes")]
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    private Vector3 startPosition;
    private Vector3 topPosition;
    private bool movingUp = true;

    private void Start()
    {
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * moveDistance;
    }

    private void Update()
    {
        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, topPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, topPosition) < 0.01f)
                movingUp = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                movingUp = true;
        }
    }
}