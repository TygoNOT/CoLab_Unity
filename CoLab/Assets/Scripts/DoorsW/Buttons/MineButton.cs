using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineButton : MonoBehaviour
{
    public bool isMine = false;
    public bool isRevealed = false;
    public List<MineButton> neighbors = new List<MineButton>();

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material revealedMaterial;
    [SerializeField] private Material mineMaterial;
    [SerializeField] private TextMesh numberText;

    private MinesweeperManager puzzleManager;

    public void Init(MinesweeperManager manager)
    {
        puzzleManager = manager;
        isMine = false;
        isRevealed = false;
        numberText.text = "";
        GetComponent<Renderer>().material = defaultMaterial;
    }

    public void Reveal()
    {
        if (isRevealed) return;

        isRevealed = true;

        if (isMine)
        {
            GetComponent<Renderer>().material = mineMaterial;
            puzzleManager.OnMineTriggered();
        }
        else
        {
            GetComponent<Renderer>().material = revealedMaterial;
            int nearbyMines = CountNearbyMines();
            numberText.text = nearbyMines.ToString();
            puzzleManager.OnSafeButtonRevealed();
        }
    }

    int CountNearbyMines()
    {
        int count = 0;
        foreach (var neighbor in neighbors)
        {
            if (neighbor != null && neighbor.isMine)
                count++;
        }
        return count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRevealed && other.CompareTag("Player"))
        {
            Reveal();
        }
    }
}
