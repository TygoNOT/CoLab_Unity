using UnityEngine;
using System.Collections;

public class PlatformVerticalMoverReverse : MonoBehaviour
{
    [Header("Attributes")]
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 bottomPosition;
    private bool movingUp = false;
    private bool isPaused = false;

    private void Start()
    {
        startPosition = transform.position;
        bottomPosition = startPosition - Vector3.up * moveDistance;
    }

    private void Update()
    {
        if (isPaused) return;

        if (!movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, bottomPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, bottomPosition) < 0.01f)
                movingUp = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                movingUp = false;
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