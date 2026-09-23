using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoundGameManager : MonoBehaviourPun
{
    [Header("调试")]
    [SerializeField] private bool verboseLog = true;
    [SerializeField] private PlayAudioFeedback audioFeedback;
    [SerializeField] private HandResetManager handResetManager;
    public WinEffectsManager effectsManager;

    // 上一手记录（给下一步质疑用）
    public int LastPlayedActor { get; private set; } = -1;
    public string LastPlayedCard { get; private set; } = "";
    public bool HasPendingPlay { get; private set; } = false;

    private readonly Dictionary<int, int> handCountByActor = new();
    private readonly HashSet<int> committedCardViewIds = new(); // 防止同一张牌重复计数
    private bool roundEnded = false;

    [PunRPC]
    private void RPC_ChallengeOnMaster(int challengerActor, PhotonMessageInfo info)
    {
        Debug.Log($"[Round] RPC_ChallengeOnMaster received from actor={challengerActor} with info={info}");
        if (!PhotonNetwork.IsMasterClient) return;
        if (!HasPendingPlay) return;
        if (info.Sender == null || info.Sender.ActorNumber != challengerActor) return;
        //if (challengerActor == LastPlayedActor) return; // 自己不能质疑自己, 测试先取消

        // 规则：Q 为正确牌
        bool lastCardIsQ = string.Equals(LastPlayedCard, "Q", System.StringComparison.OrdinalIgnoreCase);

        // 一次质疑后关闭待质疑状态
        HasPendingPlay = false;

        // 广播给所有客户端
        photonView.RPC(
            nameof(RPC_ChallengeResultAll),
            RpcTarget.All,
            challengerActor,
            LastPlayedActor,
            LastPlayedCard,
            lastCardIsQ
        );
        Debug.Log($"[Round] Challenge result broadcasted with challenger={challengerActor}, playedActor={LastPlayedActor}, card={LastPlayedCard}, isQ={lastCardIsQ}");
    }
    [PunRPC]
    
    private void RPC_ChallengeResultAll(int challengerActor, int playedActor, string playedCard, bool lastCardIsQ)
    {
        Debug.Log($"[Round] Challenge result card={playedCard}, isQ={lastCardIsQ}");

        if (audioFeedback != null)
        {
            // 这里先给所有人播同一种“结果音”
            audioFeedback.PlayChallengeResultSfx(lastCardIsQ);
            Debug.Log($"[Round] Playing challenge result sound for card={playedCard}, isQ={lastCardIsQ}");
            // 先算这次质疑谁赢
            int winnerActor = lastCardIsQ ? playedActor : challengerActor;

            // 统一音效播完后，再播个人胜负音效
            float delay = audioFeedback.GetChallengeResultClipLength(lastCardIsQ) + 0.1f;
            StartCoroutine(CoPlayWinLoseAfterDelay(winnerActor, delay));
        }
        if (PhotonNetwork.IsMasterClient && handResetManager != null)
        {
            handResetManager.RequestResetAllFromMaster();
        }
    }

    /// <summary>
    /// PlayZoneDetector 调用这里
    /// </summary>
    public void RequestPlayCard(string cardValue, GameObject cardObject)
    {
        if (roundEnded) return;
        Debug.Log($"[Round] RequestPlayCard called with cardValue={cardValue} and isMIne={photonView.IsMine}");
        if (!PhotonNetwork.InRoom) return;
        if (PhotonNetwork.LocalPlayer == null) return;
        if (cardObject == null) return;
        Debug.Log($"[Round] RequestPlayCard 2");
        PhotonView cardPv = cardObject.GetComponent<PhotonView>();
        if (cardPv == null || !cardPv.IsMine)
        {
            // 只能提交本地自己的牌
            return;
        }
        Debug.Log($"[Round] RequestPlayCard 3");
        int actor = PhotonNetwork.LocalPlayer.ActorNumber;
        int cardViewId = cardPv.ViewID;

        if (verboseLog) Debug.Log($"[Round] RequestPlayCard actor={actor}, card={cardValue}, viewId={cardViewId}");

        // 发给 Master 做权威确认
        photonView.RPC(nameof(RPC_PlayCardOnMaster), RpcTarget.MasterClient, actor, cardValue, cardViewId);
    }

    [PunRPC]
    private void RPC_PlayCardOnMaster(int actor, string cardValue, int cardViewId, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 安全校验：发送者和actor一致
        if (info.Sender == null || info.Sender.ActorNumber != actor) return;

        // 找到这张牌
        PhotonView cardPv = PhotonView.Find(cardViewId);
        if (cardPv == null) return;

        // 防重复触发（同一张牌进检测区多次）
        if (!committedCardViewIds.Add(cardViewId)) return;
        EnsureHandCountInitializedOnMaster();
        if (!handCountByActor.ContainsKey(actor)) handCountByActor[actor] = 5;
        handCountByActor[actor] = Mathf.Max(0, handCountByActor[actor] - 1);

        // 广播给所有人：确认这手牌生效
        photonView.RPC(nameof(RPC_PlayCommittedAll), RpcTarget.All, actor, cardValue, cardViewId);

        if (handCountByActor[actor] == 0)
        {
            roundEnded = true;
            photonView.RPC(nameof(RPC_RoundWinByEmptyHandAll), RpcTarget.All, actor);
            if (handResetManager != null)
                handResetManager.RequestResetAllFromMaster();
            photonView.RPC(nameof(RPC_ResetRoundStateAll), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_PlayCommittedAll(int actor, string cardValue, int cardViewId)
    {
        LastPlayedActor = actor;
        LastPlayedCard = cardValue;
        HasPendingPlay = true;

        if (verboseLog) Debug.Log($"[Round] PlayCommitted actor={actor}, card={cardValue}, viewId={cardViewId}");

        // 可选：把牌固定在桌面，避免继续乱飞
        //PhotonView cardPv = PhotonView.Find(cardViewId);
        //if (cardPv != null)
        //{
        //    Rigidbody rb = cardPv.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        rb.linearVelocity = Vector3.zero;
        //        rb.angularVelocity = Vector3.zero;
        //        rb.isKinematic = true;
        //    }
        //}

        // 第三步我们会在这里接“玩家已出牌”音效
        if (audioFeedback != null)
        {
            Debug.Log("[Round] Playing card submitted ");
            audioFeedback.PlayCardSubmittedSfx();
        }
    }
    public void RequestChallenge()
    {
        if (roundEnded) return;
        Debug.Log($"[Round] RequestChallenge called and isMine={photonView.IsMine}");
        if (!PhotonNetwork.InRoom) return;
        if (!HasPendingPlay) return;
        if (PhotonNetwork.LocalPlayer == null) return;
        //if (PhotonNetwork.LocalPlayer.ActorNumber == LastPlayedActor) return;

        photonView.RPC(nameof(RPC_ChallengeOnMaster), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        Debug.Log($"[Round] RequestChallenge sent RPC to master with actor={PhotonNetwork.LocalPlayer.ActorNumber}");
    }

    private void EnsureHandCountInitializedOnMaster()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (!handCountByActor.ContainsKey(p.ActorNumber))
                handCountByActor[p.ActorNumber] = 5;
        }
    }

    private void ResetRoundStateOnMaster()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        committedCardViewIds.Clear();
        var keys = new List<int>(handCountByActor.Keys);
        foreach (var k in keys) handCountByActor[k] = 5;

        LastPlayedActor = -1;
        LastPlayedCard = "";
        HasPendingPlay = false;
        roundEnded = false;
    }

    [PunRPC]
    private void RPC_RoundWinByEmptyHandAll(int winnerActor)
    {
        int local = PhotonNetwork.LocalPlayer?.ActorNumber ?? -1;
        bool isWinner = local == winnerActor;

        Debug.Log($"[Round] HandEmpty Winner={winnerActor}, local={local}, isWinner={isWinner}");


        // 这里放你后续动画入口：
        // if (isWinner) play win animation
        // else play lose animation
        if (audioFeedback != null)
        {
            audioFeedback.PlayWinLoseSfx(isWinner);
        }
        //endGameManager.PlayEndGame(isWinner);
        if (effectsManager != null) effectsManager.PlayEndGameEffects(isWinner);
    }

    [PunRPC]
    private void RPC_ResetRoundStateAll()
    {
        LastPlayedActor = -1;
        LastPlayedCard = "";
        HasPendingPlay = false;

        if (PhotonNetwork.IsMasterClient)
            ResetRoundStateOnMaster();
    }
    private IEnumerator CoPlayWinLoseAfterDelay(int winnerActor, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (audioFeedback == null)
            yield break;

        int localActor = PhotonNetwork.LocalPlayer?.ActorNumber ?? -1;
        bool isWinner = localActor == winnerActor;

        audioFeedback.PlayWinLoseSfx(isWinner);
        //endGameManager.PlayEndGame(isWinner);
        if (effectsManager != null) effectsManager.PlayEndGameEffects(isWinner);
    }
}