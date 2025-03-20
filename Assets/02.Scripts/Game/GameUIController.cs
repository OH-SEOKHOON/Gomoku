using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroupB;
    [SerializeField] private CanvasGroup canvasGroupW;
    
    [SerializeField] private Timer _timer;
    public enum GameUIMode
    {
        TurnB,
        TurnW,
        GameOver
    }
    
    private const float DisableAlpha = 0.2f;
    private const float EnableAlpha = 1f;
    
    public void SetGameUIMode(GameUIMode mode)
    {
        switch (mode)
        {
            
            case GameUIMode.TurnB:
                canvasGroupB.gameObject.SetActive(true);
                canvasGroupW.gameObject.SetActive(true);
                
                canvasGroupB.alpha = EnableAlpha;
                canvasGroupW.alpha = DisableAlpha;
                
                _timer.InitTimer();
                _timer.StartTimer();

                _timer.OnTimeout = () => GameManager.Instance.EndGame(GameManager.GameResult.Lose);
                
                break;
            
            case GameUIMode.TurnW:
                canvasGroupB.gameObject.SetActive(true);
                canvasGroupW.gameObject.SetActive(true);
                
                canvasGroupB.alpha = DisableAlpha;
                canvasGroupW.alpha = EnableAlpha;
                
                _timer.InitTimer();
                _timer.StartTimer();
                
                _timer.OnTimeout = () => GameManager.Instance.EndGame(GameManager.GameResult.Win);
                
                break;
            
            case GameUIMode.GameOver:
                canvasGroupB.gameObject.SetActive(true);
                canvasGroupW.gameObject.SetActive(true);
                
                canvasGroupB.alpha = EnableAlpha;
                canvasGroupW.alpha = EnableAlpha;
                
                _timer.InitTimer();
                
                break;
        }
    }

	//게임종료 버튼이 클릭되면 호출될 메서드
    public void OnClickSetButton()
    {
    	switch (GameManager.Instance._currentPlayer)
                {
                    case GameManager.PlayerType.Black:
                    {
                        if (GameManager.Instance.SetNewBoardValue(GameManager.PlayerType.Black, GameManager.Instance._lastPos[0], GameManager.Instance._lastPos[1]))
                        {
                            //CheckGameResult()를 호출하여 승리, 패배 확인
                            var gameResult = GameManager.Instance.CheckGameResult(GameManager.Instance._lastPos[0], GameManager.Instance._lastPos[1], GameManager.PlayerType.Black);
                                
                            //결과가 NONE(결과안나옴)이라면
                            if (gameResult == GameManager.GameResult.None)
                            {
                                //금수 테스트
                                GameManager.Instance.Renju(GameManager.Instance._lastPos[0], GameManager.Instance._lastPos[1], GameManager.PlayerType.Black);
                                
                                //금수 배치 해제
                                GameManager.Instance.ReplacedBan();
                                
                                //보드가 꽉찼는지 확인
                                GameManager.Instance.IsAllBlocksPlaced();
                                    
                                //백돌턴으로 넘김
                                GameManager.Instance._currentPlayer = GameManager.PlayerType.White;
                                SetGameUIMode(GameUIMode.TurnW);
                            }
                            else
                                //결과났으면 게임 종료 메서드 출력
                                GameManager.Instance.EndGame(gameResult);
                            
                        }
                        else
                        {
                            Debug.Log("이미 둔곳입니다");
                        }
                        break;
                    }
        
                    case GameManager.PlayerType.White:
                    {
                        if (GameManager.Instance.SetNewBoardValue(GameManager.PlayerType.White, GameManager.Instance._lastPos[0], GameManager.Instance._lastPos[1]))
                        {
                            var gameResult = GameManager.Instance.CheckGameResult(GameManager.Instance._lastPos[0], GameManager.Instance._lastPos[1], GameManager.PlayerType.White);
                            
                            if (gameResult == GameManager.GameResult.None)
                            {
                                //금수 배치
                                GameManager.Instance.PlacedBan();
                                
                                //보드가 꽉찼는지 확인
                                GameManager.Instance.IsAllBlocksPlaced();
                                
                                //흑돌 턴으로 넘김
                                GameManager.Instance._currentPlayer = GameManager.PlayerType.Black;
                                SetGameUIMode(GameUIMode.TurnB);
                            }
                            else
                                GameManager.Instance.EndGame(gameResult);
                        }
                        else
                        {
                            Debug.Log("이미 둔곳입니다");
                        }
                        break;
                    }
                }
    }
}