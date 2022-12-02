using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTransparencyAnimation : MonoBehaviour
{
    [SerializeField] float minTransparency = 0.5f;
    [SerializeField] float maxTransparency = 1f;
    [SerializeField] float animationSpeed = 0.01f;

    private Text text;
    private bool increasingTransparency;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        SetTransparency(maxTransparency);
        increasingTransparency = false;

        if (maxTransparency > 1.0f)
            maxTransparency = 1.0f;

        if (minTransparency < 0.0f)
            minTransparency = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float newTransparency = increasingTransparency ?
            text.color.a + (animationSpeed * Time.deltaTime) :
            text.color.a - (animationSpeed * Time.deltaTime);

        if (newTransparency >= maxTransparency)
        {
            newTransparency = maxTransparency;
            increasingTransparency = false;
        }
        else if (newTransparency < minTransparency)
        {
            newTransparency = minTransparency;
            increasingTransparency = true;
        }

        Debug.Log("newTransparency: " + newTransparency);

        SetTransparency(newTransparency);
    }

    void SetTransparency(float transparency)
    {
        Color color = text.color;
        color.a = maxTransparency;

        text.color = color;
    }
}
