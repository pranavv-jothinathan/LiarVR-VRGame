using Photon.Pun;
using Oculus.Interaction;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Grabbable))]
public class Pun2GrabPhysicsLock : MonoBehaviourPun
{
    [SerializeField]
    bool useGravityWhenReleased = true;

    Grabbable _grabbable;
    Rigidbody _rb;
    //why the cards are jumping becaude the rigibody is still using gravity and photon has prpblem.
    void Awake()
    {
        _grabbable = GetComponent<Grabbable>();
        _rb = GetComponent<Rigidbody>();

        _grabbable.WhenPointerEventRaised += HandlePointer;
    }

    void OnDestroy()
    {
        if (_grabbable != null)
            _grabbable.WhenPointerEventRaised -= HandlePointer;
    }

    void HandlePointer(PointerEvent evt)
    {
        if (!PhotonNetwork.InRoom) return;

        switch (evt.Type)
        {
            //try the rigibody disable the gravity and let photon normal
            case PointerEventType.Select:
                photonView.RPC(nameof(RPC_RemoteHeldRigidbody), RpcTarget.Others, true);
                break;

            case PointerEventType.Unselect:
            case PointerEventType.Cancel:
                photonView.RPC(nameof(RPC_RemoteHeldRigidbody), RpcTarget.Others, false);
                break;
        }
    }

    [PunRPC]
    void RPC_RemoteHeldRigidbody(bool heldElsewhere)
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();

        if (heldElsewhere)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }
        else
        {
            _rb.isKinematic = false;
            _rb.useGravity = useGravityWhenReleased;

            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}