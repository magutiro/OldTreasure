using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Piece selectedPiece;
    private Piece tmpPiece;

    public bool IsVertical;
    public bool IsHorizontal;
    // Start is called before the first frame update
    void Start()
    {
        int[] pieceList = {2,2,2,0,0,0};
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(board.board[0,2].name);
        }
    }

    private void FillPiece()
    {
        currentState = GameState.Wait;
        //StartCoroutine(board.FillPiece(() => currentState = GameState.MatchCheck));
        if(board.IsDeletePiece() >= 1)
        {
            StartCoroutine(board.FillPiece(() => currentState = GameState.FillPiece));
            //board.FillPiece();
            //currentState = GameState.FillPiece;
        }
        else
        {
            StartCoroutine(board.FillPiece(() => currentState = GameState.MatchCheck));
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
                currentState = GameState.MatchCheck;
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

            if (tmpPiece != piece && piece != selectedPiece)
            {
                tmpPiece = piece;
                board.SwitchPiece(selectedPiece, piece);
            }
            else if (tmpPiece!=null&&tmpPiece != piece && piece == selectedPiece)
            {
                board.SwitchPiece(tmpPiece, piece);
                tmpPiece = piece;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            var piece = board.GetNearestPiece(Input.mousePosition);
            if (piece != selectedPiece)
            {
                //board.SwitchPiece(selectedPiece, piece);
            }
            board.SmallDownPiece(selectedPiece);
            currentState = GameState.MatchCheck;
        }
    }

    private void Idle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedPiece = board.GetNearestPiece(Input.mousePosition);
            board.BigUpPiece(selectedPiece);
            tmpPiece = null;
            currentState = GameState.PieceMove;
        }
    }
}
