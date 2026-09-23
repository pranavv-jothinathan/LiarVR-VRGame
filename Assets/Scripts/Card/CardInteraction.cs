using Photon.Pun;
using UnityEngine;

public class CardInteraction : MonoBehaviourPun
{
    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.F)) // test key
        {
            Flip();
        }
    }

    void Flip()
    {
        transform.Rotate(0, 180, 0);

        GetComponent<CardVisibility>().RevealCard();
    }
}