using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPFaderEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;


    public Coroutine FadeIn(float fadeTime)
    {
        return StartCoroutine(PerformFade(fadeTime, 1.0f));
    }

    public Coroutine FadeOut(float fadeTime)
    {
        return StartCoroutine(PerformFade(fadeTime, 0.0f));
    }

    private IEnumerator PerformFade(float fadeTime, float finalAlpha)
    {
        if (fadeTime <= 0.0f || Mathf.Approximately(textMeshPro.color.a, finalAlpha))
            yield break;

        float startAlpha = textMeshPro.color.a;
        float currentAlpha = textMeshPro.color.a;
        float alphaChangePerSecond = (finalAlpha - currentAlpha) / fadeTime;

        while (
            (startAlpha < finalAlpha && currentAlpha < finalAlpha) || 
            (startAlpha > finalAlpha && currentAlpha > finalAlpha)
        )
        {
            yield return null;
            currentAlpha += alphaChangePerSecond * Time.deltaTime;

            if (
                (startAlpha < finalAlpha && currentAlpha > finalAlpha) ||
                (startAlpha > finalAlpha && currentAlpha < finalAlpha)
            )
            {
                currentAlpha = finalAlpha;
            }

            textMeshPro.color = new Color(
                textMeshPro.color.r, 
                textMeshPro.color.g, 
                textMeshPro.color.b, 
                currentAlpha
             );
        }
    }
}
