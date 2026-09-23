using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private const string BuildTag = "RM_DEBUG_2026_04_20_B";
    public GameObject player;
    [SerializeField] private string debugPlayerRootName = "RobotKyle";

    [Tooltip("所有玩家在同一出生点生成")]
    public Transform spawnPoint;

    void Start()
    {
        Debug.Log($"[{BuildTag}] Connecting..room");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log($"[{BuildTag}] Connected to server room");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log($"[{BuildTag}] We're in the lobby room");
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log($"[{BuildTag}] We're connected and in a room!");
        Debug.Log($"players={PhotonNetwork.CurrentRoom.PlayerCount} actors={string.Join(",", PhotonNetwork.PlayerList.Select(p => p.ActorNumber))}");

        Transform spawn = spawnPoint != null ? spawnPoint : transform;
        var go = PhotonNetwork.Instantiate(player.name, spawn.position, spawn.rotation);
        var view = go != null ? go.GetComponent<PhotonView>() : null;
        Debug.Log($"player instantiated at {spawn.position}");
        Debug.Log($"local spawned viewId={(view != null ? view.ViewID : -1)} isMine={(view != null && view.IsMine)} ownerActor={(view != null ? view.OwnerActorNr : -1)}");

        DumpPlayerViews("OnJoinedRoom immediate");
        StartCoroutine(DumpAfterDelay("OnJoinedRoom +1s", 1f));
    }

    void Update()
    {
        if (Time.frameCount % 120 == 0)
            Debug.Log($"room PUN state={PhotonNetwork.NetworkClientState}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"OnPlayerEnteredRoom actor={newPlayer.ActorNumber} count={PhotonNetwork.CurrentRoom.PlayerCount}");
        StartCoroutine(DumpAfterDelay("OnPlayerEnteredRoom +1s", 1f));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"room OnDisconnected cause={cause}");
        base.OnDisconnected(cause);
    }

    private IEnumerator DumpAfterDelay(string reason, float delay)
    {
        yield return new WaitForSeconds(delay);
        DumpPlayerViews(reason);
    }

    private void DumpPlayerViews(string reason)
    {
        var views = FindObjectsOfType<PhotonView>(true)
            .Where(v => v != null && v.gameObject != null && v.gameObject.name.StartsWith(debugPlayerRootName))
            .OrderBy(v => v.ViewID)
            .ToList();

        if (views.Count == 0)
        {
            Debug.LogWarning($"[DumpPlayerViews] {reason} -> no objects found with prefix '{debugPlayerRootName}'");
            return;
        }

        foreach (var v in views)
        {
            Debug.Log($"[DumpPlayerViews] {reason} name={v.gameObject.name} viewId={v.ViewID} isMine={v.IsMine} ownerActor={v.OwnerActorNr}");
        }
    }
}