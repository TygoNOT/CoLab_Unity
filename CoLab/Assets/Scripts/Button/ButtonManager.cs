using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ButtonManager : NetworkBehaviour
{
    public static ButtonManager Instance;

    private List<Button> buttons = new List<Button>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Button[] allButtons = FindObjectsOfType<Button>();

        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].buttonIndex = i;
            buttons.Add(allButtons[i]);
        }
    }

    public Button GetButtonByIndex(int index)
    {
        return buttons.Find(b => b.buttonIndex == index);
    }

    [ClientRpc]
    public void ResetButtonsClientRpc()
    {
        foreach (var button in buttons)
        {
            button.ResetPressed();
            button.GetComponent<Renderer>().material.color = button.defaultColor;
        }
    }
    public void AssignButtonIndicesFromServer(Dictionary<int, Button> indexToButton)
    {
        buttons.Clear();
        foreach (var pair in indexToButton)
        {
            pair.Value.buttonIndex = pair.Key;
            buttons.Add(pair.Value);
        }
    }

    public void ClearAndReassignButtons(Button[] newButtons)
    {
        buttons.Clear();
        for (int i = 0; i < newButtons.Length; i++)
        {
            newButtons[i].buttonIndex = i;
            buttons.Add(newButtons[i]);
        }
    }
}