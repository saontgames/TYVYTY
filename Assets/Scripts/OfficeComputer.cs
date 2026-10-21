
using UnityEngine;
using UnityEngine.InputSystem;

public class OfficeComputer : MonoBehaviour
{
    [Header("Interaccion")]
    public Transform player;
    public float interactionDistance = 2.5f;

    public static bool IsOpen { get; private set; }
    public static int LastClosedFrame { get; private set; } = -1;

    private bool computerOpen;
    private Vector2 scrollPosition;

    private GUIStyle titleStyle;
    private GUIStyle headingStyle;
    private GUIStyle textStyle;
    private GUIStyle smallStyle;
    private GUIStyle statusStyle;
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
                AcceptRequest(0);

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                AcceptRequest(1);

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
        scrollPosition = Vector2.zero;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseComputer()
    {
        LastClosedFrame = Time.frameCount;

        computerOpen = false;
        IsOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ThirdPersonCamera cameraController =
            Camera.main != null
            ? Camera.main.GetComponent<ThirdPersonCamera>()
            : null;

        if (cameraController != null)
            cameraController.RestoreCameraControl();

        Debug.Log("Computadora cerrada. Camara habilitada.");
    }

    void AcceptRequest(int index)
    {
        if (JobManager.Instance == null)
            return;

        JobManager.Instance.AcceptJob(index);
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
        titleStyle.normal.textColor = Color.white;

        headingStyle = new GUIStyle(GUI.skin.label);
        headingStyle.fontSize = 19;
        headingStyle.fontStyle = FontStyle.Bold;
        headingStyle.normal.textColor =
            new Color(0.94f, 0.94f, 0.90f);

        textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 14;
        textStyle.normal.textColor =
            new Color(0.86f, 0.89f, 0.85f);

        smallStyle = new GUIStyle(GUI.skin.label);
        smallStyle.fontSize = 12;
        smallStyle.normal.textColor =
            new Color(0.67f, 0.74f, 0.68f);

        statusStyle = new GUIStyle(GUI.skin.label);
        statusStyle.fontSize = 14;
        statusStyle.fontStyle = FontStyle.Bold;
        statusStyle.normal.textColor =
            new Color(0.80f, 0.91f, 0.70f);

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 13;
        buttonStyle.fontStyle = FontStyle.Bold;
    }

    void DrawRect(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(rect, Texture2D.whiteTexture);

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

        DrawRect(
            new Rect(0, 0, Screen.width, Screen.height),
            new Color(0.02f, 0.04f, 0.03f, 0.92f)
        );

        float width = Mathf.Min(680f, Screen.width - 24f);
        float height = Mathf.Min(510f, Screen.height - 24f);

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

        float left = panel.x + 22f;
        float top = panel.y + 15f;
        float innerWidth = panel.width - 44f;

        GUI.Label(
            new Rect(left, top, innerWidth, 35),
            "TYVYTY",
            titleStyle
        );

        GUI.Label(
            new Rect(left, top + 39, innerWidth, 22),
            "ADMINISTRACION - SOLICITUDES",
            smallStyle
        );

        DrawRect(
            new Rect(left, top + 72, innerWidth, 1),
            new Color(0.32f, 0.39f, 0.33f)
        );

        GUI.Label(
            new Rect(left, top + 86, innerWidth, 26),
            "Bandeja de pedidos",
            headingStyle
        );

        float listTop = top + 119f;
        float listHeight = panel.yMax - listTop - 43f;

        
        if (JobManager.Instance != null)
        {
            int count = JobManager.Instance.JobCount;
            int receivedCount = 0;

            for (int i = 0; i < count; i++)
            {
                if (JobManager.Instance.GetJob(i).received)
                    receivedCount++;
            }

            if (receivedCount == 0)
            {
                GUI.Label(
                    new Rect(left, listTop, innerWidth, 30),
                    "Bandeja vacia. No hay solicitudes nuevas.",
                    textStyle
                );
            }
            else
            {
                float contentHeight = receivedCount * 169f;

                scrollPosition = GUI.BeginScrollView(
                    new Rect(left, listTop, innerWidth, listHeight),
                    scrollPosition,
                    new Rect(
                        0, 0,
                        innerWidth - 20f,
                        Mathf.Max(contentHeight, listHeight)
                    )
                );

                int visibleIndex = 0;

                for (int i = 0; i < count; i++)
                {
                    JobManager.CemeteryJob job =
                        JobManager.Instance.GetJob(i);

                    if (!job.received)
                        continue;

                    DrawJobCard(
                        i,
                        new Rect(
                            0,
                            visibleIndex * 169f,
                            innerWidth - 25f,
                            157f
                        )
                    );

                    visibleIndex++;
                }

                GUI.EndScrollView();
            }
        }


        GUI.Label(
            new Rect(
                left,
                panel.yMax - 30,
                innerWidth,
                23
            ),
            "1 / 2 aceptar pedidos  |  ESC o E cerrar",
            smallStyle
        );
    }

    void DrawJobCard(int index, Rect rect)
    {
        JobManager.CemeteryJob job =
            JobManager.Instance.GetJob(index);

        if (job == null)
            return;

        DrawRect(
            rect,
            new Color(0.16f, 0.20f, 0.17f)
        );

        float x = rect.x + 14f;
        float y = rect.y + 10f;
        float width = rect.width - 28f;

        GUI.Label(
            new Rect(x, y, width, 21),
            "SOLICITUD #" + job.id +
            "  |  " + JobManager.Instance.GetStatusText(index),
            statusStyle
        );

        GUI.Label(
            new Rect(x, y + 26, width, 24),
            job.title,
            textStyle
        );

        GUI.Label(
            new Rect(x, y + 52, width, 21),
            job.family + " - " + job.location,
            smallStyle
        );

        GUI.Label(
            new Rect(x, y + 77, width, 21),
            job.description,
            smallStyle
        );

        GUI.Label(
            new Rect(x, y + 115, 200, 27),
            "G. " + job.reward.ToString("N0"),
            statusStyle
        );

        if (job.status == JobManager.JobStatus.Available)
        {
            Rect buttonRect = new Rect(
                rect.xMax - 170f,
                y + 109f,
                155f,
                31f
            );

            if (GUI.Button(
                buttonRect,
                "Aceptar [" + (index + 1) + "]",
                buttonStyle
            ))
            {
                AcceptRequest(index);
            }
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
