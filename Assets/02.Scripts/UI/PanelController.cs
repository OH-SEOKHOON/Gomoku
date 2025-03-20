using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PanelController : MonoBehaviour
{
    public bool IsShow { get; private set; }
    
    //hide메서드를 델리게이트 변수에 집어넣기위해 델리게이트 선언을 한다.
    public delegate void OnHide();
    private OnHide _onHideDelegate;
    
    private RectTransform _rectTransform;
    private Vector2 _hideAnchorPosition;

    //start함수를 awake로 변경
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _hideAnchorPosition = _rectTransform.anchoredPosition;
        IsShow = false;
    }

    /// <summary>
    /// Panel 표시 함수
    /// </summary>
    public void Show(OnHide onHideDelegate)
    {
        //선언했던 델리게이트함수에 파라미터로 받은 onHideDelegate함수 할당
        _onHideDelegate = onHideDelegate;
        
        _rectTransform.anchoredPosition = Vector2.zero;
        IsShow = true;
    }

    /// <summary>
    /// Panel 숨기기 함수
    /// </summary>
    public void Hide()
    {
        _rectTransform.anchoredPosition = _hideAnchorPosition;
        IsShow = false;
        
        //변수로 저장했던 델리게이트 함수 활성화
        _onHideDelegate?.Invoke();
    }
}