using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TurnManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] PhotonView photonView;

    public int CurrentTurnActorNumber { get; private set; }

    bool turnInitialized;

    int lastSerializedTurn = int.MinValue;
    bool lastSerializedInit;

    void Awake()
    {
        Instance = this;
        if (photonView == null)
            photonView = GetComponent<PhotonView>();

        Debug.Log($"[TurnDebug] Awake viewId={photonView?.ViewID} isMine={photonView?.IsMine}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[TurnDebug] OnJoinedRoom isMaster={PhotonNetwork.IsMasterClient} " +
                  $"playerCount={PhotonNetwork.CurrentRoom?.PlayerCount} localActor={PhotonNetwork.LocalPlayer?.ActorNumber}");
        TryInitializeTurnMaster("OnJoinedRoom");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[TurnDebug] OnPlayerEnteredRoom new='{newPlayer.NickName}' actor={newPlayer.ActorNumber} " +
                  $"playerCount={PhotonNetwork.CurrentRoom?.PlayerCount}");
        TryInitializeTurnMaster("OnPlayerEnteredRoom");
    }

    void TryInitializeTurnMaster(string reason)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[TurnDebug] TryInit({reason}) skip: not master.");
            return;
        }

        if (turnInitialized)
        {
            Debug.Log($"[TurnDebug] TryInit({reason}) skip: already initialized. currentTurnActor={CurrentTurnActorNumber}");
            return;
        }

        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log($"[TurnDebug] TryInit({reason}) skip: CurrentRoom null.");
            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            Debug.Log($"[TurnDebug] TryInit({reason}) wait: playerCount={PhotonNetwork.CurrentRoom.PlayerCount} (<2).");
            return;
        }

        var ordered = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
        CurrentTurnActorNumber = ordered[0].ActorNumber;
        turnInitialized = true;

        Debug.Log($"[TurnDebug] TryInit({reason}) OK -> first turn actor={CurrentTurnActorNumber} " +
                  $"players=[{string.Join(",", ordered.Select(p => p.ActorNumber.ToString()))}]");
    }

    public bool IsLocalPlayersTurn()
    {
        if (PhotonNetwork.LocalPlayer == null)
            return false;
        if (CurrentTurnActorNumber == 0)
            return false;
        return PhotonNetwork.LocalPlayer.ActorNumber == CurrentTurnActorNumber;
    }

    public void RequestEndTurn()
    {
        Debug.Log($"[TurnDebug] RequestEndTurn() called. isMaster={PhotonNetwork.IsMasterClient} " +
                  $"localActor={PhotonNetwork.LocalPlayer?.ActorNumber} currentTurnActor={CurrentTurnActorNumber} " +
                  $"isMyTurn={IsLocalPlayersTurn()}");

        if (!IsLocalPlayersTurn())
        {
            Debug.Log("[TurnDebug] RequestEndTurn() abort: not local player's turn.");
            return;
        }

        Debug.Log("[TurnDebug] RequestEndTurn() -> RPC to MasterClient");
        photonView.RPC(nameof(RPC_RequestEndTurn), RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_RequestEndTurn(PhotonMessageInfo info)
    {
        Debug.Log($"[TurnDebug] RPC_RequestEndTurn recv isMaster={PhotonNetwork.IsMasterClient} " +
                  $"senderActor={info.Sender?.ActorNumber} currentTurnActor={CurrentTurnActorNumber}");

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[TurnDebug] RPC_RequestEndTurn ignore: not master.");
            return;
        }

        if (info.Sender == null || info.Sender.ActorNumber != CurrentTurnActorNumber)
        {
            Debug.Log($"[TurnDebug] RPC_RequestEndTurn reject: sender not current turn. " +
                      $"sender={info.Sender?.ActorNumber} expected={CurrentTurnActorNumber}");
            return;
        }

        int before = CurrentTurnActorNumber;
        AdvanceToNextActor();
        Debug.Log($"[TurnDebug] RPC_RequestEndTurn OK: turn {before} -> {CurrentTurnActorNumber}");
    }

    void AdvanceToNextActor()
    {
        var ordered = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
        if (ordered.Length == 0)
        {
            Debug.LogWarning("[TurnDebug] AdvanceToNextActor: no players.");
            return;
        }

        int idx = System.Array.FindIndex(ordered, p => p.ActorNumber == CurrentTurnActorNumber);
        if (idx < 0)
        {
            Debug.LogWarning($"[TurnDebug] AdvanceToNextActor: current actor {CurrentTurnActorNumber} not in list. Reset idx=0.");
            idx = 0;
        }

        int next = (idx + 1) % ordered.Length;
        int old = CurrentTurnActorNumber;
        CurrentTurnActorNumber = ordered[next].ActorNumber;

        Debug.Log($"[TurnDebug] AdvanceToNextActor: {old} -> {CurrentTurnActorNumber} " +
                  $"order=[{string.Join(",", ordered.Select(p => p.ActorNumber.ToString()))}]");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CurrentTurnActorNumber);
            stream.SendNext(turnInitialized);

            if (CurrentTurnActorNumber != lastSerializedTurn || turnInitialized != lastSerializedInit)
            {
                lastSerializedTurn = CurrentTurnActorNumber;
                lastSerializedInit = turnInitialized;
                Debug.Log($"[TurnDebug] Serialize WRITE actor={CurrentTurnActorNumber} init={turnInitialized}");
            }
        }
        else
        {
            int newTurn = (int)stream.ReceiveNext();
            bool newInit = (bool)stream.ReceiveNext();

            if (newTurn != CurrentTurnActorNumber || newInit != turnInitialized)
                Debug.Log($"[TurnDebug] Serialize READ changed: actor {CurrentTurnActorNumber}->{newTurn}, init {turnInitialized}->{newInit}");

            CurrentTurnActorNumber = newTurn;
            turnInitialized = newInit;
        }
    }

    void OnGUI()
    {
        if (!PhotonNetwork.InRoom)
            return;

        GUILayout.BeginArea(new Rect(8, 8, 520, 140));
        GUILayout.Label($"[TurnDebug HUD] Master: {PhotonNetwork.IsMasterClient}");
        GUILayout.Label($"Local Actor: {PhotonNetwork.LocalPlayer?.ActorNumber}");
        GUILayout.Label($"Current Turn Actor: {CurrentTurnActorNumber}");
        GUILayout.Label($"Turn initialized: {turnInitialized}");
        GUILayout.Label($"Is my turn: {IsLocalPlayersTurn()}");
        GUILayout.EndArea();
    }
}