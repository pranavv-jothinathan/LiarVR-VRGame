using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OwnerOnlySpriteRenderers : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    [SerializeField] bool includeInactive = true;

    public override void OnEnable()
    {
        base.OnEnable();
        Apply();
    }

    void Start() => Apply();

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView == photonView) Apply();
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) { }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest) { }

    void Apply()
    {
        //try to set the card not be seen by another if card jump still here
        var renderers = includeInactive
            ? GetComponentsInChildren<SpriteRenderer>(true)
            : GetComponentsInChildren<SpriteRenderer>(false);

        bool show = photonView != null && photonView.IsMine;
        foreach (var s in renderers)
            if (s != null) s.enabled = show;
    }
    //no need , we solve card jump
}