using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class VrEndTurnButton : MonoBehaviourPun
{
    [SerializeField] PhotonView playerPhotonView;
    [SerializeField] Button endTurnButton;

    void Awake()
    {
        
        playerPhotonView = GetComponentInParent<PhotonView>();
        endTurnButton = GetComponent<Button>();
    }

    void OnEnable()
    {
        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
    }

    void OnDisable()
    {
        if (endTurnButton != null)
            endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
    }

    void Update()
    {
        if (playerPhotonView == null || TurnManager.Instance == null || endTurnButton == null)
            return;

        //use photon is mine to judge
        bool show = playerPhotonView.IsMine;
        endTurnButton.interactable =
            show && TurnManager.Instance.IsLocalPlayersTurn();
    }

    void OnEndTurnClicked()
    {
        if (playerPhotonView == null || !playerPhotonView.IsMine)
            return;
        if (TurnManager.Instance == null || !TurnManager.Instance.IsLocalPlayersTurn())
            return;

        TurnManager.Instance.RequestEndTurn();
    }
}