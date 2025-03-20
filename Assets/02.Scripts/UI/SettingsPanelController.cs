using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//똑같이 패널 컨트롤러 상속
public class SettingsPanelController : PanelController
{
    /// <summary>
    /// SFX On/Off시 호출되는 함수
    /// </summary>
    /// <param name="value">On/Off 값</param>
    public void OnSFXToggleValueChanged(bool value)
    {
        
    }
    
    /// <summary>
    /// BGM On/Off시 호출되는 함수
    /// </summary>
    /// <param name="value">On/Off 값</param>
    public void OnBGMToggleValueChanged(bool value)
    {
        
    }

    /// <summary>
    /// X 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        Hide();
    }
}