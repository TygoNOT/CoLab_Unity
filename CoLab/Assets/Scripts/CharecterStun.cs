using UnityEngine;
using System.Collections;
using System.Globalization;

public class CharacterStun : MonoBehaviour
{
    public bool IsStunned { get; private set; } = false;

    private CharacterController controller; 
    /*
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
    }
    */
    public void ApplyStun(float duration)
    {
        if (!IsStunned)
            StunCoroutine(duration);
    }

    private void StunCoroutine(float duration)
    {
        controller = GetComponent<CharacterController>();
        IsStunned = true;

        if (controller != null)
            controller.enabled = false;


        new WaitForSeconds(duration);

        if (controller != null)
            controller.enabled = true;

        IsStunned = false;
    }
}