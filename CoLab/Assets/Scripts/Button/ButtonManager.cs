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

    public void ResetButtons()
    {
        foreach (var button in buttons)
        {
            button.ResetPressed(); 
            button.GetComponent<Renderer>().material.color = button.defaultColor;  
        }
    }
}