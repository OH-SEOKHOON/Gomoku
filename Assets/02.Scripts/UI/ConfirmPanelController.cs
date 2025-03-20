using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;


//PanelController를 상속
public class ConfirmPanelController : PanelController
{
    //확인창에 표시될 메시지를 담을 tmp_text오브젝트
    [SerializeField] private TMP_Text messageText;

    //확인 버튼이 클릭되었을 때 실행될 함수를 저장할 델리게이트 선언
    public delegate void OnConfirmButtonClick();
    private OnConfirmButtonClick onConfirmButtonClick;

    //이 패널이 보여질때 실행될 메서드, PanelController의 Show()랑 다른것(오버라이딩 아님!!)
    public void Show(string message, OnConfirmButtonClick onConfirmButtonClick, OnHide onHide)
    {
        //전달된 메시지를 UI 텍스트에 표시
        messageText.text = message;
        
        //확인 버튼 클릭 시 실행할 델리게이트에 파라미터로 받은 메서드 저장
        this.onConfirmButtonClick = onConfirmButtonClick;
        
        //부모오브젝트의 show메서드 실행
        base.Show(onHide);
    }
    
    /// <summary>
    /// Confirm 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickConfirmButton()
    {
        //저장된 델리게이트 함수를 실행하고 숨김처리
        onConfirmButtonClick?.Invoke();
        Hide();
    }

    /// <summary>
    /// X 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        //뭐 없이 바로 숨김
        Hide();
    }
}