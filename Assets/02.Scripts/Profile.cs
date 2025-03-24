using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    //프로필 사진 담을 컴포넌트
    public Image profileImage;
    
    public Sprite[] profileImages;
    
    //마커 나타낼 컴포넌트
    public Image maker;

    [SerializeField] private Sprite _maker;
    
    bool isselected;
    
    public void InitMarker(int Index)
    {
        Deselect();
    }
    
    public void Select()
    {
        maker.sprite = _maker;
        maker.color = new Color(0, 1, 0,1f);
    }

    public void Deselect()
    {
        maker.sprite = null;
        maker.color = new Color(0, 0, 0,0f);
    }
    
}
