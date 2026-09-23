using UnityEngine;

public class ChallengeButtonTrigger : MonoBehaviour
{
    [SerializeField] private RoundGameManager roundGameManager;

    public void OnChallengePressed()
    {
        Debug.Log("Button trigger called");
        if (roundGameManager == null) return;
        roundGameManager.RequestChallenge();
    }
}