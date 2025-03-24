using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSelcectPanelController : PanelController
{
    [SerializeField] private GameObject _profile;
    
    [SerializeField] private RectTransform _content;
    
    private Profile[] profiles = new Profile[12];

    public void Show()
    {
        base.Show();

        for (int i = 0; i < profiles.Length; i++)
        {
            var _proflie = Instantiate(_profile, _content);
            
            profiles.add(_proflie.GetComponent<Profile>());
        }
        


    }
    
    public void OnClickCloseButton()
    {
        Hide();
    }

    public void OnClickSelectButton()
    {
        //todo:선택 설정 버튼
        Hide();
    }
}
