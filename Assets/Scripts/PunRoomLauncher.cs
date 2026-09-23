using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PunRoomLauncher : MonoBehaviourPunCallbacks
{
    private const string LogTag = "[PUN][Launcher]";

    [Header("Connection")]
    [SerializeField] private string gameVersion = "0.1";
    [SerializeField] private string roomName = "vr-room";
    [SerializeField] private byte maxPlayers = 2;
    [SerializeField] private bool autoConnectOnStart = true;
    [SerializeField] private bool verboseLog = true;

    [Header("Spawn")]
    [Tooltip("Must be the prefab name under Resources, e.g. \"KyleRobot\".")]
    [SerializeField] private string playerPrefabName = "KyleRobot";
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        if (!autoConnectOnStart)
            return;

        ConnectAndJoin();
    }

    [ContextMenu("Connect And Join")]
    public void ConnectAndJoin()
    {
        Log($"ConnectAndJoin called. inRoom={PhotonNetwork.InRoom} connected={PhotonNetwork.IsConnected}");

        if (PhotonNetwork.InRoom)
            return;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;

        if (PhotonNetwork.IsConnected)
        {
            Log("Already connected to master. Joining room directly.");
            JoinOrCreateTargetRoom();
            return;
        }

        Log("Calling PhotonNetwork.ConnectUsingSettings()");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Log($"OnConnectedToMaster. region={PhotonNetwork.CloudRegion} server={PhotonNetwork.ServerAddress}");
        JoinOrCreateTargetRoom();
    }

    private void JoinOrCreateTargetRoom()
    {
        var options = new RoomOptions { MaxPlayers = maxPlayers };
        Log($"JoinOrCreateRoom. room={roomName} maxPlayers={maxPlayers} gameVersion={gameVersion}");
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Log($"OnJoinedRoom. room={PhotonNetwork.CurrentRoom?.Name} actor={PhotonNetwork.LocalPlayer?.ActorNumber} " +
            $"playerCount={PhotonNetwork.CurrentRoom?.PlayerCount} players={GetPlayerList()}");
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
    {
        Transform spawn = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            var index = Mathf.Abs(PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
            spawn = spawnPoints[index];
        }

        var position = spawn != null ? spawn.position : Vector3.zero;
        var rotation = spawn != null ? spawn.rotation : Quaternion.identity;

        var spawned = PhotonNetwork.Instantiate(playerPrefabName, position, rotation, 0);
        var view = spawned != null ? spawned.GetComponent<PhotonView>() : null;
        Log($"SpawnLocalPlayer done. prefab={playerPrefabName} pos={position} rotY={rotation.eulerAngles.y:F1} " +
            $"spawned={(spawned != null ? spawned.name : "null")} viewId={(view != null ? view.ViewID : -1)}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"{LogTag} disconnected: {cause}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log($"OnPlayerEnteredRoom. actor={newPlayer.ActorNumber} nick={newPlayer.NickName} playerCount={PhotonNetwork.CurrentRoom?.PlayerCount}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log($"OnPlayerLeftRoom. actor={otherPlayer.ActorNumber} nick={otherPlayer.NickName} playerCount={PhotonNetwork.CurrentRoom?.PlayerCount}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"{LogTag} OnJoinRoomFailed code={returnCode} msg={message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"{LogTag} OnCreateRoomFailed code={returnCode} msg={message}");
    }

    private void Log(string message)
    {
        if (!verboseLog)
            return;
        Debug.Log($"{LogTag} {message}");
    }

    private static string GetPlayerList()
    {
        if (PhotonNetwork.PlayerList == null || PhotonNetwork.PlayerList.Length == 0)
            return "[]";

        var parts = new string[PhotonNetwork.PlayerList.Length];
        for (var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var p = PhotonNetwork.PlayerList[i];
            parts[i] = $"{p.ActorNumber}:{p.NickName}";
        }

        return "[" + string.Join(", ", parts) + "]";
    }
}
