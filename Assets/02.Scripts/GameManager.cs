using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//싱글톤 패턴
public class GameManager : Singleton<GameManager>
{
    //이제 ui프리팹 생성을 게임매니저가 하게 되므로 인스펙터에서 지정하기위해
    //참조변수를 설정
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject profileselectPanel;
    
    //프로필 영역
    [SerializeField] private Image _profileImage;
    [SerializeField] private Sprite[] _profileImages;
    
	//BlockController를 참조
    private BlockController _blockController;
    
    // 게임 UI 참조
    [NonSerialized] public GameUIController _gameUIController;
    
    // 캔버스 참조
    private Canvas _canvas;
    
    //Player를 저장할 열거형(이넘)변수
    public enum PlayerType { None, Black, White, Ban }
    
    //조준점을 저장할 열거형 변수
    public enum AimedType { None, Aimed };
    
    //최근 착수지점을 저장할 열거형 변수
    public enum PlacedType { None, Placed };
    
    //오목 보드 배열
    public PlayerType[,] _board;
    
    //조준 보드 배열
    private AimedType[,] _aimboard;
    
    //착수 보드 배열
    private PlacedType[,] _placeboard;
    
    //현재 플레이어를 저장할 변수
    [HideInInspector] public PlayerType _currentPlayer;
    
    //현재 위치를 저장할 변수
    [HideInInspector] public int[] _lastPos = {-1, -1};
    
    //마지막 착수지점을 저장할 변수
    [HideInInspector] public int[] _lastPlacedPos = {-1, -1};

	//게임의 상태(진행 중, 승리, 패배, 무승부)를 나타내는 열거형
    public enum GameResult
    {
        None,   // 게임 진행 중
        Win,    // 플레이어 승
        Lose,   // 플레이어 패
        Draw    // 비김
    }
    
    //금수지점을 저장할 큐
    private Queue<int[]> banposQueue = new Queue<int[]>();
    
    //탐색할 방향 (가로, 세로, 대각선)
    [HideInInspector] public int[] dx = { 1, -1, 0, 0, 1, -1, 1, -1 };
    [HideInInspector] public int[] dy = { 0, 0, 1, -1, 1, -1, -1, 1 };
    
    
    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        // 가상 보드들 초기화
        _board = new PlayerType[15, 15];
        
        _aimboard = new AimedType[15, 15];
        
        _placeboard = new PlacedType[15, 15];
        
        // 금수 리스트 초기화
        banposQueue.Clear();
        
        // BlockController의 InitBlocks() 호출 → 블록들을 초기화하고 클릭 이벤트 설정
        _blockController.InitBlocks();
        
        //흑돌의 차례로 설정
        _currentPlayer = PlayerType.Black;
        
        // Game UI도 흑돌 차례로 설정
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnB);
        
        SetPlaced();
    }
    
    private void SetPlaced()
    {
        _blockController.OnBlockClickedDelegate = (row, col) =>
        {
            // 기존 AIM을 지우기
            if (_lastPos[0] != -1 && _lastPos[1] != -1)
            {
                _blockController.PlaceAim(Block.AimType.None, _lastPos[0], _lastPos[1]);
                _aimboard[_lastPos[0], _lastPos[1]] = AimedType.None;
            }
        
            // 새로운 AIM 설정
            _blockController.PlaceAim(Block.AimType.Aimed, row, col);
            _aimboard[row, col] = AimedType.Aimed;

            // 마지막 위치 업데이트
            _lastPos[0] = row;
            _lastPos[1] = col;
        };
    }
    
    
    /// <summary>
    /// _board에 새로운 값을 할당하는 함수
    /// </summary>
    /// <param name="playerType">할당하고자 하는 플레이어 타입</param>
    /// <param name="row">Row</param>
    /// <param name="col">Col</param>
    /// <returns>False가 반환되면 할당할 수 없음, True는 할당이 완료됨</returns>
    public bool SetNewBoardValue(PlayerType playerType, int row, int col)
    {
        if (_board[row, col] != PlayerType.None)
            return false;
        
    	// 현재 playerType이 흑돌 플레이어라면
        if (playerType == PlayerType.Black)
        {
        	// 보드에 흑돌 배치
            _board[row, col] = playerType;
            
            // 흑돌 마커 표시
            _blockController.PlaceMarker(Block.MarkerType.black, row, col);
            
            //에임 지우기
            _blockController.PlaceAim(Block.AimType.None, row, col);
            _aimboard[row, col] = AimedType.None;
            
            // 기존 최근 착수지점을 지우기
            if (_lastPlacedPos[0] != -1 && _lastPlacedPos[1] != -1)
            {
                _blockController.PlaceLast(Block.PlaceType.None, _lastPlacedPos[0], _lastPlacedPos[1]);
                _placeboard[_lastPlacedPos[0], _lastPlacedPos[1]] = PlacedType.None;
            }
            
            // 새로운 최근 착수지점 업데이트
            _blockController.PlaceLast(Block.PlaceType.Last, row, col);
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
            _blockController.PlaceMarker(Block.MarkerType.white, row, col);
            
            _blockController.PlaceAim(Block.AimType.None, row, col);
            _aimboard[row, col] = AimedType.None;
            
            // 기존 최근 착수지점을 지우기
            if (_lastPlacedPos[0] != -1 && _lastPlacedPos[1] != -1)
            {
                _blockController.PlaceLast(Block.PlaceType.None, _lastPlacedPos[0], _lastPlacedPos[1]);
                _placeboard[_lastPlacedPos[0], _lastPlacedPos[1]] = PlacedType.None;
            }
            
            // 새로운 최근 착수지점 업데이트
            _blockController.PlaceLast(Block.PlaceType.Last, row, col);
            
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
    public GameResult CheckGameResult(int row, int col, PlayerType playerType)
    {
        if (playerType == PlayerType.Black)
        {
            //흑돌이 이겼다면 Win 반환
            if (ISFive(row, col, PlayerType.Black))
            {
                var wonlist = WinPos(row, col, playerType);
                _blockController.PlaceLine(Block.MarkerType.blackwin, wonlist);
                return GameResult.Win;
            }
        }

        if (playerType == PlayerType.White)
        {
            //백돌이 이겼다면 Lose 반환
            if (ISFive(row, col, PlayerType.White) || ISSix(row, col, PlayerType.White))
            {
                var wonlist = WinPos(row, col, playerType);
                _blockController.PlaceLine(Block.MarkerType.whitewin, wonlist);
                return GameResult.Lose;
            }
        }
        
        //다 아니라면 계속 게임진행중이라고 판단
        return GameResult.None;
    }

    /// <summary>
    /// 모든 마커가 보드에 배치 되었는지 확인하는 함수
    /// </summary>
    /// <returns>True: 모두 배치</returns>
    public void IsAllBlocksPlaced()
    {
        //이중 for문으로 보드배열을 순회
        for (var row = 0; row < _board.GetLength(0); row++)
        {
            for (var col = 0; col < _board.GetLength(1); col++)
            {
                //하나라도 none이라면
                if (_board[row, col] == PlayerType.None || _board[row, col] == PlayerType.Ban)
                    return;
            }
        }
        
        EndGame(GameResult.Draw);
    }

    public PlayerType GetCurrentPlayer()
    {
        return _currentPlayer;
    }
    
    
    //보드범위체크 메서드
    public bool CheckBoardRange(int row, int col)
    {
        return (row < 0 || row >= 15 || col < 0 || col >= 15);
    }
    
    
    //돌을 세는 메서드
    public int GetCountStone(int row, int col, PlayerType playerType, int direction)
    {
        int count = 1;

        for (int i = 0; i < 2; i++)
        {
            int dirx = dx[direction * 2 + i];
            int diry = dy[direction * 2 + i];
            
            int newRow = row;
            int newCol = col;
            
            while (true)
            {
                newRow += diry;
                newCol += dirx;
                

                if (CheckBoardRange(newRow, newCol) || _board[newRow, newCol] != playerType)
                    break;
                
                count++;
            }
        }
        return count;
    }

    //금수 확인 메서드
    public void Renju(int row, int col, PlayerType playerType)
    {
        for (int i = 0; i < 8; i++)
        {
            int dirx = dx[i];
            int diry = dy[i];
            
            for (int step = 1; step < 5; step++)
            {
                int newRow = row + diry * step;
                int newCol = col + dirx * step;
                
                if (CheckBoardRange(newRow, newCol))
                    break;
                
                int[] newPos = { newRow, newCol };

                if (_board[newRow, newCol] == PlayerType.None)
                {
                    if (ISSix(newRow, newCol, playerType))
                    {
                        Debug.Log("장목금수 추가");
                        banposQueue.Enqueue(newPos);
                    }
                    else if (ISDoubleThree(newRow, newCol, playerType))
                    {
                        Debug.Log("3-3 금수 추가");
                        banposQueue.Enqueue(newPos);
                    }
                    else if (ISDoubleFour(newRow, newCol, playerType))
                    {
                        Debug.Log("4-4 금수 추가");
                        banposQueue.Enqueue(newPos);
                    }
                }
            }
        }
    }
    
    //금수 시뮬레이션 메서드
    private bool TestRenju(int row, int col, PlayerType playerType)
    {
        if (ISFive(row, col, playerType))
            return false;
        
        if (ISSix(row, col, playerType))
            return true;
        
        if (ISDoubleThree(row, col, playerType))
            return true;
        
        if (ISDoubleFour(row, col, playerType))
            return true;
        
        return false;
    }
    
    //게임의 승패를 판단하는 메서드 (돌 5섯개를 검사)
    public bool ISFive(int row, int col, PlayerType playerType)
    {
        for (int i = 0; i < 4; i++)
        {
            int count = GetCountStone(row, col, playerType, i);
            if (count == 5) { return true; }
        }
        return false;
    }
    
    //이긴 돌의 좌표들을 받아오는 메서드
    private List<int[]> WinPos(int row, int col, PlayerType playerType)
    {
        List<int[]> _wonPos = new List<int[]>();
        
        for (int i = 0; i < 4; i++)
        {
            _wonPos.Clear();
            _wonPos.Add(new[] { row, col });
            
            for (int j = 0; j < 2; j++)
            {
                int dirx = dx[i * 2 + j];
                int diry = dy[i * 2 + j];
            
                int newRow = row;
                int newCol = col;
            
                while (true)
                {
                    newRow += diry;
                    newCol += dirx;
                    
                    if (CheckBoardRange(newRow, newCol) || _board[newRow, newCol] != playerType)
                        break;
                    
                    _wonPos.Add(new[] { newRow, newCol });

                    if (_wonPos.Count == 5)
                        return _wonPos;
                }
            }
        }
        return null;
    }
    
    //장목 판단 메서드
    public bool ISSix(int row, int col, PlayerType playerType)
    {
        for (int i = 0; i < 4; i++)
        {
            int count = GetCountStone(row, col, playerType, i);
            if (count >= 6) { return true; }
        }
        return false;
    }

    public int[] FindEmptyPos(int row, int col, PlayerType playerType, int direction)
    {
        int dirx = dx[direction];
        int diry = dy[direction];

        int newRow = row;
        int newCol = col;
            
        while (true)
        {
            newRow += diry;
            newCol += dirx;
            
            if (CheckBoardRange(newRow, newCol) || _board[newRow, newCol] != playerType)
                break;
        }

        // 범위 내에서 비어 있지 않은 곳을 찾으면 해당 위치 반환
        if (!CheckBoardRange(newRow, newCol) && _board[newRow, newCol] == PlayerType.None)
            return new[] { newRow, newCol };
        
        return null;
    }

    public bool ISOpenThree(int row, int col, PlayerType playerType, int direction)
    {
        for (int i = 0; i < 2; i++)
        {
            int[] Emptypos = FindEmptyPos(row, col, playerType, direction * 2 + i);

            if (Emptypos != null)
            {
                _board[Emptypos[0], Emptypos[1]] = PlayerType.Black;

                if (CountOpenFour(Emptypos[0], Emptypos[1], playerType, direction) == 1)
                {
                    if (!TestRenju(Emptypos[0], Emptypos[1], playerType))
                    {
                        _board[Emptypos[0], Emptypos[1]] = PlayerType.None;
                        return true;
                    }
                }
                
                _board[Emptypos[0], Emptypos[1]] = PlayerType.None;
            }
        }
        return false;
    }
    
    public int CountOpenFour(int row, int col, PlayerType playerType, int direction)
    {
        if (ISFive(row, col, playerType))
            return -1;
        
        int count = 0;
        
        for (int i = 0; i < 2; i++)
        {
            int[] Emptypos = FindEmptyPos(row, col, playerType, direction * 2 + i);

            if (Emptypos != null)
            {
                if (ISFive(Emptypos[0], Emptypos[1], playerType))
                {
                    count++;
                }
            }
        }

        if (count == 2)
        {
            if (GetCountStone(row, col, playerType, direction) == 4)
                count = 1;
        }
        else
            count = 0;
        
        return count;
    }

    public bool ISDoubleThree(int row, int col, PlayerType playerType)
    {
        int count = 0;
        
        _board[row, col] = playerType;

        for (int i = 0; i < 4; i++)
        {
            if (ISOpenThree(row, col, playerType, i))
                count++;
        }
        
        _board[row, col] = PlayerType.None;
        
        if (count >= 2)
            return true;
        
        return false;
    }

    public bool ISDoubleFour(int row, int col, PlayerType playerType)
    {
        int count = 0;
        
        _board[row, col] = playerType;
        
        for (int i = 0; i < 4; i++)
        {
            if (CountOpenFour(row, col, playerType, i) == 2)
                count += 2;
            else if (ISFour(row, col, playerType, i))
                count ++;
        }
        
        _board[row, col] = PlayerType.None;
        
        if (count >= 2)
            return true;
        
        return false;
    }

    public bool ISFour(int row, int col, PlayerType playerType, int direction)
    {
        for (int i = 0; i < 2; i++)
        {
            int[] Emptypos = FindEmptyPos(row, col, playerType, direction * 2 + i);

            if (Emptypos != null)
            {
                if (ISFiveInDirection(Emptypos[0], Emptypos[1], playerType, direction))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool ISFiveInDirection(int row, int col, PlayerType playerType, int direction)
    {
        if (GetCountStone(row, col, playerType, direction) == 5)
            return true;
        return false;
    }
    
    //실제로 금수자리를 표시하는 메서드
    public void PlacedBan()
    {
        //임시 큐 생성
        Queue<int[]> tempqueue = new Queue<int[]>();
        
        while (banposQueue.Count > 0)
        {
            int[] current = banposQueue.Dequeue();  // 금수 큐 순회하며 요소 꺼내기

            if (_board[current[0], current[1]] == PlayerType.None)
            {
                //여전히 금수라면 금수 표시 후 임시큐에 삽입
                if (TestRenju(current[0], current[1], PlayerType.Black))
                {
                    // 보드에 밴 마커 배치
                    _board[current[0], current[1]] = PlayerType.Ban;
                    
                    // 밴 마커 표시
                    _blockController.PlaceMarker(Block.MarkerType.ban, current[0], current[1]);
                
                    tempqueue.Enqueue(new int[] {current[0], current[1]});
                }
            }
        }
        // 기존 큐를 새로운 큐로 교체
        banposQueue = tempqueue;
    }

    //금수 제거 메서드
    public void ReplacedBan()
    {
        //디큐하지않고 금수리스트를 순회
        foreach (var pos in banposQueue)
        {
            //보드에 놓았던 금수 해제
            _board[pos[0], pos[1]] = PlayerType.None;
            
            // 마커도 해제
            _blockController.PlaceMarker(Block.MarkerType.None, pos[0], pos[1]);
        }
    }
    
    /// <summary>
    /// 게임 오버시 호출되는 함수
    /// gameResult에 따라 결과 출력
    /// </summary>
    /// <param name="gameResult">win, lose, draw</param>
    public void EndGame(GameResult gameResult)
    {
        // TODO: 나중에 구현!!
        
        switch (gameResult)
        {
            case GameResult.Win:
                Debug.Log("흑돌 우승");
                OpenConfirmPanel("흑돌 우승!", () => ChangeToMainScene(), false);
                break;
            case GameResult.Lose:
                Debug.Log("백돌 우승");
                OpenConfirmPanel("백돌 우승!", () => ChangeToMainScene(), false);
                break;
            case GameResult.Draw:
                Debug.Log("무승부");
                OpenConfirmPanel("무승부", () => ChangeToMainScene(), false);
                break;
        }
    }


    public void BoardDebug()
    {
        string boardString = "\n";
        
        for (int r = 0; r < 15; r++)
        {
            for (int c = 0; c < 15; c++)
            {
                boardString += (_board[r, c] == PlayerType.Black ? "흑" : _board[r, c] == PlayerType.White ? "백" : _board[r, c] == PlayerType.None ? "ㅇ" : "금") + " ";
            }
            boardString += "\n";
        }
        
        Debug.Log($"최근 착수지점: {_lastPos[0]}, {_lastPos[1]}");
        Debug.Log("Board State:\n" + boardString);
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //로드된 씬의 정보가 "Game"이라면
        if (scene.name == "Game")
        {
            //블록컨트롤러와 게임ui컨트롤러 오브젝트를 찾아서 참조변수에 할당
            _blockController = GameObject.FindObjectOfType<BlockController>();
            
            _gameUIController = GameObject.FindObjectOfType<GameUIController>();

            // 게임 시작 메서드 호출
            StartGame();
        }

        if (scene.name == "Main")
        {
            ChangeToProfile();
        }
        
        //캔버스 오브젝트를 찾아서 참조변수에 할당
        _canvas = GameObject.FindObjectOfType<Canvas>();
    }
    
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    //메인 씬으로 이동하게 만드는 메서드
    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("Main");
    }
	
    //세팅패널을 여는 메서드
    public void OpenSettingsPanel()
    {
        //캔버스가 할당되어 있다면
        if (_canvas != null)
        {
            //_canvas(UI의 부모) 아래에 settingsPanel을 생성하여 변수에 할당
            var settingsPanelObject = Instantiate(settingsPanel, _canvas.transform);
            
            //변수의 PanelController컴포넌트를 가져와 Show()를 호출해 패널을 활성화
            settingsPanelObject.GetComponent<PanelController>().Show();
        }
        else 
            Debug.Log("캔버스가 null입니다");
    }

    //컨펌패널을 여는 메서드
    public void OpenConfirmPanel(string message, ConfirmPanelController.OnConfirmButtonClick onConfirmButtonClick, bool isshowclose = true)
    {
        //캔버스가 할당되어 있다면
        if (_canvas != null)
        {
            //_canvas(UI의 부모) 아래에 confirmPanel을 생성하여 변수에 할당
            var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
            
            //변수의 ConfirmPanelController컴포넌트를 가져와 .Show(message, onConfirmButtonClick)를
            //호출해 패널을 활성화
            confirmPanelObject.GetComponent<ConfirmPanelController>()
                .Show(message, onConfirmButtonClick, isshowclose);
        }
        else
        {
            Debug.Log("캔버스가 null입니다");
        }
    }

    public void OpenProfileSelectPanel()
    {
        //캔버스가 할당되어 있다면
        if (_canvas != null)
        {
            //_canvas(UI의 부모) 아래에 settingsPanel을 생성하여 변수에 할당
            var profileslectPanelObject = Instantiate(profileselectPanel, _canvas.transform);
            
            //변수의 ProfileSelcectPanelController컴포넌트를 가져와 Show()를 호출해 패널을 활성화
            profileslectPanelObject.GetComponent<ProfileSelcectPanelController>().Show();
        }
        else 
            Debug.Log("캔버스가 null입니다");
    }

    public void ChangeToProfile()
    {
        _profileImage.sprite = _profileImages[UserInformations.ProfileIndex];
    }
}