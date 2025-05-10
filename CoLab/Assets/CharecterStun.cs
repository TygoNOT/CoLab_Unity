using UnityEngine;
using System.Collections;
using System.Globalization;

public class CharacterStun : MonoBehaviour
{
    public bool IsStunned { get; private set; } = false;

    private CharacterController controller; // или другой скрипт движения

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
    }

    public void ApplyStun(float duration)
    {
        if (!IsStunned)
            StunCoroutine(duration);
    }

    private void StunCoroutine(float duration)
    {
        controller = GetComponent<CharacterController>();
        IsStunned = true;

        // Отключаем движение
        if (controller != null)
            controller.enabled = false;

        // Тут можно проиграть анимацию стана, эффект и т.д.

        new WaitForSeconds(duration);

        // Включаем обратно
        if (controller != null)
            controller.enabled = true;

        IsStunned = false;
    }
}