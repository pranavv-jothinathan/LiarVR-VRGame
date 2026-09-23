using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class RoomManager2 : MonoBehaviourPunCallbacks
{
    [Header("放在 Resources 里的玩家预制体名")]
    public string playerPrefabName = "Player";

    [Header("2个出生点：0号给第一个玩家，1号给第二个玩家")]
    public Transform[] spawnPoints;

    [Header("手牌预制体（0给第一个玩家，1给第二个玩家）")]
    [SerializeField] private GameObject handPrefab;
    [Header("手牌出生点（0给第一个玩家，1给第二个玩家）")]
    [SerializeField] private Transform[] handSpawnPoints;
    private GameObject _myHand;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2
        };

        PhotonNetwork.JoinOrCreateRoom("test", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (spawnPoints == null || spawnPoints.Length < 2)
        {
            Debug.LogError("请在 Inspector 里设置 2 个 spawnPoints。");
            return;
        }

        // 按 ActorNumber 排序，得到自己在房间中的稳定索引
        var orderedPlayers = PhotonNetwork.PlayerList
            .OrderBy(p => p.ActorNumber)
            .ToArray();

        int myIndex = System.Array.FindIndex(orderedPlayers, p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        // 双人房间：0或1；做个保护避免越界
        int spawnIndex = Mathf.Clamp(myIndex, 0, spawnPoints.Length - 1);

        Transform spawn = spawnPoints[spawnIndex];
        PhotonNetwork.Instantiate(playerPrefabName, spawn.position, spawn.rotation);
        SpawnLocalHand(myIndex);
    }
    // ====== 3) 追加到 RoomManager2 类末尾 ======
    private void SpawnLocalHand(int myIndex)
    {
        if (handPrefab == null) return;

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (handSpawnPoints != null && handSpawnPoints.Length > 0)
        {
            int handSpawnIndex = Mathf.Clamp(myIndex, 0, handSpawnPoints.Length - 1);
            Transform handSpawn = handSpawnPoints[handSpawnIndex];
            if (handSpawn != null)
            {
                pos = handSpawn.position;
                rot = handSpawn.rotation;
            }
        }

        _myHand = PhotonNetwork.Instantiate(handPrefab.name, pos, rot);
    }

    // UI按钮可绑定这个方法

}