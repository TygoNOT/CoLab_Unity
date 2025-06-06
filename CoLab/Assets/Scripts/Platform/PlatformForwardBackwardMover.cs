using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlatformForwardBackwardMover : NetworkBehaviour
{
    [Header("Attributes")]
    public float moveDistance = 5f;    
    public float moveSpeed = 2f;
    private Vector3 startPosition;
    private Vector3 forwardPosition;
    private bool movingForward = true;
    private bool isPaused = false;
    private void Start()
    {
        startPosition = transform.position;
        forwardPosition = startPosition + Vector3.forward * moveDistance;
    }

    private void Update()
    {
        if (isPaused) return;
        if (movingForward)
        {
            transform.position = Vector3.MoveTowards(transform.position, forwardPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, forwardPosition) < 0.01f)
                movingForward = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                movingForward = true;
        }
    }
    public void PauseMovement(float duration)
    {
        StartCoroutine(PauseCoroutine(duration));
    }

    private IEnumerator PauseCoroutine(float duration)
    {
        isPaused = true;
        yield return new WaitForSeconds(duration);
        isPaused = false;
    }
}