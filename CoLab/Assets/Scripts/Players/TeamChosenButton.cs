using System.Collections;
using UnityEngine;

public class TeamChosenButton : MonoBehaviour
{
    [SerializeField] GameObject teamChoosingCanvas;
    [SerializeField] GameObject teamChoosingButton;
    public void ChooseRed()
    {
        PlayerMovement.LocalInstance?.SelectTeam(Team.Red);
        teamChoosingButton.SetActive(false);
        teamChoosingCanvas.SetActive(false);
    }

    public void ChooseBlue()
    {
        PlayerMovement.LocalInstance?.SelectTeam(Team.Blue);
        teamChoosingButton.SetActive(false);
        teamChoosingCanvas.SetActive(false);
    }
}
