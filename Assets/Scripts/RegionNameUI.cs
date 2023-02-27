using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RegionNameUI : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Animator regionNameAnimator;
    [SerializeField] private TextMeshProUGUI regionText;
    [SerializeField] private float secondsShowingRegionName = 4.0f;
    [SerializeField] private float openCloseRegionUITime = 0.5f;

    private string currentRegion = "";
    private bool isShowingRegionName = false;

    private void Start()
    {
        isShowingRegionName = false;
    }

    public void SetRegionName(StringVariable newRegionName)
    {
        SetRegionName(newRegionName.Value);
    }

    private void SetRegionName(string newRegionName)
    {
        Debug.Log("setting regionName to: " + newRegionName);
        currentRegion = newRegionName;
    }

    public void ChangeRegion(StringVariable newRegionName)
    {
        if (currentRegion == newRegionName.Value)
            return;

        currentRegion = newRegionName.Value;

        if (isShowingRegionName)
        {
            CancelInvoke();
            CloseRegionText();
            Invoke("ShowCurrentRegion", openCloseRegionUITime);
            
            return;
        }

        ShowCurrentRegion();
    }

    public void ShowCurrentRegion()
    {
        regionText.text = currentRegion;
        regionNameAnimator.SetTrigger("Open");
        isShowingRegionName = true;

        Invoke("CloseRegionText", secondsShowingRegionName + openCloseRegionUITime);
    }

    private void CloseRegionText()
    {
        regionNameAnimator.SetTrigger("Close");
        isShowingRegionName = false;
    }

    public void LoadData(GameData data)
    {
        if (data != null)
            SetRegionName(data.currentRegionName);
    }

    public void SaveData(GameData data)
    {
        data.currentRegionName = currentRegion;
    }
}
