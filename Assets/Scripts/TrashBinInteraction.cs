
using UnityEngine;
using UnityEngine.InputSystem;

public class TrashBinInteraction : MonoBehaviour
{
    public Transform player;
    public FlowerInteraction flowers;

    public float interactionDistance = 2.5f;

    void Update()
    {
        if (player == null ||
            flowers == null ||
            Keyboard.current == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance &&
            flowers.IsCarrying &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            flowers.DisposeFlowers();

            Debug.Log("Flores depositadas en el basurero.");
        }
    }
}
