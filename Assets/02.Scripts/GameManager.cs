using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//싱글톤 패턴
public class GameManager : Singleton<GameManager>
{
	//BlockController를 참조
    [SerializeField] private BlockController blockController;

	//게임 시작 시 표시될 시작 패널
    [SerializeField] private GameObject startPanel;     // 임시 변수, 나중에 삭제 예정
    
    //Player를 저장할 열거형(이넘)변수
    private enum PlayerType { None, Black, White, Ban }
    
    //조준점을 저장할 열거형 변수
    private enum AimedType { None, Aimed };
    
    //최근 착수지점을 저장할 열거형 변수
    private enum PlacedType { None, Placed };
    
    //오목 보드 배열
    private PlayerType[,] _board;
    
    //조준 보드 배열
    private AimedType[,] _aimboard;
    
    //착수 보드 배열
    private PlacedType[,] _placeboard;
    
    //현재 플레이어를 저장할 변수
    private PlayerType _currentPlayer;
    
    //현재 위치를 저장할 변수
    public int[] _lastPos = {-1, -1};
    
    //마지막 착수지점을 저장할 변수
    public int[] _lastPlacedPos = {-1, -1};

	//게임의 상태(진행 중, 승리, 패배, 무승부)를 나타내는 열거형
    private enum GameResult
    {
        None,   // 게임 진행 중
        Win,    // 플레이어 승
        Lose,   // 플레이어 패
        Draw    // 비김
    }
    
    //금수지점을 저장할 큐
    private Queue<int[]> banposQueue = new Queue<int[]>();

    private void Start()
    {
        // 게임 초기화
        InitGame();
    }

    /// <summary>
    /// 게임 초기화 함수
    /// </summary>
    public void InitGame()
    {
        // 가상 보드들 초기화
        _board = new PlayerType[15, 15];
        
        _aimboard = new AimedType[15, 15];
        
        _placeboard = new PlacedType[15, 15];
        
        // 금수 리스트 초기화
        banposQueue.Clear();
        
        // BlockController의 InitBlocks() 호출 → 블록들을 초기화하고 클릭 이벤트 설정
        blockController.InitBlocks();
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
    	//startPanel을 비활성화하여 게임을 시작
        startPanel.SetActive(false);        // TODO: 테스트 코드, 나중에 삭제 예정
        
        //SetTurn(TurnType.흑돌) 호출 → 흑돌의 차례로 설정
        _currentPlayer = PlayerType.Black;
        SetPlaced();
    }
    
    private void SetPlaced()
    {
        blockController.OnBlockClickedDelegate = (row, col) =>
        {
            // 기존 AIM을 지우기
            if (_lastPos[0] != -1 && _lastPos[1] != -1)
            {
                blockController.PlaceAim(Block.AimType.None, _lastPos[0], _lastPos[1]);
                _aimboard[_lastPos[0], _lastPos[1]] = AimedType.None;
            }
        
            // 새로운 AIM 설정
            blockController.PlaceAim(Block.AimType.Aimed, row, col);
            _aimboard[row, col] = AimedType.Aimed;

            // 마지막 위치 업데이트
            _lastPos[0] = row;
            _lastPos[1] = col;
        };
    }
    
    //착수 메서드
    public void OnPlacedStone()
    {
        switch (_currentPlayer)
        {
            case PlayerType.Black:
            {
                if (SetNewBoardValue(PlayerType.Black, _lastPos[0], _lastPos[1]))
                {
                    
                    //CheckGameResult()를 호출하여 승리, 패배, 무승부 확인
                    var gameResult = CheckGameResult();
                        
                    //결과가 NONE(결과안나옴)이라면
                    if (gameResult == GameResult.None)
                    {
                        //금수 테스트
                        Renju();
                        
                        //금수 배치 해제
                        ReplacedBan();
                            
                        //백돌턴으로 넘김
                        _currentPlayer = PlayerType.White;
                    }
                    else
                        //결과났으면 게임 종료 메서드 출력
                        EndGame(gameResult);
                }
                else
                {
                    Debug.Log("이미 둔곳입니다");
                }
                break;
            }

            case PlayerType.White:
            {
                if (SetNewBoardValue(PlayerType.White, _lastPos[0], _lastPos[1]))
                {
                    var gameResult = CheckGameResult();
                    if (gameResult == GameResult.None)
                    {
                        //금수 배치
                        PlacedBan();
                        _currentPlayer = PlayerType.Black;
                    }
                    else
                        EndGame(gameResult);
                }
                else
                {
                    Debug.Log("이미 둔곳입니다");
                }
                break;
            }
        }
    }
    
    /// <summary>
    /// _board에 새로운 값을 할당하는 함수
    /// </summary>
    /// <param name="playerType">할당하고자 하는 플레이어 타입</param>
    /// <param name="row">Row</param>
    /// <param name="col">Col</param>
    /// <returns>False가 반환되면 할당할 수 없음, True는 할당이 완료됨</returns>
    private bool SetNewBoardValue(PlayerType playerType, int row, int col)
    {
        if (_board[row, col] != PlayerType.None)
            return false;
        
    	// 현재 playerType이 흑돌 플레이어라면
        if (playerType == PlayerType.Black)
        {
        	// 보드에 흑돌 배치
            _board[row, col] = playerType;
            
            // 흑돌 마커 표시
            blockController.PlaceMarker(Block.MarkerType.black, row, col);
            
            //에임 지우기
            blockController.PlaceAim(Block.AimType.None, row, col);
            _aimboard[row, col] = AimedType.None;
            
            // 기존 최근 착수지점을 지우기
            if (_lastPlacedPos[0] != -1 && _lastPlacedPos[1] != -1)
            {
                blockController.PlaceLast(Block.PlaceType.None, _lastPlacedPos[0], _lastPlacedPos[1]);
                _placeboard[_lastPlacedPos[0], _lastPlacedPos[1]] = PlacedType.None;
            }
            
            // 새로운 최근 착수지점 업데이트
            blockController.PlaceLast(Block.PlaceType.Last, row, col);
            _placeboard[row, col] = PlacedType.Placed;
            
            //마지막 착수지점 갱신
            _lastPlacedPos[0] = row;
            _lastPlacedPos[1] = col;
            
            return true;
        }
        // 현재 playerType이 백돌플레이어라면
        else if (playerType == PlayerType.White)
        {
        	// 보드에 B 플레이어 마커 배치
            _board[row, col] = playerType;
            
            // X 마커 표시
            blockController.PlaceMarker(Block.MarkerType.white, row, col);
            
            blockController.PlaceAim(Block.AimType.None, row, col);
            _aimboard[row, col] = AimedType.None;
            
            // 기존 최근 착수지점을 지우기
            if (_lastPlacedPos[0] != -1 && _lastPlacedPos[1] != -1)
            {
                blockController.PlaceLast(Block.PlaceType.None, _lastPlacedPos[0], _lastPlacedPos[1]);
                _placeboard[_lastPlacedPos[0], _lastPlacedPos[1]] = PlacedType.None;
            }
            
            // 새로운 최근 착수지점 업데이트
            blockController.PlaceLast(Block.PlaceType.Last, row, col);
            
            //마지막 착수지점 갱신
            _lastPlacedPos[0] = row;
            _lastPlacedPos[1] = col;
            
            return true;
        }
        // 실패 시 false 반환
        return false;
    }

    
    /// <summary>
    /// 게임 결과 확인 함수
    /// </summary>
    /// <returns>플레이어 기준 게임 결과</returns>
    private GameResult CheckGameResult()
    {
    	//흑돌이 이겼다면 Win 반환
        if (CheckGameWin(PlayerType.Black)) { return GameResult.Win; }
        
        //백돌이 이겼다면 Lose 반환
        if (CheckGameWin(PlayerType.White)) { return GameResult.Lose; }
        
        //모든 칸이 채워졌다면 Draw 반환
        if (IsAllBlocksPlaced()) { return GameResult.Draw; }
        
        //다 아니라면 계속 게임진행중이라고 판단
        return GameResult.None;
    }
    
    //게임의 승패를 판단하는 함수
    private bool CheckGameWin(PlayerType playerType)
    {
        // 검사할 방향 (가로, 세로, 대각선)
        int[] dx = { 1, 0, 1, 1 };
        int[] dy = { 0, 1, 1, -1 };

        // 마지막 착수 위치
        int startRow = _lastPlacedPos[0];
        int startCol = _lastPlacedPos[1];

        // 방향 탐색
        for (int i = 0; i < dx.Length; i++)
        {
            int count = 1;  // 현재 착수한 돌 포함

            // 한 방향으로 4칸 이동하면서 검사
            for (int step = 1; step < 5; step++)
            {
                int newRow = startRow + dx[i] * step;
                int newCol = startCol + dy[i] * step;

                // 보드 범위 초과 시 중단
                if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
                    break;

                // 같은 플레이어 돌이면 카운트 증가
                if (_board[newRow, newCol] == playerType)
                    count++;
                else
                    break;
            }
            
            // 반대방향으로도 4칸 이동하면서 검사
            for (int step = 1; step < 5; step++)
            {
                int newRow = startRow - dx[i] * step;
                int newCol = startCol - dy[i] * step;

                // 보드 범위 초과 시 중단
                if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
                    break;

                // 같은 플레이어 돌이면 카운트 증가
                if (_board[newRow, newCol] == playerType)
                    count++;
                else
                    break;
            }
            
            // 5개 연속이면 승리
            if (count >= 5)
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// 모든 마커가 보드에 배치 되었는지 확인하는 함수
    /// </summary>
    /// <returns>True: 모두 배치</returns>
    private bool IsAllBlocksPlaced()
    {
        //이중 for문으로 보드배열을 순회
        for (var row = 0; row < _board.GetLength(0); row++)
        {
            for (var col = 0; col < _board.GetLength(1); col++)
            {
                //하나라도 none이라면
                if (_board[row, col] == PlayerType.None || _board[row, col] == PlayerType.Ban)
                    return false; //false를 반환
            }
        }
        return true;
    }

    private void Renju()
    {
        Debug.Log("금수테스트 시작");
        
        // 검사할 방향 (가로, 세로, 대각선)
        int[] dx = { 1, 0, 1, 1};
        int[] dy = { 0, 1, 1, -1};

        // 마지막 착수 위치
        int startRow = _lastPlacedPos[0];
        int startCol = _lastPlacedPos[1];

        // 8방향 탐색
        for (int i = 0; i < dx.Length; i++)
        {
            for (int step = 1; step < 5; step++) // 최대 4칸까지 탐색
            {
                int newRow = startRow + dx[i] * step;
                int newCol = startCol + dy[i] * step;

                // 보드 범위 초과 시 중단
                if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
                    break;

                // 빈 칸이면 검사
                if (_board[newRow, newCol] == PlayerType.None)
                {
                    if (ISSix(newRow, newCol))
                    {
                        banposQueue.Enqueue(new int[] {newRow, newCol});
                        Debug.Log($"{newRow}, {newCol} 장목금수추가");
                    }
                    else if (ISFourFour(newRow, newCol))
                    {
                        banposQueue.Enqueue(new int[] {newRow, newCol});
                        Debug.Log($"{newRow}, {newCol} 4-4금수추가");
                    }
                    else if (IsThreeThree(newRow, newCol))
                    {
                        banposQueue.Enqueue(new int[] {newRow, newCol});
                        Debug.Log($"{newRow}, {newCol} 3-3금수추가");
                    }
                }
            }
            
            for (int step = 1; step < 5; step++) // 반대 4칸까지 탐색
            {
                int newRow = startRow - dx[i] * step;
                int newCol = startCol - dy[i] * step;

                // 보드 범위 초과 시 중단
                if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
                    break;

                // 빈 칸이면 검사
                if (_board[newRow, newCol] == PlayerType.None)
                {
                    if (ISSix(newRow, newCol))
                    {
                        banposQueue.Enqueue(new int[] {newRow, newCol});
                        Debug.Log($"{newRow}, {newCol} 장목금수추가");
                    }
                    else if (ISFourFour(newRow, newCol))
                    {
                        banposQueue.Enqueue(new int[] {newRow, newCol});
                        Debug.Log($"{newRow}, {newCol} 4-4금수추가");
                    }
                    else if (IsThreeThree(newRow, newCol))
                    {
                        banposQueue.Enqueue(new int[] {newRow, newCol});
                        Debug.Log($"{newRow}, {newCol} 3-3금수추가");
                    }
                }
            }
        }
    }

    private bool ISSix(int row, int col)
    {
        // 검사할 방향 (가로, 세로, 대각선)
        int[] dx = { 1, 0, 1, 1 };
        int[] dy = { 0, 1, 1, -1 };
        
        // 방향 탐색
        for (int i = 0; i < dx.Length; i++)
        {
            int count = 1;  // 현재 착수한 돌 포함

            // 한 방향으로 5칸 이동하면서 검사
            for (int step = 1; step < 6; step++)
            {
                int newRow = row + dx[i] * step;
                int newCol = col + dy[i] * step;

                // 보드 범위 초과 시 중단
                if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
                    break;

                // 흑돌이면 카운트 증가
                if (_board[newRow, newCol] == PlayerType.Black)
                {
                    count++;
                }
                else
                    break;
            }
            
            // 반대방향으로도 5칸 이동하면서 검사
            for (int step = 1; step < 6; step++)
            {
                int newRow = row - dx[i] * step;
                int newCol = col - dy[i] * step;

                // 보드 범위 초과 시 중단
                if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
                    break;

                // 흑돌이면 카운트 증가
                if (_board[newRow, newCol] == PlayerType.Black)
                {
                    count++;
                }
                else
                    break;
            }
            // 6개 연속이면 금수
            if (count >= 6)
                return true;
        }
        
        return false;
    }

    //4-4금수 메서드 (미구현)
    private bool ISFourFour(int row, int col)
    {
        // // 검사할 방향 (가로, 세로, 대각선, 중복 금수때문에 반대방향 포함)
        // int[] dx = { 1, 0, 1, 1, -1, 0, -1, -1 };
        // int[] dy = { 0, 1, 1, -1, 0, -1, -1, 1 };
        //
        // int fourcount = 0; // 4칸 줄 개수
        //
        // // 4가지 방향 탐색
        // for (int i = 0; i < dx.Length; i++)
        // {
        //     
        //     int count = 1;  // 현재 착수한 돌 포함
        //     int nonecount = 0; // 띈 칸 개수
        //
        //     // 정방향 검사 (최대 4칸)
        //     for (int step = 1; step < 5; step++)
        //     {
        //         int newRow = row + dx[i] * step;
        //         int newCol = col + dy[i] * step;
        //         
        //         if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
        //             break;
        //
        //         if (_board[newRow, newCol] == PlayerType.Black)
        //         {
        //             count++;
        //         }
        //         else if (_board[newRow, newCol] != PlayerType.White)
        //         {
        //             nonecount++;
        //             if (nonecount >= 2)
        //                 break;
        //         }
        //         
        //         else
        //             break;
        //     }
        //     
        //     // 4개 연속 (공백 1개 포함 가능) → 4줄 카운트 증가
        //     if (count >= 4)
        //         fourcount++;
        //
        //     else if (count != 1)
        //     {
        //         // 역방향 검사 (최대 4칸)를 하되 바로 반대로 돌아온 경우는 패스한다.
        //         for (int step = 1; step < 5; step++)
        //         {
        //             int newRow = row - dx[i] * step;
        //             int newCol = col - dy[i] * step;
        //
        //             if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
        //                 break;
        //
        //             if (_board[newRow, newCol] == PlayerType.Black)
        //                 count++;
        //         
        //             else if (_board[newRow, newCol] != PlayerType.White)
        //             {
        //                 nonecount++;
        //                 if (nonecount >= 3)
        //                     break;
        //             }
        //         
        //             else
        //                 break;
        //         }
        //         
        //         if (count >= 4)
        //             fourcount++;
        //     }
        //     
        //     
        //     // 4-4 판정되면 즉시 종료
        //     if (fourcount >= 2)
        //         return true;
        // }
        //
        // // 테스트 종료
        return false;
    }

    //3-3금수 메서드(미구현)
    private bool IsThreeThree(int row, int col)
    {
        // // 검사할 방향 (가로, 세로, 대각선)
        // int[] dx = { 1, 0, 1, 1 };
        // int[] dy = { 0, 1, 1, -1 };
        //
        // int threecount = 0;
        //
        // // 방향 탐색
        // for (int i = 0; i < dx.Length; i++)
        // {
        //     int count = 1;  // 현재 착수한 돌 포함
        //
        //     // 한 방향으로 2칸 이동하면서 검사
        //     for (int step = 1; step < 3; step++)
        //     {
        //         int newRow = row + dx[i] * step;
        //         int newCol = col + dy[i] * step;
        //
        //         // 보드 범위 초과 시 중단
        //         if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
        //             break;
        //
        //         // 열린곳 탐지후 역방향 탐색
        //         if (_board[newRow, newCol] == PlayerType.None)
        //         {
        //             for (int backstep = 1; backstep < 3; backstep++)
        //             {
        //                 int newnewRow = newRow - dx[i] * backstep;
        //                 int newnewCol = newCol - dy[i] * backstep;
        //
        //                 // 보드 범위 초과 시 중단
        //                 if (newRow < 0 || newRow >= 15 || newCol < 0 || newCol >= 15)
        //                     break;
        //
        //                 // 같은 플레이어 돌이면 카운트 증가
        //                 if (_board[newnewRow, newnewCol] == PlayerType.Black)
        //                 {
        //                     count++;
        //                     if (count >= 3)
        //                     {
        //                         if (_board[newnewRow - dx[i], newnewCol - dy[i]] == PlayerType.None)
        //                             threecount++;
        //                     }
        //                 }
        //                 else
        //                     break;
        //             }
        //         }
        //         else if (_board[newRow, newCol] == PlayerType.White)
        //             break;
        //     }
        //     
        //     if (threecount >= 2)
        //         return true;
        // }
        //
        return false;
    }
    
    //실제로 금수자리를 표시하는 메서드
    private void PlacedBan()
    {
        //임시 큐 생성
        Queue<int[]> tempqueue = new Queue<int[]>();
        
        while (banposQueue.Count > 0)
        {
            int[] current = banposQueue.Dequeue();  // 금수 큐 순회하며 요소 꺼내기

            if (_board[current[0], current[1]] == PlayerType.None)
            {
                //여전히 금수라면 금수 표시 후 임시큐에 삽입
                if (ISSix(current[0], current[1]))
                {
                    // 보드에 밴 마커 배치
                    _board[current[0], current[1]] = PlayerType.Ban;
            
                    // 밴 마커 표시
                    blockController.PlaceMarker(Block.MarkerType.ban, current[0], current[1]);
                
                    tempqueue.Enqueue(new int[] {current[0], current[1]});
                }
                else if (ISFourFour(current[0], current[1]))
                {
                    // 보드에 밴 마커 배치
                    _board[current[0], current[1]] = PlayerType.Ban;
            
                    // 밴 마커 표시
                    blockController.PlaceMarker(Block.MarkerType.ban, current[0], current[1]);
                
                    tempqueue.Enqueue(new int[] {current[0], current[1]});
                }
                else if (IsThreeThree(current[0], current[1]))
                {
                    // 보드에 밴 마커 배치
                    _board[current[0], current[1]] = PlayerType.Ban;
            
                    // 밴 마커 표시
                    blockController.PlaceMarker(Block.MarkerType.ban, current[0], current[1]);
                
                    tempqueue.Enqueue(new int[] {current[0], current[1]});
                }
            }
        }

        // 기존 큐를 새로운 큐로 교체
        banposQueue = tempqueue;
    }

    //금수 제거 메서드
    private void ReplacedBan()
    {
        //디큐하지않고 금수리스트를 순회
        foreach (var pos in banposQueue)
        {
            //보드에 놓았던 금수 해제
            _board[pos[0], pos[1]] = PlayerType.None;
            
            // 마커도 해제
            blockController.PlaceMarker(Block.MarkerType.None, pos[0], pos[1]);
        }
    }
    
    /// <summary>
    /// 게임 오버시 호출되는 함수
    /// gameResult에 따라 결과 출력
    /// </summary>
    /// <param name="gameResult">win, lose, draw</param>
    private void EndGame(GameResult gameResult)
    {
        // TODO: 나중에 구현!!
        
        switch (gameResult)
        {
            case GameResult.Win:
                Debug.Log("Player B win");
                break;
            case GameResult.Lose:
                Debug.Log("Player W win");
                break;
            case GameResult.Draw:
                Debug.Log("Draw");
                break;
        }
    }


    public void BoardDebug()
    {
        foreach (var pos in banposQueue)
        {
            Debug.Log($"{pos[0]}, {pos[1]}");
        }
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}