using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMainMenu : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private Image returnButtonImage;
    [SerializeField] private Animator confirmPopupAnimator;
    [SerializeField] private Animator UiBlockerAnimator;
    [SerializeField] private SceneChanger sceneChanger;

    [Header("SFX")]
    [SerializeField] private AudioClip openPopupButtonSound;
    [SerializeField] private AudioClip cancelButtonSound;
    [SerializeField] private AudioClip confirmButtonSound;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator playerAnimator;

    private PlayerSwipeController playerController;

    private void Start()
    {
        playerController = playerTransform.gameObject.GetComponent<PlayerSwipeController>();
        UiBlockerAnimator.gameObject.SetActive(false);
    }

    public void ShowConfirmPopup()
    {
        Debug.Log("Show confirm popup fired");

        if (openPopupButtonSound)
            SFXManager.GetInstance().PlayClip(openPopupButtonSound);

        playerController.BlockMovement();

        UiBlockerAnimator.gameObject.SetActive(true);
        UiBlockerAnimator.SetTrigger("Show");
        confirmPopupAnimator.SetTrigger("Open");
    }

    public void HideConfirmPopup()
    {
        if(cancelButtonSound)
            SFXManager.GetInstance().PlayClip(cancelButtonSound);

        playerController.UnblockMovement();

        StartCoroutine(HidePopupCO());
    }

    public void GoToMenu()
    {
        if (confirmButtonSound)
            SFXManager.GetInstance().PlayClip(confirmButtonSound);

        sceneChanger.SetFadeMode(true);
        sceneChanger.SetSavingMode(true);
        sceneChanger.SetSaveCurrentScene(true);
        sceneChanger.SetSavePlayerPosition(true);
        sceneChanger.SetSavePlayerDirection(true);

        sceneChanger.SetPlayerPositionVector(playerTransform.position);
        sceneChanger.SetPlayerDirection(GetPlayerDirection());

        sceneChanger.ChangeTo(mainMenuSceneName);
    }

    private IEnumerator HidePopupCO()
    {
        confirmPopupAnimator.SetTrigger("Close");
        UiBlockerAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);
        UiBlockerAnimator.gameObject.SetActive(false);
    }

    public void HideReturnButton()
    {
        Color btnColor = returnButtonImage.color;
        returnButtonImage.color = new Color(btnColor.r, btnColor.g, btnColor.b, 0.0f);
    }

    public void ShowReturnButton()
    {
        Color btnColor = returnButtonImage.color;
        returnButtonImage.color = new Color(btnColor.r, btnColor.g, btnColor.b, 1.0f);
    }

    private Direction GetPlayerDirection()
    {
        float xDirection = playerAnimator.GetFloat("xDirection");
        float yDirection = playerAnimator.GetFloat("yDirection");
        Direction playerDirection;

        if (xDirection > 0.0f)
            playerDirection = Direction.Right;
        else if (xDirection < 0.0f)
            playerDirection = Direction.Left;
        else if (yDirection > 0.0f)
            playerDirection = Direction.Up;
        else
            playerDirection = Direction.Down;

        return playerDirection;
    }   
}
