using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour
{
	//돌에 쓸 스프라이트를 참조할 변수
    [SerializeField] private Sprite black;
    [SerializeField] private Sprite white;
    [SerializeField] private Sprite ban;
    
    //돌이 그려질 스프라이트 렌더러 컴포넌트
    [SerializeField] private SpriteRenderer stoneSpriteRenderer;
    
    //조준점지점을 나타낼 스프라이터 렌더러 컴포넌트
    [SerializeField] private SpriteRenderer aimSpriteRenderer;
    
    //최근 착수지점을 나타낼 스프라이터 렌더러 컴포넌트
    [SerializeField] private SpriteRenderer lastPosSpriteRenderer;
    
    //조준에 쓸 스프라이트를 변수
    [SerializeField] private Sprite aimed;
    
    //최근 착수지점에 쓸 스프라이트 변수
    [SerializeField] private Sprite lastpos;
    

	//마커타입 enum변수. 마커없음, 흑, 백 타입으로 나뉨
    public enum MarkerType { None, black, white, ban }
    
    //조준점타입 enum 변수. 조준점 없음, 조준점, 최근 착수
    public enum AimType { None, Aimed }
    
    //착수지점 enum 변수
    public enum PlaceType { None, Last }
    
    //블록이 클릭되었을때 호출될 메서드를 델리게이트로 정의. 매개변수 int도 받도록시킴
    public delegate void OnBlockClicked(int index);
    
    //OnBlockClicked 타입의 이벤트를 정의.
    //이 이벤트는 블록이 클릭되었을 때 외부에서 등록한 메서드를 호출하는 데 사용
    public OnBlockClicked onBlockClicked;
    
    //블록의 고유 인덱스를 저장하는 변수
    private int _blockIndex;
    
    /// <summary>
    /// Block 초기화 함수
    /// </summary>
    /// <param name="blockIndex">Block 인덱스</param>
    /// <param name="onBlockClicked">Block 터치 이벤트</param>
    public void InitMarker(int blockIndex, OnBlockClicked onBlockClicked)
    {
    	// 전달받은 blockIndex를 _blockIndex 변수에 저장
        _blockIndex = blockIndex;
        
        //마커를 바꾸는 메서드를 호출. 여기서는 MarkerType.None을 전달하여 마커를 지움.
        SetMarker(MarkerType.None);
        
        //에임도 다 지움
        SetAim(AimType.None);
        
        //최근 착수지점도 다 지움
        SetLast(PlaceType.None);
        
        //전달받은 onBlockClicked 델리게이트를 클래스의 onBlockClicked 변수에 저장
        this.onBlockClicked = onBlockClicked;
    }
    
    /// <summary>
    /// 어떤 마커를 표시할지 전달하는 함수
    /// </summary>
    /// <param name="markerType">마커 타입</param>
    public void SetMarker(MarkerType markerType)
    {
    	//위 마커타입의 값에 따라 실행결과가 변함.
        switch (markerType)
        {
        	//마커타입이 o면 스프라이트를 o로 변환.
            case MarkerType.black:
                stoneSpriteRenderer.sprite = black;
                break;
                
            //마커타입이 x면 스프라이트를 o로 변환.
            case MarkerType.white:
                stoneSpriteRenderer.sprite = white;
                break;
            
            case MarkerType.ban:
                stoneSpriteRenderer.sprite = ban;
                break;
                
            //마커타입이 none이면 스프라이트를 지움.
            case MarkerType.None:
                stoneSpriteRenderer.sprite = null;
                break;
        }
    }
    
    public void SetAim(AimType aimType)
    {
        //위 마커타입의 값에 따라 실행결과가 변함.
        switch (aimType)
        {
            case AimType.Aimed:
                aimSpriteRenderer.sprite = aimed;
                break;
            
            case AimType.None:
                aimSpriteRenderer.sprite = null;
                break;
        }
    }
    
    public void SetLast(PlaceType placedType)
    {
        //위 마커타입의 값에 따라 실행결과가 변함.
        switch (placedType)
        {
            case PlaceType.Last:
                lastPosSpriteRenderer.sprite = lastpos;
                break;
            
            case PlaceType.None:
                lastPosSpriteRenderer.sprite = null;
                break;
        }
    }

	//마우스 버튼이 블록 위에서 떼어질 때 호출. 누르는것,떼는것 전부 감지하기 때문에
    //오작동 방지가 용이
    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        Debug.Log($"현재 선택지점: {_blockIndex} \n" +
                  $"lastpos: {GameManager.Instance._lastPos[0]}, {GameManager.Instance._lastPos[1]}" +
                  $"lastplacepos: {GameManager.Instance._lastPlacedPos[0]}, {GameManager.Instance._lastPlacedPos[1]}");

        onBlockClicked?.Invoke(_blockIndex);
    }
}