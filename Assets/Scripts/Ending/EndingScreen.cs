using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingScreen : MonoBehaviour
{
    [System.Serializable]
    private struct EndingTextData {
        public TextMeshProUGUI tmpText;
        public float fadeTime;
        public float showingTime;
    }

    [SerializeField] private EndingTextData[] endingTextsData;
    [SerializeField] private GameObject continueText;
    [SerializeField] private float waitingTimeBeforeStart = 1.0f;
    [SerializeField] private float waitingTimeBeforeEnding = 1.0f;
    [SerializeField] private float fadeOutTime = 1.5f;

    private void Start()
    {
        continueText.SetActive(false);

        if (endingTextsData.Length > 0)
        {
            foreach (EndingTextData textData in endingTextsData)
            {
                Color currentColor = textData.tmpText.color;
                textData.tmpText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f);

                if (!textData.tmpText.gameObject.activeSelf)
                    textData.tmpText.gameObject.SetActive(true);
            }
                
            
            StartCoroutine(ShowEndingTexts());
        }
    }

    private IEnumerator ShowEndingTexts()
    {
        yield return new WaitForSeconds(waitingTimeBeforeStart);

        foreach (EndingTextData textData in endingTextsData)
        {
            yield return AnimateEndingText(textData.tmpText, textData.fadeTime);

            if (textData.showingTime > 0.0f)
                yield return new WaitForSeconds(textData.showingTime);
        }

        yield return new WaitForSeconds(waitingTimeBeforeEnding);

        StartCoroutine(HideEndingTexts());
    }

    private IEnumerator HideEndingTexts()
    {
        float currentAlpha = 1.0f;
        float alphaDecreasePerSecond = 1.0f / fadeOutTime;

        while (currentAlpha > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            currentAlpha -= Time.deltaTime * alphaDecreasePerSecond;

            if (currentAlpha < 0.0f)
                currentAlpha = 0.0f;

            foreach (EndingTextData textData in endingTextsData)
            {
                Color color = textData.tmpText.color;
                textData.tmpText.color = new Color(color.r, color.g, color.b, currentAlpha);
            }
        }

        continueText.SetActive(true);
    }

    private IEnumerator AnimateEndingText(TextMeshProUGUI tmpText, float fadeTime)
    {
        Color color = tmpText.color;
        float currentAlpha = 0.0f;
        float alphaIncreasePerSecond = 1.0f / fadeTime;

        while(currentAlpha < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            currentAlpha += Time.deltaTime * alphaIncreasePerSecond;

            if (currentAlpha > 1.0f)
                currentAlpha = 1.0f;

            tmpText.color = new Color(color.r, color.g, color.b, currentAlpha);
        }
    }

}
