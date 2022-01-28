using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Idle,
        PieceMove,
        MatchCheck,
        DeletePiece,
        FillPiece,
        Wait
    }
    [SerializeField]
    private Board board;
    [SerializeField]
    public List<Board> boards;
    [SerializeField]
    private GameState currentState;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Text countText;
    private Piece selectedPiece;
    private Piece tmpPiece;
    private Vector3 nowPos;

    public static int ChangeCount = 0;
    int HP = 30;

    public bool IsVertical;
    public bool IsHorizontal;
    // Start is called before the first frame update
    void Start()
    {
        ChangeCount = 0;
        int[] pieceList = {1,1,1,1,1,1};
        board.setRandomPiecekindList(pieceList);
        board.InitializeBorad();
        board.boardDir = BoardKind.BoardDir.Front;
        for (int i=0;i<boards.Count;i++)
        {
            boards[i].boardDir = (BoardKind.BoardDir)i+1;
            boards[i].setRandomPiecekindList(pieceList);
            boards[i].InitializeBorad();
        }

        currentState = GameState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.Idle:
                Idle();
                break;
            case GameState.PieceMove:
                PieceMove();
                break;
            case GameState.MatchCheck:
                MatchCheck();
                break;
            case GameState.DeletePiece:
                DeletePiece();
                break;
            case GameState.FillPiece:
                FillPiece();
                break;
            default:
                break;
        }
        if(ChangeCount >= HP)
        {
            GameOver();
        }else if (board.isGetTreger())
        {
            GameCrear();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(board.board[0,2].name);
        }
    }
    private void GameOver()
    {
        Debug.Log("崩壊");
        SceneManager.LoadScene("GameOverScene");
    }
    private void GameCrear()
    {
        Debug.Log("クリア");
        SceneManager.LoadScene("GameCrearScene");
    }
    private void FillPiece()
    {
        currentState = GameState.Wait;
        //StartCoroutine(board.FillPiece(() => currentState = GameState.MatchCheck));
        if(board.IsDeletePiece() >= 1)
        {
            StartCoroutine(board.FillPiece(() => currentState = GameState.MatchCheck));
            //board.FillPiece();
            //currentState = GameState.FillPiece;
        }
        else
        {
            //StartCoroutine(board.FillPiece(() => currentState = GameState.MatchCheck));
            return;
        }
        for (int i = 0; i < boards.Count; i++)
        {
            if (boards[i].IsDeletePiece() >= 1)
            {
                StartCoroutine(boards[i].FillPiece(() => currentState = GameState.FillPiece));
                continue;
            }
            else if (i == boards.Count)
            {
                //currentState = GameState.MatchCheck;
            }
        }

    }

    private void DeletePiece()
    {
        currentState = GameState.Wait;
        StartCoroutine(board.DeleteMatchPiece(() => currentState = GameState.FillPiece));
    }

    private void MatchCheck()
    {
        if (board.MatchPiece() || board.IsDeletePiece()>1)
        {
            currentState = GameState.DeletePiece;
        }
        else
        {
            currentState = GameState.Idle; 
            IsHorizontal = false;
            IsVertical = false;
        }
    }

    private void PieceMove()
    {
        if (Input.GetMouseButton(0))
        {
            var piece = board.GetNearestPiece(Input.mousePosition); 
            //piece = GetVectorPiece(nowPos - Input.mousePosition);
            //piece = GetVectorPiece((int)piece.boardPos.x - (int)selectedPiece.boardPos.x, (int)selectedPiece.boardPos.y - (int)piece.boardPos.y);
            if (tmpPiece != piece && piece != selectedPiece && isNearPiece(selectedPiece,piece))
            {
                //piece = GetVectorPiece(nowPos - Input.mousePosition);
                tmpPiece = piece;
                board.SwitchPiece(selectedPiece, piece);
            }
            else if (tmpPiece!=null&&tmpPiece != piece && piece == selectedPiece && isNearPiece(selectedPiece, piece))
            {
                //piece = GetVectorPiece(nowPos - Input.mousePosition);
                board.SwitchPiece(tmpPiece, piece);
                tmpPiece = piece;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            var piece = board.GetNearestPiece(Input.mousePosition);
            if (piece != selectedPiece)
            {
                ChangeCount++;
                countText.text = "崩壊まで" + (HP-ChangeCount) + "/" + HP;
                slider.value = (float)ChangeCount / HP;
                //board.SwitchPiece(selectedPiece, piece);
            }
            board.SmallDownPiece(selectedPiece);
            currentState = GameState.MatchCheck;
        }
    }
    private Piece GetVectorPiece(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (y > x && y > -x)
        {
            //下方向
            Debug.Log("下");
            return boards[0].board[(int)selectedPiece.boardPos.x, (int)selectedPiece.boardPos.y -1];
        }
        else if(y < x && y > -x)
        {
            //左方向
            Debug.Log("左");
            return boards[0].board[(int)selectedPiece.boardPos.x +1, (int)selectedPiece.boardPos.y];
        }
        else if(y > x && y < -x)
        {
            //右方向
            Debug.Log("右");
            return boards[0].board[(int)selectedPiece.boardPos.x -1, (int)selectedPiece.boardPos.y];
        }
        else if (y < x && y < -x)
        {
            //上方向
            Debug.Log("上");
            return boards[0].board[(int)selectedPiece.boardPos.x, (int)selectedPiece.boardPos.y +1];
        }
        return null;
    }
    private bool isNearPiece(Piece selectPiece,Piece ClickPiece)
    {
        if(Math.Pow((int)selectPiece.boardPos.x - (int)ClickPiece.boardPos.x, 2) == 1 && Math.Pow((int)selectPiece.boardPos.y - (int)ClickPiece.boardPos.y, 2) == 0)
        {
            return true;
        }
        else if (Math.Pow((int)selectPiece.boardPos.x - (int)ClickPiece.boardPos.x, 2) == 0 && Math.Pow((int)selectPiece.boardPos.y - (int)ClickPiece.boardPos.y, 2) == 1)
        {
            return true;
        }
        return false;
    }
    private void Idle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            nowPos = Input.mousePosition;
            selectedPiece = board.GetNearestPiece(Input.mousePosition);
            board.BigUpPiece(selectedPiece);
            tmpPiece = null;
            currentState = GameState.PieceMove;
        }
    }
}
