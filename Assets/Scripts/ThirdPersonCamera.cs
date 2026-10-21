
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 3.5f;
    public float height = 1.5f;
    public float shoulderOffset = 0.65f;
    public float mouseSensitivity = 0.12f;
    public float smoothSpeed = 12f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        if (target != null)
            yaw = target.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool locked =
                Cursor.lockState == CursorLockMode.Locked;

            Cursor.lockState = locked
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            Cursor.visible = locked;
        }

        if (Mouse.current != null &&
            Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 mouse = Mouse.current.delta.ReadValue();

            yaw += mouse.x * mouseSensitivity;
            pitch -= mouse.y * mouseSensitivity;

            pitch = Mathf.Clamp(pitch, -30f, 65f);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 focusPoint =
            target.position + Vector3.up * height;

        Vector3 desiredPosition =
            focusPoint
            - rotation * Vector3.forward * distance
            + rotation * Vector3.right * shoulderOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.rotation = rotation;
    }
}
