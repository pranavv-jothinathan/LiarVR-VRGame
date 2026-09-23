using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Area : MonoBehaviour
{
    [SerializeField] Round session;
    [SerializeField] Collider playTrigger;

    void Awake()
    {
        if (playTrigger == null) playTrigger = GetComponent<Collider>();
        playTrigger.isTrigger = true;
    }

    public bool IsInsidePlayArea(Vector3 worldPoint)
    {
        if (playTrigger == null) return false;

        Vector3 closest = playTrigger.ClosestPoint(worldPoint);
        return (closest - worldPoint).sqrMagnitude < 0.0001f;
    }

    public void TrySubmitCard(RoundCard card)
    {
        if (session == null || card == null) return;
        session.RequestPlayCard(card.CardValue);
    }

    void OnTriggerEnter(Collider other)
    {
        var c = other.GetComponentInParent<RoundCard>();
        if (c != null) c.RegisterPlayTrigger(playTrigger);
    }

    void OnTriggerExit(Collider other)
    {
        var c = other.GetComponentInParent<RoundCard>();
        if (c != null) c.UnregisterPlayTrigger(playTrigger);
    }
}