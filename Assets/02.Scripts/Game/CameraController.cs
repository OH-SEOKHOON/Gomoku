using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera 컴포넌트가 반드시 필요함. 없으면 자동으로 추가
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	//카메라의 너비 단위를 정의하는 변수 (인스펙터에서 지정)
    [SerializeField] private float widthUnit = 6f;

	//Camera 타입의 private 변수를 선언
    private Camera _camera;
    
    private void Start()
    {
    	//이 게임 오브젝트에 붙은 Camera 컴포넌트를 가져와 _camera 변수에 할당
        _camera = GetComponent<Camera>();
        
        //카메라의 Orthographic(2D) 크기를 설정.
        //위에서 설정한 너비단위 / 카메라의 화면 비율(가로/세로) / 2라서
        //화면 비율(aspect)에 따라 자동으로 높이를 계산하여 카메라가 원하는 너비를 항상 유지하도록 함
        _camera.orthographicSize = widthUnit / _camera.aspect / 2;
    }
}
