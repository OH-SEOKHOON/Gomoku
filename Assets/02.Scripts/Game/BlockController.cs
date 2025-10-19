using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    private Block[] blocks = new Block[15 * 15];
    
    //델리게이트를 선언. 특정 블록이 클릭되었을 때 실행할 함수를 저장
    public delegate void OnBlockClicked(int row, int col);
    
    //OnBlockClickedDelegate는 OnBlockClicked 타입의 델리게이트 변수
    public OnBlockClicked OnBlockClickedDelegate;
    

    public void InitBlocks()
    {
        //보드 만들기
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                var _block = Instantiate(blockPrefab, new Vector2(-3.15f + j * 0.45f, 3.15f - i * 0.45f), Quaternion.identity);
                blocks[i * 15 + j] = _block.GetComponent<Block>();
            }
        }
        
        //블록 배열 순회
        for (int i = 0; i < blocks.Length; i++)
        {
            //i번째 블록의 InitMarker() 메서드를 호출
            blocks[i].InitMarker(i, blockIndex =>
            {
                //람다식으로 블록 인덱스를 행(row)과 열(col) 좌표로 변환
                var clickedRow = blockIndex / 15;
                var clickedCol = blockIndex % 15;
                
                //nBlockClickedDelegate가 null이 아닐 경우, 등록된 함수를 실행
                OnBlockClickedDelegate?.Invoke(clickedRow, clickedCol);
            });
        }
    }
    
    /// <summary>
    /// 특정 Block에 마커 표시하는 함수
    /// </summary>
    /// <param name="markerType">마커 타입</param>
    /// <param name="row">Row</param>
    /// <param name="col">Col</param>
    public void PlaceMarker(Block.MarkerType markerType, int row, int col)
    {
        // row, col을 index로 변환
        var markerIndex = row * 15 + col;
        
        // Block에게 마커 표시
        blocks[markerIndex].SetMarker(markerType);
    }

    public void PlaceAim(Block.AimType aimType, int row, int col)
    {
        // row, col을 index로 변환
        var aimIndex = row * 15 + col;
        
        // Block에게 마커 표시
        blocks[aimIndex].SetAim(aimType);
    }

    public void PlaceLast(Block.PlaceType placedType, int row, int col)
    {
        var lastIndex = row * 15 + col;
        
        blocks[lastIndex].SetLast(placedType);
    }
    
    public void PlaceLine(Block.MarkerType markerType, List<int[]> wonlist)
    {
        
        if (wonlist == null || wonlist.Count == 0)
        {
            Debug.Log("빈 리스트입니다.");
            return;
        }

        for (int i = 0; i < wonlist.Count; i++)
        {
            var index = wonlist[i][0] * 15 + wonlist[i][1];
            blocks[index].SetMarker(markerType);
        }
    }
}
