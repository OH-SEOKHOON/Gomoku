using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    // [SerializeField] private CanvasGroup canvasGroupA;
    // [SerializeField] private CanvasGroup canvasGroupB;
    // [SerializeField] private Button gameOverButton;
    //
    // public enum GameUIMode
    // {
    //     Init,
    //     TurnA,
    //     TurnB,
    //     GameOver
    // }
    //
    // private const float DisableAlpha = 0.5f;
    // private const float EnableAlpha = 1f;
    //
    // public void SetGameUIMode(GameUIMode mode)
    // {
    //     switch (mode)
    //     {
    //         case GameUIMode.Init:
    //             canvasGroupA.gameObject.SetActive(true);
    //             canvasGroupB.gameObject.SetActive(true);
    //             gameOverButton.gameObject.SetActive(false);
    //             
    //             canvasGroupA.alpha = DisableAlpha;
    //             canvasGroupB.alpha = DisableAlpha;
    //             break;
    //         case GameUIMode.TurnA:
    //             canvasGroupA.gameObject.SetActive(true);
    //             canvasGroupB.gameObject.SetActive(true);
    //             gameOverButton.gameObject.SetActive(false);
    //             
    //             canvasGroupA.alpha = EnableAlpha;
    //             canvasGroupB.alpha = DisableAlpha;
    //             break;
    //         case GameUIMode.TurnB:
    //             canvasGroupA.gameObject.SetActive(true);
    //             canvasGroupB.gameObject.SetActive(true);
    //             gameOverButton.gameObject.SetActive(false);
    //             
    //             canvasGroupA.alpha = DisableAlpha;
    //             canvasGroupB.alpha = EnableAlpha;
    //             break;
    //         case GameUIMode.GameOver:
    //             canvasGroupA.gameObject.SetActive(false);
    //             canvasGroupB.gameObject.SetActive(false);
    //             gameOverButton.gameObject.SetActive(true);
    //             break;
    //     }
    // }

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
                                    
                                //백돌턴으로 넘김
                                GameManager.Instance._currentPlayer = GameManager.PlayerType.White;
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
                                GameManager.Instance._currentPlayer = GameManager.PlayerType.Black;
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