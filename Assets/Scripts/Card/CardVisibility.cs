using Photon.Pun;
using UnityEngine;

public class CardVisibility : MonoBehaviourPun
{
    public GameObject front;
    public GameObject back;

    private bool isRevealed = false;

    void Start()
    {
        UpdateView();
    }

    public void RevealCard()
    {
        photonView.RPC("RPC_Reveal", RpcTarget.All);
    }

    [PunRPC]
    void RPC_Reveal()
    {
        isRevealed = true;
        UpdateView();
    }

    void UpdateView()
    {
        if (photonView.IsMine)
        {
            front.SetActive(true);
            back.SetActive(false);
        }
        else
        {
            front.SetActive(isRevealed);
            back.SetActive(!isRevealed);
        }
    }
}