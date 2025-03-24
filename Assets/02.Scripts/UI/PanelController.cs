using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; //가벼운 애니메이션을 적용시켜주는 외부 에셋.

//캔버스 그룹을 반드시 요구, 없을시 자동 추가
[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour
{
    [SerializeField] private RectTransform panelRectTransform;      // 팝업창
    
    private CanvasGroup _backgroundCanvasGroup;                     // 뒤에 시커먼 배경
    
    
    //델리게이트 타입 선언. 패널을 숨길때 실행할 함수
    public delegate void PanelControllerHideDelegate();
    
    private void Awake()
    {
    	//게임 오브젝트가 생성될 때 CanvasGroup 컴포넌트를 가져와
        //_backgroundCanvasGroup 변수에 저장
        _backgroundCanvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Panel 표시 함수
    /// </summary>
    public void Show()
    {
    	//패널투명도와 크기를 처음에 0으로 만들어 보이지 않게함 
        _backgroundCanvasGroup.alpha = 0;
        panelRectTransform.localScale = Vector3.zero;
        
        //이후 투명도와 크기를 0.3초에 걸쳐 1로 만드는
        //애니메이션 메서드 (DG.Tweening) 실행
        _backgroundCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear);
        panelRectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Panel 숨기기 함수
    /// </summary>
    //'=null'을 넣어서 선택적으로 아까선언했던 델리게이트 타입의
    //hideDelegate(콜백 함수)를 실행할 수 있음
    public void Hide(PanelControllerHideDelegate hideDelegate = null)
    {
    	//처음에 패널투명도와 크기를 1로 만들어 보이게 함
        _backgroundCanvasGroup.alpha = 1;
        panelRectTransform.localScale = Vector3.one;
        
        //이후 투명도와 크기를 0.3초에 걸쳐 0으로 만드는
        //애니메이션 메서드 (DG.Tweening) 실행
        _backgroundCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear);
        panelRectTransform.DOScale(0, 0.3f)
            .SetEase(Ease.InBack).OnComplete(() =>
            {
                hideDelegate?.Invoke();
                Destroy(gameObject);
            });
            //.OnComplete(() => { ... })는 애니메이션이 끝난 후 특정 동작 실행하게함
            //여기선 hideDelegate?.Invoke();로 델리게이트를 받으면 실행하고
            //destroy로 삭제까지 함
    }
}