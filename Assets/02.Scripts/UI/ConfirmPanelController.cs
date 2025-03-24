using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class ConfirmPanelController : PanelController
{
    [SerializeField] private TMP_Text messageText;
    
    [SerializeField] private GameObject closebutton;

    public delegate void OnConfirmButtonClick();
    private OnConfirmButtonClick onConfirmButtonClick;

    //프리팹화 시킴에 따라 onHide 델리게이트 함수가 필요없으므로 삭제
    //(아까 패널 컨트롤러 스크립트에서도 지웠음)
    public void Show(string message, OnConfirmButtonClick onConfirmButtonClick, bool isshowclose = true)
    {
        messageText.text = message;
        this.onConfirmButtonClick = onConfirmButtonClick;

        if (!isshowclose)
            closebutton.SetActive(false);
        
        //부모 클래스(PanelController)의 Show() 실행
        base.Show();
        
        
    }
    
    /// <summary>
    /// Confirm 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickConfirmButton()
    {
        //Hide()메서드 실행후,
        //확인 버튼을 눌렀을 때 실행할 콜백(delegate) 정의.
        Hide(() => onConfirmButtonClick?.Invoke());
    }

    /// <summary>
    /// X 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        Hide();
    }
}