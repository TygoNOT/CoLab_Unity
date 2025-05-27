using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperManager : MonoBehaviour
{
    [SerializeField] private List<MineButton> buttons;
    [SerializeField] private int mineCount = 10;
    [SerializeField] private float resetDelay = 2f;
    [SerializeField] private GameObject doorToOpen;

    private int totalSafeButtons;
    private int revealedSafeButtons;

    private void Start()
    {
        SetupPuzzle();
    }

    void SetupPuzzle()
    {
        revealedSafeButtons = 0;
        List<MineButton> available = new List<MineButton>(buttons);

        // Clear all buttons first
        foreach (var b in buttons)
        {
            b.Init(this);
        }

        // Randomly assign mines
        for (int i = 0; i < mineCount && available.Count > 0; i++)
        {
            int index = Random.Range(0, available.Count);
            available[index].isMine = true;
            available.RemoveAt(index);
        }

        // Count total safe buttons
        totalSafeButtons = buttons.Count - mineCount;

        // Set neighbors
        foreach (var b in buttons)
        {
            b.neighbors.Clear();

            foreach (var other in buttons)
            {
                if (b == other) continue;
                if (Vector3.Distance(b.transform.position, other.transform.position) < 3f)
                {
                    b.neighbors.Add(other);
                }
            }
        }
    }

    public void OnMineTriggered()
    {
        StartCoroutine(ResetAfterDelay());
    }
    public void OnSafeButtonRevealed()
    {
        revealedSafeButtons++;

        if (revealedSafeButtons >= totalSafeButtons)
        {
            Debug.Log("Puzzle Solved! Door can open now.");
            // You can trigger door opening here
            if (doorToOpen != null)
            {
                Renderer rend = doorToOpen.GetComponent<Renderer>();
                Collider col = doorToOpen.GetComponent<Collider>();

                if (rend != null) rend.enabled = false;
                if (col != null) col.enabled = false;
            }
        }
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);
        SetupPuzzle();
    }
}