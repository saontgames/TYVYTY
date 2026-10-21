
using UnityEngine;
using UnityEngine.InputSystem;

public class GravestoneCleaning : MonoBehaviour
{
    [Header("Configuracion")]
    public Transform player;

    public float interactionDistance = 2.5f;
    public float cleaningDuration = 4f;

    [Header("Aspecto")]
    public Color dirtyColor =
        new Color(0.27f, 0.27f, 0.25f);

    public Color cleanColor =
        new Color(0.80f, 0.79f, 0.73f);

    private float cleaningProgress;
    private bool cleaned;

    private Renderer gravestoneRenderer;
    private Material stoneMaterial;

    void Start()
    {
        gravestoneRenderer = GetComponent<Renderer>();

        if (gravestoneRenderer != null)
        {
            stoneMaterial = gravestoneRenderer.material;
            stoneMaterial.color = dirtyColor;
        }
    }

    void Update()
    {
        if (player == null ||
            cleaned ||
            Keyboard.current == null ||
            OfficeComputer.IsOpen)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance > interactionDistance)
            return;

        if (!JobManager.Instance ||
            !JobManager.Instance.CanCleanGravestone())
            return;

        if (Keyboard.current.eKey.isPressed)
        {
            JobManager.Instance.RegisterCleaningProgress();

            cleaningProgress += Time.deltaTime;

            float fraction = Mathf.Clamp01(
                cleaningProgress / cleaningDuration
            );

            if (stoneMaterial != null)
            {
                stoneMaterial.color = Color.Lerp(
                    dirtyColor,
                    cleanColor,
                    fraction
                );
            }

            if (fraction >= 1f)
                FinishCleaning();
        }
    }

    void FinishCleaning()
    {
        if (cleaned)
            return;

        if (JobManager.Instance == null)
            return;

        bool completed =
            JobManager.Instance.CompleteGravestoneCleaning();

        if (!completed)
            return;

        cleaned = true;

        if (stoneMaterial != null)
            stoneMaterial.color = cleanColor;

        Debug.Log("Lapida limpiada correctamente.");
    }

    void OnGUI()
    {
        if (player == null ||
            cleaned ||
            OfficeComputer.IsOpen)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance > interactionDistance)
            return;

        if (JobManager.Instance == null)
            return;

        if (!JobManager.Instance.CanCleanGravestone())
            return;

        float percentage = Mathf.Clamp01(
            cleaningProgress / cleaningDuration
        ) * 100f;

        GUI.Box(
            new Rect(
                Screen.width / 2f - 165f,
                Screen.height - 145f,
                330f,
                65f
            ),
            "Mantene E para limpiar la lapida"
        );

        GUI.Label(
            new Rect(
                Screen.width / 2f - 85f,
                Screen.height - 111f,
                170f,
                25f
            ),
            "Limpieza: " +
            percentage.ToString("F0") + "%"
        );
    }

    void OnDestroy()
    {
        if (stoneMaterial != null)
            Destroy(stoneMaterial);
    }
}
