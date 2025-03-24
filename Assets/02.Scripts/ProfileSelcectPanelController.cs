using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSelcectPanelController : PanelController
{
    [SerializeField] private GameObject _profile;
    
    [SerializeField] private RectTransform _content;
    
    private Profile[] profiles = new Profile[12];
    private Profile selectedProfile = null;
    private int selectedIndex = -1;

    public void Show()
    {
        base.Show();

        for (int i = 0; i < profiles.Length; i++)
        {
            var _proflie = Instantiate(_profile, _content);
            
            profiles[i] = _proflie.GetComponent<Profile>();
            
            profiles[i].InitMarker(i);
            
            profiles[i].profileImage.sprite = profiles[i].profileImages[i];
            
            // 버튼 이벤트 추가
            Button button = profiles[i].GetComponent<Button>();
            int index = i; // 람다 캡처 문제 방지
            button.onClick.AddListener(() => OnProfileSelected(index));
        }
        
    }
        
    private void OnProfileSelected(int index)
    {
        if (selectedIndex == index)
        {
            selectedProfile.Deselect();
            selectedIndex = -1;
            return;
        }
        
        // 기존 선택 해제
        if (selectedProfile != null)
        {
            selectedProfile.Deselect();
            selectedIndex = -1;
        }

        // 새 프로필 선택
        selectedProfile = profiles[index];
        selectedIndex = index;
        selectedProfile.Select();
    }
    
    public void OnClickCloseButton()
    {
        Hide();
    }

    public void OnClickSelectButton()
    {
        if (selectedIndex == -1)
        {
            GameManager.Instance.OpenConfirmPanel("프로필을 선택해주세요", null, false);
            return;
        }
        
        UserInformations.ProfileIndex = selectedIndex;
        GameManager.Instance.ChangeToProfile();
        Hide();
    }
}
