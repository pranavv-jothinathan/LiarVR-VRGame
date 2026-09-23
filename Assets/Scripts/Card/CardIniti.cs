using UnityEngine;
using System.Collections;

public class CardController : MonoBehaviour
{
    [Header("Target Objects")]
    // Drag your existing card from Hierarchy here
    public GameObject targetCard;

    // Drag your empty GameObject (target spot) here
    public Transform[] slotPositions;

    [Header("Settings")]
    public float moveDelay = 0.5f;

    // This runs automatically when the game starts
    private void Start()
    {
        if (targetCard != null && slotPositions.Length > 0)
        {
            StartCoroutine(MoveCardSequence());
        }
        else
        {
            Debug.LogWarning("Missing References: Please assign Target Card and Slot Positions in Inspector.");
        }
    }

    // ContextMenu allows you to trigger this by right-clicking the component in Inspector
    [ContextMenu("Execute Move")]
    public void ExecuteMove()
    {
        StartCoroutine(MoveCardSequence());
    }

    IEnumerator MoveCardSequence()
    {
        Debug.Log("Card move sequence started: " + targetCard.name);

        // 1. Physics Safety: Disable physics to prevent collision glitches
        Rigidbody rb = targetCard.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 2. Movement: Set position and rotation
        // Adding a tiny offset (0.005f) to Y axis to prevent Z-Fighting (flickering)
        targetCard.transform.position = slotPositions[0].position + new Vector3(0, 0.005f, 0);
        targetCard.transform.rotation = slotPositions[0].rotation;

        Debug.Log("Card moved to slot 0 successfully.");

        // 3. Wait for the set delay
        yield return new WaitForSeconds(moveDelay);

        // 4. Re-enable physics so you can grab it in VR
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}