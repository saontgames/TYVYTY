
using UnityEngine;
using UnityEngine.InputSystem;

public class OfficeComputer : MonoBehaviour
{
    [Header("Interaccion")]
    public Transform player;
    public float interactionDistance = 2.5f;

    private bool computerOpen = false;

    void Update()
    {
        if (player == null || Keyboard.current == null)
            return;

        // Si la computadora esta abierta:
        if (computerOpen)
        {
            // E cierra la computadora.
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                computerOpen = false;
                return;
            }

            // 1 acepta el pedido disponible.
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                if (JobManager.Instance != null)
                {
                    JobManager.Instance.AcceptJob();
                }
            }

            return;
        }

        // Solo se puede abrir estando cerca.
        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            computerOpen = true;
        }
    }

    void OnGUI()
    {
        if (player == null)
            return;

        if (!computerOpen)
        {
            float distance = Vector3.Distance(
                player.position,
                transform.position
            );

            if (distance <= interactionDistance)
            {
                GUI.Box(
                    new Rect(
                        Screen.width / 2f - 140,
                        Screen.height - 100,
                        280,
                        45
                    ),
                    "E - Usar computadora"
                );
            }

            return;
        }

        float width = Mathf.Min(550, Screen.width - 40);
        float left = (Screen.width - width) / 2f;
        float top = (Screen.height - 350) / 2f;

        GUI.Box(
            new Rect(left, top, width, 350),
            "TYVYTY - ADMINISTRACION"
        );

        GUI.Label(
            new Rect(left + 25, top + 45, width - 50, 30),
            "SOLICITUD N. 001"
        );

        GUI.Label(
            new Rect(left + 25, top + 85, width - 50, 30),
            "Familia Gonzalez - Sector A - Tumba 01"
        );

        GUI.Label(
            new Rect(left + 25, top + 125, width - 50, 30),
            "Trabajo: retirar flores marchitas"
        );

        GUI.Label(
            new Rect(left + 25, top + 165, width - 50, 30),
            "Recompensa: G. 15.000"
        );

        if (JobManager.Instance != null)
        {
            string status = JobManager.Instance.GetStatusText();

            GUI.Label(
                new Rect(left + 25, top + 210, width - 50, 30),
                "Estado: " + status
            );

            if (JobManager.Instance.CurrentState ==
                JobManager.JobState.Available)
            {
                GUI.Label(
                    new Rect(left + 25, top + 260, width - 50, 30),
                    "Presiona 1 para aceptar el pedido"
                );
            }
        }

        GUI.Label(
            new Rect(left + 25, top + 315, width - 50, 25),
            "Presiona E para cerrar"
        );
    }
}
