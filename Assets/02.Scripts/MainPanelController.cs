using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] TMP_Text creditsText;
    
    private void Start()
    {
        creditsText.text = "Credits: " + UserInformations.Credits;
    }

    public void OnClickPlayButton()
    {
        GameManager.Instance.ChangeToGameScene();
    }
    
    public void OnClickReplayButton()
    {
        
    }
    
    public void OnClickRankingButton()
    {
        
    }
    
    public void OnClickShopButton()
    {
        
    }
    
    public void OnClickSettingsButton()
    {
        
    }
    
    public void OnClickProfileButton()
    {
        
    }
}