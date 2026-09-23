using UnityEngine;
using Photon.Pun;

public class PlayZoneDetector : MonoBehaviour
{
    [SerializeField] private RoundGameManager roundGameManager;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PlayZone is: " + other.name);
        CardIdentity card = null;
        // find parent of the card
        if (other.attachedRigidbody != null)
            card = other.attachedRigidbody.GetComponent<CardIdentity>();
        // at least can find a card and return, won't appear pun error
        if (card == null) card = other.GetComponentInParent<CardIdentity>();
        if (card == null) card = other.GetComponentInChildren<CardIdentity>();
        
        if (card == null) return;
        if (roundGameManager == null) return;
        roundGameManager.RequestPlayCard(card.cardValue, card.gameObject);
        Debug.Log("PlayZone rquestCard: " + card.cardValue);
    }
}