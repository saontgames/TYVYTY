
using UnityEngine;

public class ObjectiveNavigator : MonoBehaviour
{
    [Header("Objetos de la escena")]
    public Transform player;
    public Transform flowers;
    public Transform gravestone02;

    [Header("Apariencia")]
    public float arrowDistance = 110f;
    public float arrowOpacity = 0.55f;

    private GUIStyle arrowStyle;
    private GUIStyle labelStyle;

    void CreateStyles()
    {
        if (arrowStyle != null)
            return;

        arrowStyle = new GUIStyle(GUI.skin.label);
        arrowStyle.fontSize = 45;
        arrowStyle.alignment = TextAnchor.MiddleCenter;
        arrowStyle.normal.textColor = Color.white;

        labelStyle = new GUIStyle(GUI.skin.box);
        labelStyle.fontSize = 13;
        labelStyle.alignment = TextAnchor.MiddleCenter;
    }

    Transform FindNearestTrashBin()
    {
        if (player == null)
            return null;

        TrashBinInteraction[] bins =
            FindObjectsByType<TrashBinInteraction>(
                FindObjectsSortMode.None
            );

        Transform nearest = null;
        float bestDistance = float.MaxValue;

        foreach (TrashBinInteraction bin in bins)
        {
            float distance = Vector3.Distance(
                player.position,
                bin.transform.position
            );

            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearest = bin.transform;
            }
        }

        return nearest;
    }

    Transform GetTarget(out string targetName)
    {
        targetName = "";

        if (JobManager.Instance == null)
            return null;

        int index = JobManager.Instance.GetGuidedJobIndex();

        if (index < 0)
            return null;

        if (index == 0)
        {
            FlowerInteraction flower =
                flowers != null
                ? flowers.GetComponent<FlowerInteraction>()
                : null;

            if (flower != null && flower.IsCarrying)
            {
                targetName = "Basurero";
                return FindNearestTrashBin();
            }

            targetName = "Flores marchitas";
            return flowers;
        }

        if (index == 1)
        {
            targetName = "Tumba 02";
            return gravestone02;
        }

        return null;
    }

    void OnGUI()
    {
        if (OfficeComputer.IsOpen || player == null)
            return;

        Camera cam = Camera.main;

        if (cam == null)
            return;

        string targetName;
        Transform target = GetTarget(out targetName);

        if (target == null || !target.gameObject.activeInHierarchy)
            return;

        CreateStyles();

        Vector3 screenPoint = cam.WorldToScreenPoint(
            target.position + Vector3.up
        );

        Vector2 center = new Vector2(
            Screen.width / 2f,
            Screen.height / 2f
        );

        Vector2 direction = new Vector2(
            screenPoint.x - center.x,
            Screen.height - screenPoint.y - center.y
        );

        if (screenPoint.z < 0f)
            direction = -direction;

        if (direction.sqrMagnitude < 1f)
            direction = Vector2.up;

        direction.Normalize();

        Vector2 arrowPosition =
            center + direction * arrowDistance;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg + 90f;

        Matrix4x4 oldMatrix = GUI.matrix;
        Color oldColor = GUI.color;

        GUIUtility.RotateAroundPivot(
            angle,
            arrowPosition
        );

        GUI.color = new Color(
            0.85f, 0.98f, 0.80f, arrowOpacity
        );

        GUI.Label(
            new Rect(
                arrowPosition.x - 30,
                arrowPosition.y - 30,
                60,
                60
            ),
            "\u25B2",
            arrowStyle
        );

        GUI.matrix = oldMatrix;
        GUI.color = oldColor;

        float distance = Vector3.Distance(
            player.position,
            target.position
        );

        float labelX = Mathf.Clamp(
            center.x - 105,
            5,
            Screen.width - 215
        );

        float labelY = Mathf.Clamp(
            center.y + 155,
            5,
            Screen.height - 45
        );

        GUI.Box(
            new Rect(labelX, labelY, 210, 35),
            targetName + " - " +
            distance.ToString("F0") + " m",
            labelStyle
        );
    }
}
