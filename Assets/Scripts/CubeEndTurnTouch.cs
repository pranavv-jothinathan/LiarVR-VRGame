using Photon.Pun;
using UnityEngine;

public class CubeEndTurnTouch : MonoBehaviour
{
    [SerializeField] float cooldownSeconds = 0.35f;
    float nextAllowedTime;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[CubeTouch] OnTriggerEnter from='{other.name}' layer={LayerMask.LayerToName(other.gameObject.layer)} time={Time.time:F3}");

        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("[CubeTouch] Skip: not in room.");
            return;
        }

        if (TurnManager.Instance == null)
        {
            Debug.Log("[CubeTouch] Skip: TurnManager.Instance is null.");
            return;
        }

        if (!TurnManager.Instance.IsLocalPlayersTurn())
        {
            Debug.Log($"[CubeTouch] Skip: not my turn. localActor={PhotonNetwork.LocalPlayer?.ActorNumber} " +
                      $"currentTurnActor={TurnManager.Instance.CurrentTurnActorNumber}");
            return;
        }

        if (Time.time < nextAllowedTime)
        {
            Debug.Log($"[CubeTouch] Skip: cooldown until {nextAllowedTime:F3}");
            return;
        }

        nextAllowedTime = Time.time + cooldownSeconds;
        Debug.Log("[CubeTouch] OK -> RequestEndTurn()");
        TurnManager.Instance.RequestEndTurn();
    }
}