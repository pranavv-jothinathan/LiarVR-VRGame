using UnityEngine;
using Photon.Pun;

public class PlayAudioFeedback : MonoBehaviourPun
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip cardPlayedClip;
    [SerializeField] private AudioClip challengeSuccessClip; // card king
    [SerializeField] private AudioClip challengeFailClip;    // sadness

    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // roundmanager to use 
    public void PlayCardSubmittedSfx()
    {
        Debug.Log("PlayCardSubmittedSfx called");
        //debug the called of feedbackaudio
        audioSource.PlayOneShot(cardPlayedClip);
        Debug.Log(" Played card submitted sound effect");
    }
    public void PlayChallengeResultSfx(bool lastCardIsQ)
    {
        if (audioSource == null) return;
        AudioClip clip = lastCardIsQ ? challengeFailClip : challengeSuccessClip;
        Debug.Log($" Sfx called with lastCardIsQ={lastCardIsQ}, playing clip={clip?.name}");  
        if (clip != null) audioSource.PlayOneShot(clip);
    }
    public float GetChallengeResultClipLength(bool lastCardIsQ)
    {
        AudioClip clip = lastCardIsQ ? challengeFailClip : challengeSuccessClip;
        return clip != null ? clip.length : 0f;
    }

    public void PlayWinLoseSfx(bool isWinner)
    {
        if (audioSource == null) return;
        AudioClip clip = isWinner ? winClip : loseClip;
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
