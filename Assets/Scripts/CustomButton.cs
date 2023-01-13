using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class CustomButton : MonoBehaviour, IPointerDownHandler
{

    [SerializeField] private AudioClip buttonPressedSound;

    private Button button;
    private Image buttonImage;
    private Color btnColor;

    // Start is called before the first frame update
    private void OnEnable()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        btnColor = buttonImage.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable && buttonPressedSound)
        {
            SFXManager.GetInstance().PlayClip(buttonPressedSound);
        }
    }

    public void Disable()
    {
        buttonImage.color = new Color(btnColor.r, btnColor.g, btnColor.b, 0.4f);
        button.interactable = false;   
    }

    public void Enable()
    { 
        buttonImage.color = new Color(btnColor.r, btnColor.g, btnColor.b, 1f);
        button.interactable = true;
    }

    public Button GetButtonInstance()
    {
        return button;
    }
   
}
