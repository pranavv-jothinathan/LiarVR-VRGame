using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RetargeterEnableByPhotonMine : MonoBehaviourPunCallbacks
{
    [Header("把 Character retargeter 组件拖到这里")]
    [SerializeField] private MonoBehaviour characterRetargeter;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        if (characterRetargeter == null)
        {
            Debug.LogWarning($"{name}: characterRetargeter 没有绑定。");
        }
    }

    private void Start()
    {
        RefreshRetargeterState();
    }

    // 当对象所有权发生变化时，重新判断一次
    

    private void RefreshRetargeterState()
    {
        if (characterRetargeter == null || pv == null) return;

        characterRetargeter.enabled = pv.IsMine;
    }
}