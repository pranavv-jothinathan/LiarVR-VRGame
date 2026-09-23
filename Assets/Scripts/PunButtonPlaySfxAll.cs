using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PunButtonPlaySfxAll : MonoBehaviourPun
{
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip clip;

    public void OnButtonPressed()
    {
        if (!PhotonNetwork.InRoom || clip == null || sfxSource == null) return;
        photonView.RPC(nameof(RPC_PlayClip), RpcTarget.All);
    }

    [PunRPC]
    void RPC_PlayClip()
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}