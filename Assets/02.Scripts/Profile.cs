using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    //프로필 사진 담을 컴포넌트
    public Image profileImage;
    
    [SerializeField] Image[] profileImages;
    
    //마커 나타낼 컴포넌트
    public Image maker;

    
    private int index;
    
    
    //블록이 클릭되었을때 호출될 메서드를 델리게이트로 정의. 매개변수 int도 받도록시킴
    public delegate void OnProfileClicked(int index);
    
    //OnBlockClicked 타입의 이벤트를 정의.
    //이 이벤트는 블록이 클릭되었을 때 외부에서 등록한 메서드를 호출하는 데 사용
    public OnProfileClicked onProfileClicked;
    
    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        onProfileClicked?.Invoke(index);
    }
}
