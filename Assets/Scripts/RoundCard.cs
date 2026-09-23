using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class RoundCard : MonoBehaviour
{
    [SerializeField] string cardValue = "CARD_01";
    public string CardValue => cardValue;

    [SerializeField] GameObject playAreaObject; // Inspector 里拖“出牌区物体”
    Area playArea;

    readonly HashSet<Collider> _playTriggers = new HashSet<Collider>();
    XRGrabInteractable _grab;

    void Awake()
    {
        if (playAreaObject != null)
            playArea = playAreaObject.GetComponent<Area>();

        _grab = GetComponent<XRGrabInteractable>();
        _grab.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        if (_grab != null) _grab.selectExited.RemoveListener(OnReleased);
    }

    public void RegisterPlayTrigger(Collider c)
    {
        if (c != null) _playTriggers.Add(c);
    }

    public void UnregisterPlayTrigger(Collider c)
    {
        if (c != null) _playTriggers.Remove(c);
    }

    void OnReleased(SelectExitEventArgs _)
    {
        if (!PhotonNetwork.InRoom) return;
        if (playArea == null) return;
        if (!playArea.IsInsidePlayArea(transform.position)) return;

        playArea.TrySubmitCard(this);
    }

    public void SetCardValue(string v) => cardValue = v;
}