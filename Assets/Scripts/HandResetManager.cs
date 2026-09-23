using System.Linq;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class HandResetManager : MonoBehaviourPun
{
    
    [SerializeField] private Transform[] handSpawnPoints;

  
    [SerializeField] private bool resetChildrenLocalPose = false;

    /// <summary>
    ///only for round manager to trigger。
    /// 
    /// </summary>
    public void RequestResetAllFromMaster()
    {
        if (!PhotonNetwork.InRoom) return;
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_DoResetLocalOwnedHand), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_DoResetLocalOwnedHand()
    {
        ResetLocalOwnedHandNow();
    }

    private void ResetLocalOwnedHandNow()
    {
        if (handSpawnPoints == null || handSpawnPoints.Length == 0) return;

        int myIndex = GetLocalPlayerIndex();
        int spawnIndex = Mathf.Clamp(myIndex, 0, handSpawnPoints.Length - 1);
        Transform targetSpawn = handSpawnPoints[spawnIndex];
        if (targetSpawn == null) return;

        Transform handRoot = FindLocalOwnedHandRoot();
        if (handRoot == null)
        {
            Debug.LogWarning("[HandReset] Local owned hand root not found.");
            return;
        }

        //parent to local originon for reset
        handRoot.SetPositionAndRotation(targetSpawn.position, targetSpawn.rotation);

        // no speed for reset
        var rbs = handRoot.GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in rbs)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        //we need 5 children all be reset
        if (resetChildrenLocalPose)
        {
            for (int i = 0; i < handRoot.childCount; i++)
            {
                Transform c = handRoot.GetChild(i);
                c.localPosition = Vector3.zero;
                c.localRotation = Quaternion.identity;
            }
        }

        
    }

    private static int GetLocalPlayerIndex()
    {
        var ordered = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
        int idx = System.Array.FindIndex(ordered, p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
        return Mathf.Max(0, idx);
    }

    /// <summary>
    /// find card identity for valuse
    /// </summary>
    private static Transform FindLocalOwnedHandRoot()
    {
        var views = Object.FindObjectsByType<PhotonView>(FindObjectsSortMode.None);

        Transform best = null;
        int bestCount = 0;

        foreach (var pv in views)
        {
            if (pv == null || !pv.IsMine) continue;

            // don;t need card identity
            if (pv.GetComponent<CardIdentity>() != null) continue;

            var cards = pv.GetComponentsInChildren<CardIdentity>(true);
            if (cards == null || cards.Length == 0) continue;

            // get parent
            if (cards.Length > bestCount)
            {
                bestCount = cards.Length;
                best = pv.transform;
            }
        }

        // 5 card like liar bar
        if (best != null && bestCount >= 5) return best;

        return null;
    }
}