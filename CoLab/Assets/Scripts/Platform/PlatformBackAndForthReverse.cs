using System.Collections;
using UnityEngine;

public class PlatformBackAndForthReverse : MonoBehaviour
{
    [Header("Attributes")]
    public float moveDistance = 5f;
    public float moveSpeed = 2f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingRight = false;
    private bool isPaused = false;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition - Vector3.right * moveDistance;
    }

    private void Update()
    {
        if (isPaused) return;
        if (!movingRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                movingRight = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                movingRight = false;
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