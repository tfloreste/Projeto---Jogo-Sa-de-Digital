using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreChangeTextSpawner : MonoBehaviour
{

    [SerializeField] private TMP_FontAsset tmpFont;
    [SerializeField] private float fontSize;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Canvas canvas;

    private Queue<TextMeshProUGUI> textInstancesQueue;

    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI test = new TextMeshProUGUI();
    }

    public void InstaciateAtPosition(string text, Vector3 position)
    {
        InstaciateAtPosition(text, position, defaultColor);
    }

    public void InstaciateAtPosition(string text, Vector3 position, Color color)
    {
        TextMeshProUGUI textInstance = GetTextInstance();
        textInstance.color = color;

        GameObject emptyGameObject = Instantiate(new GameObject(), position, Quaternion.identity, canvas.transform);
        emptyGameObject.AddComponent<TextMeshProUGUI>();
    }

    private TextMeshProUGUI GetTextInstance()
    {
        if(textInstancesQueue.Count == 0)
        {
            CreateInstance();
        }

        return textInstancesQueue.Dequeue();
    }

    private void CreateInstance()
    {
        TextMeshProUGUI newInstance = new TextMeshProUGUI();
        newInstance.font = tmpFont;
        newInstance.fontSize = fontSize;

        textInstancesQueue.Enqueue(newInstance);
    }
}
