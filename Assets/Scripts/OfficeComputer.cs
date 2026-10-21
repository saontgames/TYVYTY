
using UnityEngine;
using UnityEngine.InputSystem;

public class OfficeComputer : MonoBehaviour
{
    [Header("Interaccion")]
    public Transform player;
    public float interactionDistance = 2.5f;

    public static bool IsOpen { get; private set; }

    private bool computerOpen;
    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle headingStyle;
    private GUIStyle textStyle;
    private GUIStyle statusStyle;
    private GUIStyle moneyStyle;
    private GUIStyle buttonStyle;

    void Awake()
    {
        IsOpen = false;
    }

    void Update()
    {
        if (player == null || Keyboard.current == null)
            return;

        if (computerOpen)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame ||
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                CloseComputer();
                return;
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                AcceptRequest();
            }

            return;
        }

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenComputer();
        }
    }

    void OpenComputer()
    {
        computerOpen = true;
        IsOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseComputer()
    {
        computerOpen = false;
        IsOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ThirdPersonCamera cameraController =
            Camera.main != null
            ? Camera.main.GetComponent<ThirdPersonCamera>()
            : null;

        if (cameraController != null)
        {
            cameraController.RestoreCameraControl();
        }

        Debug.Log("Computadora cerrada. Camara habilitada.");
    }


    void AcceptRequest()
    {
        if (JobManager.Instance == null)
            return;

        JobManager.Instance.AcceptJob();
    }

    void OnDisable()
    {
        if (computerOpen)
            CloseComputer();

        IsOpen = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && computerOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void CreateStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 26;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor =
            new Color(0.94f, 0.93f, 0.87f);

        subtitleStyle = new GUIStyle(GUI.skin.label);
        subtitleStyle.fontSize = 12;
        subtitleStyle.normal.textColor =
            new Color(0.65f, 0.72f, 0.66f);

        headingStyle = new GUIStyle(GUI.skin.label);
        headingStyle.fontSize = 19;
        headingStyle.fontStyle = FontStyle.Bold;
        headingStyle.normal.textColor = Color.white;

        textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 14;
        textStyle.normal.textColor =
            new Color(0.83f, 0.86f, 0.82f);

        statusStyle = new GUIStyle(GUI.skin.label);
        statusStyle.fontSize = 14;
        statusStyle.fontStyle = FontStyle.Bold;
        statusStyle.normal.textColor =
            new Color(0.83f, 0.90f, 0.72f);

        moneyStyle = new GUIStyle(GUI.skin.label);
        moneyStyle.fontSize = 23;
        moneyStyle.fontStyle = FontStyle.Bold;
        moneyStyle.normal.textColor =
            new Color(0.77f, 0.91f, 0.67f);

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 14;
        buttonStyle.fontStyle = FontStyle.Bold;
    }

    void DrawRect(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(
            rect,
            Texture2D.whiteTexture
        );

        GUI.color = previousColor;
    }

    void OnGUI()
    {
        if (!computerOpen)
        {
            DrawInteractionHint();
            return;
        }

        CreateStyles();

        // Fondo oscuro para concentrarse en la PC.
        DrawRect(
            new Rect(0, 0, Screen.width, Screen.height),
            new Color(0.02f, 0.04f, 0.03f, 0.80f)
        );

        float width = Mathf.Min(620f, Screen.width - 30f);
        float height = Mathf.Min(425f, Screen.height - 30f);

        Rect panel = new Rect(
            (Screen.width - width) / 2f,
            (Screen.height - height) / 2f,
            width,
            height
        );

        DrawRect(
            panel,
            new Color(0.09f, 0.12f, 0.10f)
        );

        float x = panel.x + 24f;
        float y = panel.y + 16f;
        float contentWidth = panel.width - 48f;

        GUI.Label(
            new Rect(x, y, contentWidth, 38),
            "TYVYTY",
            titleStyle
        );

        GUI.Label(
            new Rect(x, y + 39, contentWidth, 20),
            "ADMINISTRACION DEL CEMENTERIO",
            subtitleStyle
        );

        DrawRect(
            new Rect(x, y + 72, contentWidth, 1),
            new Color(0.29f, 0.35f, 0.30f)
        );

        GUI.Label(
            new Rect(x, y + 91, contentWidth, 30),
            "Solicitudes de mantenimiento",
            headingStyle
        );

        Rect card = new Rect(
            x,
            y + 132,
            contentWidth,
            220f
        );

        DrawRect(
            card,
            new Color(0.15f, 0.19f, 0.16f)
        );

        float cardX = card.x + 16f;

        GUI.Label(
            new Rect(cardX, card.y + 13, 230, 24),
            "SOLICITUD #001",
            subtitleStyle
        );

        GUI.Label(
            new Rect(cardX, card.y + 45, card.width - 30, 28),
            "Limpieza de tumba 01",
            headingStyle
        );

        GUI.Label(
            new Rect(cardX, card.y + 79, card.width - 30, 24),
            "Familia Gonzalez - Sector A",
            textStyle
        );

        GUI.Label(
            new Rect(cardX, card.y + 108, card.width - 30, 24),
            "Retirar flores marchitas y llevarlas al basurero.",
            textStyle
        );

        DrawRect(
            new Rect(cardX, card.y + 143, card.width - 32, 1),
            new Color(0.29f, 0.35f, 0.30f)
        );

        GUI.Label(
            new Rect(cardX, card.y + 154, 160, 22),
            "RECOMPENSA",
            subtitleStyle
        );

        GUI.Label(
            new Rect(cardX, card.y + 174, 220, 34),
            "G. 15.000",
            moneyStyle
        );

        DrawJobStatus(card);

        GUI.Label(
            new Rect(x, panel.yMax - 29, contentWidth, 22),
            "ESC o E para cerrar",
            subtitleStyle
        );
    }

    void DrawJobStatus(Rect card)
    {
        if (JobManager.Instance == null)
            return;

        JobManager.JobState state =
            JobManager.Instance.CurrentState;

        Rect buttonRect = new Rect(
            card.xMax - 188,
            card.y + 165,
            170,
            38
        );

        if (state == JobManager.JobState.Available)
        {
            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor =
                new Color(0.47f, 0.66f, 0.45f);

            if (GUI.Button(
                buttonRect,
                "Aceptar pedido [1]",
                buttonStyle))
            {
                AcceptRequest();
            }

            GUI.backgroundColor = previousColor;
        }
        else
        {
            string status =
                JobManager.Instance.GetStatusText();

            GUI.Label(
                buttonRect,
                status,
                statusStyle
            );
        }
    }

    void DrawInteractionHint()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance > interactionDistance)
            return;

        GUI.Box(
            new Rect(
                Screen.width / 2f - 135,
                Screen.height - 85,
                270,
                43
            ),
            "E - Usar computadora"
        );
    }
}
