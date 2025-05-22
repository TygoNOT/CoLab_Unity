using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourManager : MonoBehaviour
{
    [SerializeField] private GameObject jumpPad;
    [SerializeField] private NetworkUi networkUi;
    private bool hasActivated = false;

    void Update()
    {
        if (networkUi == null || jumpPad == null)
            return;

        if (!hasActivated && networkUi.TimerValue >= 180f)
        {
            jumpPad.SetActive(true);
            hasActivated = true;
        }
    }
}