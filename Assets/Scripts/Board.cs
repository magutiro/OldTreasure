using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Board : MonoBehaviour
{
    public Piece[,] board;
    public BoardKind.BoardDir boardDir;

    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    private RectTransform canvasRect;

    public bool IsLastBoard;
    void OutputBoard()
    {
        string str = "";
        for (int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                str += (board[i, j].piecekind+" ");
            }
            str += "\n";
        }
        Debug.Log(str);

    }
    void OutputInt()
    {
        string str = "";
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                str += (board[i, j].boardPos + " " +i + " "+j+" ");
            }
            str += "\n";
        }
        //Debug.Log(str);

    }
    public void InitializeBorad()
    {
        board = new Piece[6, 6];
        for(int y = 0; y < 6; y++)
        {
            for(int x = 0; x < 6; x++)
            {
                board[y, x] = transform.GetChild(y*6+x).gameObject.GetComponent<Piece>();
                board[y, x].boardPos = new Vector2(y, x);

                //Debug.Log(board[x,y].boardPos);

                board[y, x].piecekind = (PieceKind.piecekind)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PieceKind.piecekind)).Length-1);
                board[y, x].setSprite();
            }
        }
        while (MatchPiece())
        {
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    board[y, x].piecekind = (PieceKind.piecekind)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PieceKind.piecekind)).Length-1);
                    board[y, x].setSprite();
                    gameManager.IsHorizontal = false;
                    gameManager.IsVertical = false;
                }
            }
        }
    }
    private void Start()
    {
    }
    public Piece GetNearestPiece(Vector3 input)
    {
        OutputInt();
        var minDist = float.MaxValue;
        Piece nearestPiece = null;
        foreach (var p in board)
        {
            var dist = Vector3.Distance(input, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestPiece = p;
            }
        }
        return nearestPiece;
    }

    public void SwitchPiece(Piece p1,Piece p2)
    {
        var p1Position = p1.piecekind;
        p1.piecekind = p2.piecekind;
        p2.piecekind = p1Position;
        p1.setSprite(); p2.setSprite();
        //board[(int)p1.boardPos.x, (int)p1.boardPos.y] = p2;
        //board[(int)p2.boardPos.x, (int)p2.boardPos.y] = p1;
    }

    public bool MatchPiece()
    {

        //OutputBoard();
        //マッチングしているピースの有無を判断
        foreach (var piece in board)
        {
            if (IsMatchPiece(piece))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsMatchPiece(Piece piece)
    {
        var kind = piece.piecekind;
        var pos = piece.boardPos;

        // 横方向にマッチするかの判定 自分自身をカウントするため +1 する
        var horizontalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.up) + GetSameKindPieceNum(kind, pos, Vector2.down) + 1;
        // 縦方向にマッチするかの判定 自分自身をカウントするため +1 する
        var verticalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.right) + GetSameKindPieceNum(kind, pos, Vector2.left) + 1;

        if(verticalMatchCount >= 3)
        {
            gameManager.IsVertical = true;
        }
        if(horizontalMatchCount >= 3)
        {
            gameManager.IsHorizontal = true;
        }
        return verticalMatchCount >= 3 || horizontalMatchCount >= 3;
    }
    // 対象の方向に引数で指定したの種類のピースがいくつあるかを返す
    private int GetSameKindPieceNum(PieceKind.piecekind kind, Vector2 piecePos, Vector2 searchDir)
    {
        var count = 0;
        while (true)
        {
            piecePos += searchDir;
            if (IsInBoard(piecePos) && board[(int)piecePos.x, (int)piecePos.y].piecekind == kind)
            {
                count++;

            }
            else
            {
                break;
            }
        }
        return count;
    }

    // 対象の座標がボードに存在するか(ボードからはみ出していないか)を判定する
    public bool IsInBoard(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < 6 && pos.y < 6;
    }
    public IEnumerator DeleteMatchPiece(Action endCallBadk)
    {
        int count = 0;
        // マッチしているピースの削除フラグを立てる
        foreach (var piece in board)
        {
            piece.isDeletePiece = IsMatchPiece(piece);
            if (piece.isDeletePiece)
            {
                count++;
            }
            //Debug.Log(piece.isDeletePiece);
        }
        foreach (var piece in board)
        {
            if (piece.piecekind != PieceKind.piecekind.None && piece.isDeletePiece)
            {
                piece.piecekind = PieceKind.piecekind.None;
                piece.setSprite();
            }
        }
        yield return new WaitForSeconds(0.5f);
        endCallBadk();
    }
    public IEnumerator FillPiece(Action endCallBack)
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (gameManager.IsVertical)
                {
                    FillPiece(new Vector2(i, j), Vector2.left);
                    continue;
                }
                if (gameManager.IsHorizontal)
                {
                    FillPiece(new Vector2(i, j), Vector2.up);
                    continue;
                }

            }
        }
        yield return new WaitForSeconds(0.5f);
        endCallBack();
    }
    public void FillPiece()
    {
        //Debug.Log(name);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (gameManager.IsVertical)
                {
                    FillPiece(new Vector2(i, j), Vector2.left);
                    continue;
                }
                if (gameManager.IsHorizontal)
                {
                    FillPiece(new Vector2(i, j), Vector2.up);
                    continue;
                }

            }
        }
    }
    private void FillPiece(Vector2 pos,Vector2 delDir)
    {
        var piece = board[(int)pos.x, (int)pos.y];
        //ピースが消されていなければ何もしない
        if (piece.piecekind != PieceKind.piecekind.None && !piece.isDeletePiece)
        {
            return;
        }
        var checkPos = pos + delDir;
        while (IsInBoard(checkPos))
        {
            var checkPiece = board[(int)checkPos.x, (int)checkPos.y];
            //上または右のピースが、消えていなければ
            if (checkPiece.piecekind != PieceKind.piecekind.None && !checkPiece.isDeletePiece)
            {
                //Debug.Log(checkPiece.piecekind);
                piece.piecekind = checkPiece.piecekind;
                checkPiece.piecekind = PieceKind.piecekind.None;
                piece.setSprite();
                checkPiece.setSprite();

                piece.isDeletePiece = false;
                checkPiece.isDeletePiece = true;


                //StartCoroutine(FillPiece(() => Debug.Log("Fill2")));
                //board[(int)pos.x, (int)pos.y] = checkPiece;
                //board[(int)checkPos.x, (int)checkPos.y] = piece;
                //board[(int)checkPos.x, (int)checkPos.y].isDeletePiece = false;
                return;
            }
            checkPos += delDir;
        }
        //Debug.Log("check "+ pos + name);
        CreatePiece(pos);
    }

    public int IsDeletePiece()
    {
        var count = 0;
        foreach (var piece in board)
        {
            if (piece.isDeletePiece)
            {
                count++;
            }
        }
        //Debug.Log("check " + count + name);
        return count;
    }
    private void UpFillPiece(Vector2 pos)
    {
        var piece = board[(int)pos.x, (int)pos.y].piecekind;
        board[(int)pos.x, (int)pos.y].piecekind = gameManager.boards[0].board[5, (int)pos.y].piecekind;
        gameManager.boards[0].board[5, (int)pos.y].piecekind = piece;

        gameManager.boards[0].board[5, (int)pos.y].isDeletePiece = true;
        board[(int)pos.x, (int)pos.y].isDeletePiece = false;

        gameManager.boards[0].board[5, (int)pos.y].setSprite();
        board[(int)pos.x, (int)pos.y].setSprite();

        //Debug.Log("check " + pos + name);
        if(gameManager.boards[0].IsDeletePiece()>=1)
        {
            //gameManager.boards[0].FillPiece();
        }
    }
    private void CreatePiece(Vector2 pos)
    {
        if(IsLastBoard == true)
        {
            board[(int)pos.x, (int)pos.y].piecekind = PieceKind.piecekind.Blue;
        }
        if (boardDir == BoardKind.BoardDir.Front)
        {
            if (gameManager.boards.Count >=1 && gameManager.IsVertical)
            {
                UpFillPiece(pos);
            }
            if (gameManager.boards.Count >= 2 && gameManager.IsHorizontal)
            {

            }
            if (board[(int)pos.x, (int)pos.y].isDeletePiece)
            {
                board[(int)pos.x, (int)pos.y].piecekind = (PieceKind.piecekind)UnityEngine.Random.Range(1, Enum.GetNames(typeof(PieceKind.piecekind)).Length) - 1;
                board[(int)pos.x, (int)pos.y].isDeletePiece = false;

            }
        }
        else if(boardDir == BoardKind.BoardDir.Up)
        {
            Debug.Log("create " + pos + name);
            board[(int)pos.x, (int)pos.y].piecekind = (PieceKind.piecekind)UnityEngine.Random.Range(1, Enum.GetNames(typeof(PieceKind.piecekind)).Length) - 1;
            board[(int)pos.x, (int)pos.y].isDeletePiece = false;
        }
        board[(int)pos.x, (int)pos.y].setSprite();
        if (IsDeletePiece() > 0)
        {
            //FillPiece();
        }
        else
        {
            //gameManager.IsHorizontal = false;
            //gameManager.IsVertical = false;
        }
    }

}
