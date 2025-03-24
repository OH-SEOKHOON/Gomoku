using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.Mathematics;
using UnityEngine;

public static class MinimaxAIController
{
    //ai가 볼 시야
    static int view = 1;
        
    //ai가 생각할 깊이
    static int depth;
    
    //최적의 수를 찾는 함수
    public static (int row, int col)? GetBestMove(GameManager.PlayerType[,] board, int rank, int rawplace, int colplace )
    {
        
        //ai가 얻을 수 있는 최고 점수를 저장할 변수. 최저점부터 시작하게 함.
        float bestScore = float.MinValue;
        
        //랭크와 관련된 난이도 설정
        if (rank == 1)
        {
            depth = 7;
        }
        else if (rank <= 5)
        {
            depth = 6;
        }
        else if (rank <= 10)
        {
            depth = 5;
        }
        else if (rank <= 15)
        {
            depth = 4;
        }
        else
        {
            depth = 3;
        }
        
        //최적의 수를 저장할 변수
        (int row, int col)? bestMove = null;
        
        for (var row = rawplace - view; row <= rawplace + view; row++)
        {
            for (var col = colplace - view; col <= colplace + view; col++)
            {
                //보드 범위 확인
                if (!GameManager.Instance.CheckBoardRange(row, col))
                {
                    //만약 현재 순회중인 칸이 비어있다면
                    if (board[row, col] == GameManager.PlayerType.None)
                    {
                        //AI(PlayerB)의 돌을 임시로 놓음
                        board[row, col] = GameManager.PlayerType.White;
                    
                        //DoMinimax 함수를 통해 점수를 계산
                        var score = DoMinimax(board, 0, false, row, col, float.MinValue, float.MaxValue);
                        
                        Debug.Log(score);
                    
                        //임시로 놓은 돌은 다시 회수
                        board[row, col] = GameManager.PlayerType.None;
                    
                        //score > bestScore일 경우, 현재 위치를 최적의 수로 기록.
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (row, col);
                        }
                    }
                }
            }
        }
        
        return bestMove;
    }
    
    //미니맥스 알고리즘 함수. 보드, 깊이(현재 턴 수),
    //현재 턴이 AI(max)인지, 상대(mini)인지 나타내는 불값을 파라미터로 받음
    private static float DoMinimax(GameManager.PlayerType[,] board, int currentdepth, bool isMaximizing, int rowplace, int colplace, float alpha, float beta)
    {
        
        //일단 검사 메서드를 돌려서 게임이 끝났는지 검사. 흑돌이 이긴 경우
            if (CheckSimulGameWin(rowplace, colplace, GameManager.PlayerType.Black))
                //최대한 AI가 버틸 수 있도록 턴이 지날때마다 DEPTH가 잃는 점수를 적게 만들어준다.
                return -10000 + depth * 100;

            //흑돌은 금수를 두지 않음. 최소화턴에 가장 작은 값을 리턴하므로 최대값을 주게 설정하면 금수를 두지 않을것
            if (GameManager.Instance.ISSix(rowplace, colplace, GameManager.PlayerType.Black) ||
                GameManager.Instance.ISDoubleThree(rowplace, colplace, GameManager.PlayerType.Black) ||
                GameManager.Instance.ISDoubleFour(rowplace, colplace, GameManager.PlayerType.Black))
                return float.MaxValue;
            
            //플레이어 B(즉, AI)가 이긴 경우):
            if (CheckSimulGameWin(rowplace, colplace, GameManager.PlayerType.White) ||
                GameManager.Instance.ISDoubleThree(rowplace, colplace, GameManager.PlayerType.White) ||
                GameManager.Instance.ISDoubleFour(rowplace, colplace, GameManager.PlayerType.White))
                //최대한 AI가 빠르게 이길 수 있도록 턴이 지날때마다 DEPTH가 얻는 점수를 적게 만들어준다.
                return 10000 - depth * 100;

            //4나 열린3이 완성되면 당장 막아야함
            for (int i = 0; i < 4; i++)
            {
                if (GameManager.Instance.ISFour(rowplace, colplace, GameManager.PlayerType.Black, i))
                    return -5000 + depth * 100;

                if (GameManager.Instance.ISOpenThree(rowplace, colplace, GameManager.PlayerType.Black, i))
                    return -5000 + depth * 100;;

                if (GameManager.Instance.ISFour(rowplace, colplace, GameManager.PlayerType.White, i))
                    return 5000 - depth * 100;;

                if (GameManager.Instance.ISOpenThree(rowplace, colplace, GameManager.PlayerType.White, i))
                    return 5000 - depth * 100;;
                
                if (ISThree(rowplace, colplace, GameManager.PlayerType.Black, i))
                    return -1000;
                    
                if (ISTwo(rowplace, colplace, GameManager.PlayerType.Black, i))
                    return -500;

                if (ISThree(rowplace, colplace, GameManager.PlayerType.White, i))
                    return 1000;

                if (ISTwo(rowplace, colplace, GameManager.PlayerType.White, i))
                    return 500;
            }
            
        //모든 칸이 채워진 경우(무승부)
        if (IsAllBlocksPlaced(board))
        	//0점 반환
            return 0;
        

		//최대화 턴 (AI의 차례)
        if (isMaximizing)
        {
        	//최대점수의 초기값을 일단 무한이 낮은 값으로 설정
            var bestScore = float.MinValue;
            
            //보드 순회
            for (var row = rowplace - view; row <= rowplace + view; row++)
            {
                for (var col = colplace - view; col <= colplace + view; col++)
                {
                    //보드 범위 확인
                    if (!GameManager.Instance.CheckBoardRange(row, col))
                    {
                        //현제 순회중인 보드가 비어있다면
                        if (board[row, col] == GameManager.PlayerType.None)
                        {
                            //임시로 내 돌을 놓음
                            board[row, col] = GameManager.PlayerType.White;
                        
                            //DoMinimax 함수를 통해 점수를 계산(재귀)
                            //임시로 턴을 하나 진행시키는 것이므로 depth에 1턴 추가.
                            var score = DoMinimax(board, currentdepth + 1, false, row, col, alpha, beta);
                        
                            //임시로 놓은 돌 회수
                            board[row, col] = GameManager.PlayerType.None;
                        
                            if (score > bestScore)
                                bestScore = score;
                            
                            if (alpha < bestScore)
                                alpha = bestScore;
                            
                            if (beta <= alpha)
                                goto EndLoop;
                        }
                    }
                }
            }
            
            EndLoop:;
            //bestScore 반환
            return bestScore;
        }
        //최소화 턴 (플레이어의 차례)
        else
        {
        	//최대점수의 초기값을 일단 무한이 높은 값으로 설정
            var bestScore = float.MaxValue;
            
            //보드 순회
            for (var row = rowplace - view; row <= rowplace + view; row++)
            {
                for (var col = colplace - view; col <= colplace + view; col++)
                {
                    //보드 범위 확인
                    if (!GameManager.Instance.CheckBoardRange(row, col))
                    {
                        //현재 순회중인 보드가 비어있다면
                        if (board[row, col] == GameManager.PlayerType.None)
                        {
                            //임시로 플레이어의 돌을 놔둬봄
                            board[row, col] = GameManager.PlayerType.Black;
                        
                            //DoMinimax 함수를 통해 점수를 계산(재귀)
                            //임시로 턴을 하나 진행시키는 것이므로 depth에 1턴 추가.
                            var score = DoMinimax(board, currentdepth + 1, true, row, col, alpha, beta);
                        
                            //임시로 놓은 돌 회수
                            board[row, col] = GameManager.PlayerType.None;
                        
                            //bestScore와 score 중 더 작은 값을 bestScore로 재할당
                            if (score < bestScore)
                                bestScore = score;
                            
                            if (beta > bestScore)
                                beta = bestScore;
                            
                            if (beta <= alpha)
                                goto EndLoop;
                        }
                    }
                }
            }
            
            EndLoop:;
            //bestScore 반환
            return bestScore;
        }
    }
    
    /// <summary>
    /// 모든 마커가 보드에 배치 되었는지 확인하는 함수. 게임 매니저에 있던거
    /// </summary>
    /// <returns>True: 모두 배치</returns>
    public static bool IsAllBlocksPlaced(GameManager.PlayerType[,] board)
    {
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] == GameManager.PlayerType.None)
                    return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 게임의 승패를 판단하는 함수. 게임 매니저에 있던거
    /// </summary>
    /// <param name="playerType"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    private static bool CheckSimulGameWin(int row, int col, GameManager.PlayerType playerType)
    {
        if (playerType == GameManager.PlayerType.Black)
        {
            if (GameManager.Instance.ISFive(row, col, GameManager.PlayerType.Black))
                return true;
        }
        
        if (playerType == GameManager.PlayerType.White)
        {
            if (GameManager.Instance.ISFive(row, col, GameManager.PlayerType.White) ||
                GameManager.Instance.ISSix(row, col, GameManager.PlayerType.White))
                return true;
        }
        
        return false;
    }

    private static bool ISThree(int row, int col, GameManager.PlayerType playerType, int direction)
    {
        for (int i = 0; i < 2; i++)
        {
            int[] Emptypos = GameManager.Instance.FindEmptyPos(row, col, playerType, direction * 2 + i);

            if (Emptypos != null)
            {
                if (GameManager.Instance.ISFour(Emptypos[0], Emptypos[1], playerType, direction))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool ISTwo(int row, int col, GameManager.PlayerType playerType, int direction)
    {
        for (int i = 0; i < 2; i++)
        {
            int dirx = GameManager.Instance.dx[direction * 2 + i];
            int diry = GameManager.Instance.dy[direction * 2 + i];
            
            int newRow = row + diry;
            int newCol = col + dirx;

            if (!(GameManager.Instance.CheckBoardRange(newRow, newCol) ||
                  GameManager.Instance._board[newRow, newCol] != playerType))
                return true;
        }
        
        return false;
    }
    
}
