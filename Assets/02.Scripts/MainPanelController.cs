using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] TMP_Text creditsText;
    [SerializeField] TMP_Text nameText;
    
    private void Start()
    {
        creditsText.text = "Credits: " + UserInformations.Credits;
        
        int quotient = UserInformations.Rankpoints / 3;

        // 18급부터 1급까지 계산
        int rank = 18 - quotient;

        // 최소 1급, 최대 18급 제한
        if (rank < 1) rank = 1;
        if (rank > 18) rank = 18;

        nameText.text = $"{rank}급 {UserInformations.Nickname}";
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