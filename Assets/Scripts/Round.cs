using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class Round : MonoBehaviourPunCallbacks
{
    public const string TurnActorKey = "turnActor";

    public static event Action<int, string> OnPlayCommittedAll; // actor, cardValue（所有人收到）
    public static event Action<int> OnTurnChangedAll;          // 当前回合 actor（所有人收到）

    PhotonView _pv;

    void Awake() => _pv = GetComponent<PhotonView>();

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            TryInitTwoPlayerTurn();

        PushTurnToListeners();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
            TryInitTwoPlayerTurn();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
            TryInitTwoPlayerTurn();
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps != null && changedProps.ContainsKey(TurnActorKey))
            PushTurnToListeners();
    }

    void TryInitTwoPlayerTurn()
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.PlayerList.Length < 2) return;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(TurnActorKey)) return;

        int a = PhotonNetwork.PlayerList[0].ActorNumber;
        int b = PhotonNetwork.PlayerList[1].ActorNumber;
        int first = Mathf.Min(a, b);

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { TurnActorKey, first } });
    }

    void PushTurnToListeners()
    {
        if (!PhotonNetwork.InRoom) return;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TurnActorKey, out object v)) return;
        OnTurnChangedAll?.Invoke((int)v);
    }

    public bool IsLocalPlayersTurn()
    {
        if (!PhotonNetwork.InRoom) return false;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TurnActorKey, out object v)) return false;
        return (int)v == PhotonNetwork.LocalPlayer.ActorNumber;
    }

    /// <summary>VR 出牌区在确认“落入出牌区”后调用：会 RPC 给 Master 校验并切回合。</summary>
    public void RequestPlayCard(string cardValue)
    {
        if (!PhotonNetwork.InRoom) return;
        if (!IsLocalPlayersTurn()) return;

        _pv.RPC(nameof(RPC_PlayCardOnMaster), RpcTarget.MasterClient,
            PhotonNetwork.LocalPlayer.ActorNumber,
            cardValue);
    }

    [PunRPC]
    void RPC_PlayCardOnMaster(int actor, string cardValue, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TurnActorKey, out object v)) return;

        int current = (int)v;
        if (actor != current) return; // 不是当前回合：拒绝

        // 广播：用于音效/UI/把牌固定到桌面等
        _pv.RPC(nameof(RPC_NotifyPlayAll), RpcTarget.All, actor, cardValue);

        int next = GetOtherActor(actor);
        if (next == -1) return;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { TurnActorKey, next } });
    }

    [PunRPC]
    void RPC_NotifyPlayAll(int actor, string cardValue)
    {
        OnPlayCommittedAll?.Invoke(actor, cardValue);
    }

    static int GetOtherActor(int actor)
    {
        foreach (var p in PhotonNetwork.PlayerList)
            if (p.ActorNumber != actor) return p.ActorNumber;

        return -1;
    }
}