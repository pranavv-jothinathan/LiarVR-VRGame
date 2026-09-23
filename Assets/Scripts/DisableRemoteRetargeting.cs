using Photon.Pun;
using UnityEngine;

/// <summary>仅本地玩家启用 Movement/Retarget；远端走网络 Transform。</summary>
public class DisableRemoteRetargeting : MonoBehaviourPun
{
    [Tooltip("只在本地玩家上保持启用，例如 Character Retargeter、Body 相关驱动等")]
    public Behaviour[] localOnlyBehaviours;

    void Start()
    {
        if (photonView.IsMine)
            return;

        foreach (var b in localOnlyBehaviours)
        {
            if (b != null)
                b.enabled = false;
        }
    }
}