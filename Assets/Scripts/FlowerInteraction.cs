
using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerInteraction : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 2.5f;

    private bool isCarrying = false;
    private bool disposed = false;

    private Transform originalParent;
    private Collider flowerCollider;

    public bool IsCarrying => isCarrying;

    void Start()
    {
        originalParent = transform.parent;
        flowerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (player == null ||
            disposed ||
            Keyboard.current == null)
            return;

        if (!isCarrying)
        {
            float distance = Vector3.Distance(
                player.position,
                transform.position
            );

            if (distance <= interactionDistance &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                PickUp();
            }
        }
        else if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Drop();
        }
    }

    void PickUp()
    {
        isCarrying = true;

        transform.SetParent(player);

        transform.localPosition =
            new Vector3(0.5f, 1.1f, 0.8f);

        transform.localRotation = Quaternion.identity;

        if (flowerCollider != null)
            flowerCollider.enabled = false;

        if (JobManager.Instance != null)
            JobManager.Instance.RegisterFlowerPickup();

        Debug.Log("Flores recogidas.");
    }

    void Drop()
    {
        isCarrying = false;

        transform.SetParent(originalParent, true);

        Vector3 dropPosition =
            player.position + player.forward * 1.2f;

        dropPosition.y = 0.2f;

        transform.position = dropPosition;
        transform.rotation = Quaternion.identity;

        if (flowerCollider != null)
            flowerCollider.enabled = true;

        Debug.Log("Flores soltadas.");
    }

    public void DisposeFlowers()
    {
        if (!isCarrying || disposed)
            return;

        if (JobManager.Instance == null)
        {
            Debug.LogWarning("Falta JobManager.");
            return;
        }

        bool completed =
            JobManager.Instance.CompleteFlowerDisposal();

        if (!completed)
            return;

        disposed = true;
        isCarrying = false;

        gameObject.SetActive(false);
    }
}
