using UnityEngine;
using System.Collections;
using TMPro;

public class WinEffectsManager : MonoBehaviour
{
    [Header("Animations")]
    public ParticleSystem fireAnimation;
    public ParticleSystem smokeAnimation;

    [Header("DisplayDuration")]
    public float displayTime = 15f;

    [Header("User Interface")]
    public GameObject winTextCanvas;
    public GameObject loseTextCanvas;

    [Header("Animation Settings")]
    public float animationSpeed = 2f;


    void Awake()
    {
        if (fireAnimation != null) fireAnimation.Stop();
        if (smokeAnimation != null) smokeAnimation.Stop();
        if (winTextCanvas != null) winTextCanvas.SetActive(false);
        if (loseTextCanvas != null) loseTextCanvas.SetActive(false);
    }

    public void PlayEndGameEffects(bool isWinner, bool isOppositeSide = false)
    {
        StopAllCoroutines();

        if (fireAnimation != null) fireAnimation.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (smokeAnimation != null) smokeAnimation.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        GameObject activeCanvas = isWinner ? winTextCanvas : loseTextCanvas;

        if (activeCanvas != null)
        {
            float yRotation = isOppositeSide ? 180f : 0f;                                      // To rotate the canvas
            activeCanvas.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        }

        if (isWinner)
        {
            if (fireAnimation != null) fireAnimation.Play();
            if (winTextCanvas != null) StartCoroutine(AnimateUI(winTextCanvas, true));
            Debug.Log("Playing Fire for Winner at table center.");
        }
        else
        {
            if (smokeAnimation != null) smokeAnimation.Play();
            if (loseTextCanvas != null) StartCoroutine(AnimateUI(loseTextCanvas, true));
            Debug.Log("Playing Smoke for Loser at table center.");
        }
        StartCoroutine(TimerToRestart());
    }

    private IEnumerator TimerToRestart()
    {
        yield return new WaitForSeconds(displayTime);
        ResetEffects();
    }

    private IEnumerator AnimateUI(GameObject uiObject, bool appearing)
    {
        Vector3 startScale = appearing ? Vector3.zero : Vector3.one;
        Vector3 endScale = appearing ? Vector3.one : Vector3.zero;

        float elapsed = 0;
        float duration = 1.0f;

        uiObject.transform.localScale = startScale;
        uiObject.SetActive(true);

        while (elapsed < duration)
        {
            uiObject.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        uiObject.transform.localScale = endScale;

        if (!appearing) uiObject.SetActive(false);
    }

    public void ResetEffects()
    {
        if (fireAnimation != null) fireAnimation.Stop();
        if (smokeAnimation != null) smokeAnimation.Stop();

        if (winTextCanvas != null && winTextCanvas.activeSelf)
            StartCoroutine(AnimateUI(winTextCanvas, false));

        if (loseTextCanvas != null && loseTextCanvas.activeSelf)
            StartCoroutine(AnimateUI(loseTextCanvas, false));
        Debug.Log("End game effects clearing with animation.");
    }
}