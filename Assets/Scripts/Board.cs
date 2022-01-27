using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Board : MonoBehaviour
{
    private const float FillPieceDuration = 0.2f;
    private const float SwitchPieceCuration = 0.02f;

    public Piece[,] board;
    public BoardKind.BoardDir boardDir;
    public bool IsLastBoard;

    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    private RectTransform canvasRect;
    [SerializeField]
    private TweenAnimationManager animManager;

    private List<AnimData> fillPieceAnim = new List<AnimData>();

    private int[] bx = {0,0,0,0,0,0 };
    private int[] by = { 0, 0, 0, 0, 0, 0 };

    List<PieceKind.piecekind> pieceList = new List<PieceKind.piecekind>();
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
    //初期化
    public void InitializeBorad()
    {
        board = new Piece[6, 6];
        for(int y = 0; y < 6; y++)
        {
            for(int x = 0; x < 6; x++)
            {
                //ボード[x,y]にピースを当てはめる。
                //左上0,0  右上5,0
                //左下0,5　右下5,5
                board[x, y] = transform.GetChild(y*6+x).gameObject.GetComponent<Piece>();
                board[x, y].boardPos = new Vector2(x,y);
                //ランダムにピースの種類を決定
                board[x, y].piecekind = pieceList[UnityEngine.Random.Range(0, 6)];
                board[x, y].setSprite();
            }
        }
        //もし繋がっていたらやり直し
        while (MatchPiece())
        {
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    board[x, y].piecekind = pieceList[UnityEngine.Random.Range(0, 6)];
                    board[x, y].setSprite();
                    gameManager.IsHorizontal = false;
                    gameManager.IsVertical = false;
                }
            }
        }
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                if (boardDir == BoardKind.BoardDir.Front)
                {
                    board[x, y].setAnimation(1f);
                }
            }
        }
        animManager.AddListAnimData(fillPieceAnim);
    }
    public bool setRandomPiecekindList(int[] list)
    {
        int countlist=0;
        foreach(int count in list)
        {
            countlist += count;
        }
        if (countlist > 6)
        {
            Debug.Log(countlist+"Error");
            return true;
        }
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < list[i]; j++)
            {
                pieceList.Add((PieceKind.piecekind)i);
            }
        }
        return false;
    }
    private void Start()
    {
    }

    //クリックした場所に近いピースを返す
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
    //選択されたピースと周囲の拡大
    public void BigUpPiece(Piece piece)
    {
        List<Piece> pieces = new List<Piece>();
        Vector2 pos = piece.boardPos;
        pieces.Add(board[(int)pos.x, (int)pos.y]);
        pieces.Add(board[(int)pos.x-1, (int)pos.y]);
        pieces.Add(board[(int)pos.x+1, (int)pos.y]);
        pieces.Add(board[(int)pos.x, (int)pos.y+1]);
        pieces.Add(board[(int)pos.x, (int)pos.y-1]);

        foreach(var p in pieces)
        {
            p.transform.localScale = p.transform.localScale*1.5f;
        }
    }
    //選択されたピースと周囲の縮小
    public void SmallDownPiece(Piece piece)
    {
        List<Piece> pieces = new List<Piece>();
        Vector2 pos = piece.boardPos;
        pieces.Add(board[(int)pos.x, (int)pos.y]);
        pieces.Add(board[(int)pos.x - 1, (int)pos.y]);
        pieces.Add(board[(int)pos.x + 1, (int)pos.y]);
        pieces.Add(board[(int)pos.x, (int)pos.y + 1]);
        pieces.Add(board[(int)pos.x, (int)pos.y - 1]);

        foreach (var p in pieces)
        {
            p.transform.localScale = p.transform.localScale / 1.5f;
        }

    }
    //ピースの入れ替え

    public void SwitchPiece(Piece p1,Piece p2)
    {

        var p1kind = p1.piecekind;
        p1.piecekind = p2.piecekind;
        p2.piecekind = p1kind;

        p1.setSprite(); p2.setSprite();
    }

    //マッチングしているピースの有無を判断
    public bool MatchPiece()
    {

        //OutputBoard();
        foreach (var piece in board)
        {
            if (IsMatchPiece(piece))
            {
                return true;
            }
        }
        return false;
    }
    //繋がっているかどうか判断
    private bool IsMatchPiece(Piece piece)
    {
        var kind = piece.piecekind;
        var pos = piece.boardPos;

        // 横縦方向にマッチするかの判定 自分自身をカウントするため +1 する
        var verticalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.up) + GetSameKindPieceNum(kind, pos, Vector2.down) + 1;
        // 横方向にマッチするかの判定 自分自身をカウントするため +1 する
        var horizontalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.right) + GetSameKindPieceNum(kind, pos, Vector2.left) + 1;

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
            if (IsInBoard(piecePos) && board[(int)piecePos.x, (int)piecePos.y].piecekind == kind && kind != PieceKind.piecekind.None && kind != PieceKind.piecekind.Gley)
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
        for(int i = 0; i < 6; i++)
        {
            bx[i] = 0;
            by[i] = 0;
        }
        // マッチしているピースの削除フラグを立てる
        foreach (var piece in board)
        {
            //piece.isDeletePiece = IsMatchPiece(piece);
        }
        foreach (var piece in board)
        {
            if (piece.piecekind != PieceKind.piecekind.None && IsMatchPiece(piece) && piece.piecekind != PieceKind.piecekind.Gley)
            {
                DeleteMatchPiece(piece.boardPos, piece.piecekind);
                //piece.piecekind = PieceKind.piecekind.None;
                yield return new WaitForSeconds(0.5f);
            }
        }
        endCallBadk();
    }
    void DeleteMatchPiece(Vector2 pos,PieceKind.piecekind kind)
    {
        if (!IsInBoard(pos))
        {
            return;
        }

        var piece = board[(int)pos.x, (int)pos.y];
        if(piece.isDeletePiece || piece.piecekind != kind)
        {
            return;
        }

        if (!IsMatchPiece(piece)) {
            return;
        }

        piece.isDeletePiece = true;
        Vector2[] directions= { Vector2.up,Vector2.down,Vector2.left,Vector2.right};
        foreach (var dir in directions)
        {
            DeleteMatchPiece(pos + dir, kind);
        }
        board[(int)pos.x, (int)pos.y].isDeletePiece = true;
        board[(int)pos.x, (int)pos.y].piecekind = PieceKind.piecekind.None;
        board[(int)pos.x, (int)pos.y].setSprite();
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
    public IEnumerator FillPiece(Action endCallBack)
    {
        for (int y = 5; y >= 0; y--)
        {
            for (int x = 0; x < 6; x++)
            {
                if (gameManager.IsVertical && boardDir == BoardKind.BoardDir.Front)
                {
                    FillPiece(new Vector2(x, y), Vector2.down);
                    continue;
                }
                if (gameManager.IsHorizontal && boardDir == BoardKind.BoardDir.Front)
                {
                    FillPiece(new Vector2(x, y), Vector2.right);
                    continue;
                }

                switch (boardDir)
                {
                    case BoardKind.BoardDir.Up:
                        UpFillPiece(new Vector2(x, y), Vector2.down);
                        break;
                    case BoardKind.BoardDir.Right:
                        RightFillPiece(new Vector2(x, y), Vector2.right);
                        break;
                }
            }
        }
        animManager.AddListAnimData(fillPieceAnim);
        FillPiece();
        yield return new WaitForSeconds(1f);
        endCallBack();
    }
    public void FillPiece()
    {
        for (int y = 5; y >= 0; y--)
        {
            for (int x = 0; x < 6; x++)
            {
                if (boardDir == BoardKind.BoardDir.Front)
                {
                    FillGreyPiece(new Vector2(x, y), Vector2.down);
                }
            }
        }
        for (int y = 5; y >= 0; y--)
        {
            for (int x = 0; x < 6; x++)
            {
                if (boardDir == BoardKind.BoardDir.Front)
                {
                    FillGreyPiece(new Vector2(x, y), Vector2.right);
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
                fillPieceAnim.Add(new AnimData(checkPiece.gameObject, piece.pieceRectTransform.position, FillPieceDuration));

                piece.piecekind = checkPiece.piecekind;
                board[(int)checkPos.x, (int)checkPos.y].piecekind = PieceKind.piecekind.None;
                piece.setSprite();
                board[(int)checkPos.x, (int)checkPos.y].setSprite();

                piece.isDeletePiece = false;
                board[(int)checkPos.x, (int)checkPos.y].isDeletePiece = true;
                if (gameManager.IsVertical)
                {
                    piece.setAnimation(250, 0.3f, false);
                    return;
                }
                else
                {
                    piece.setAnimation(250, 0.3f, gameManager.IsHorizontal);
                    return;
                }
            }
            checkPos += delDir;
        }
        if (gameManager.IsVertical)
        {
            by[(int)pos.x]++;
        }
        else
        {
            bx[(int)pos.y]++;
        }
        CreatePiece(pos);
    }
    private void FillGreyPiece(Vector2 pos, Vector2 delDir)
    {
        var piece = board[(int)pos.x, (int)pos.y];
        //ピースがグレーでなければ何もしない
        if (piece.piecekind != PieceKind.piecekind.None && piece.piecekind != PieceKind.piecekind.Gley)
        {
            return;
        }
        var checkPos = pos + delDir;
        while (IsInBoard(checkPos))
        {
            var checkPiece = board[(int)checkPos.x, (int)checkPos.y];
            //上または右のピースが、消えていなければ
            if (piece.piecekind != PieceKind.piecekind.None && checkPiece.piecekind != PieceKind.piecekind.Gley)
            {
                piece.piecekind = checkPiece.piecekind;
                board[(int)checkPos.x, (int)checkPos.y].piecekind = PieceKind.piecekind.Gley;
                piece.setSprite();
                board[(int)checkPos.x, (int)checkPos.y].setSprite();


                return;
            }
            checkPos += delDir;
        }
        CreatePiece(pos);
    }
    private void UpFillPiece(Vector2 pos)
    {
        var piece = board[(int)pos.x, (int)pos.y];
        board[(int)pos.x, (int)pos.y].piecekind = gameManager.boards[0].board[(int)pos.x, 5].piecekind;
        gameManager.boards[0].board[(int)pos.x, 5].piecekind = PieceKind.piecekind.None;

        gameManager.boards[0].board[(int)pos.x, 5].isDeletePiece =true;
        board[(int)pos.x, (int)pos.y].isDeletePiece = false;
        board[(int)pos.x, (int)pos.y].setSprite();
        gameManager.boards[0].board[(int)pos.x, 5].setSprite();
        for(int y = 5; y >= 0; y--)
        {
            gameManager.boards[0].UpFillPiece(new Vector2((int)pos.x, y), Vector2.down);
        }
        WaitTimer(0.2f);
    }
    void UpFillPiece(Vector2 pos,Vector2 delDir)
    {
        var piece = board[(int)pos.x, (int)pos.y];
        //ピースが消されていなければ何もしない
        if (piece.piecekind != PieceKind.piecekind.None && piece.piecekind != PieceKind.piecekind.Gley)
        {
            return;
        }
        var checkPos = pos + delDir;
        while (IsInBoard(checkPos))
        {
            var checkPiece = board[(int)checkPos.x, (int)checkPos.y];
            //上のピースが、消えていなければ
            if (checkPiece.piecekind != PieceKind.piecekind.None && checkPiece.piecekind != PieceKind.piecekind.Gley)
            {
                piece.piecekind = checkPiece.piecekind;
                checkPiece.piecekind = PieceKind.piecekind.None;
                piece.setSprite();
                checkPiece.setSprite();

                piece.isDeletePiece = false;
                checkPiece.isDeletePiece = false;
                return;
            }
            checkPos += delDir;
        }
        CreatePiece(pos);
    }
    private void RightFillPiece(Vector2 pos)
    {
        var piece = board[(int)pos.x, (int)pos.y].piecekind;
        board[(int)pos.x, (int)pos.y].piecekind = gameManager.boards[1].board[0, (int)pos.y].piecekind;
        gameManager.boards[1].board[0, (int)pos.y].piecekind = PieceKind.piecekind.None;

        gameManager.boards[1].board[0, (int)pos.y].isDeletePiece = true;
        board[(int)pos.x, (int)pos.y].isDeletePiece = false;

        gameManager.boards[1].board[0, (int)pos.y].setSprite();
        board[(int)pos.x, (int)pos.y].setSprite(); 
        for (int x = 0; x < 6; x++)
        {
            gameManager.boards[1].RightFillPiece(new Vector2(x,(int)pos.y), Vector2.right);
        }
        WaitTimer(0.2f);
    }
    void RightFillPiece(Vector2 pos, Vector2 delDir)
    {
        var piece = board[(int)pos.x, (int)pos.y];
        //ピースが消されていなければ何もしない
        if (piece.piecekind != PieceKind.piecekind.None && piece.piecekind != PieceKind.piecekind.Gley)
        {
            return;
        }
        var checkPos = pos + delDir;
        while (IsInBoard(checkPos))
        {
            var checkPiece = board[(int)checkPos.x, (int)checkPos.y];
            //上のピースが、消えていなければ
            if (checkPiece.piecekind != PieceKind.piecekind.None && checkPiece.piecekind != PieceKind.piecekind.Gley)
            {
                piece.piecekind = checkPiece.piecekind;
                checkPiece.piecekind = PieceKind.piecekind.None;
                piece.setSprite();
                checkPiece.setSprite();

                piece.isDeletePiece = false;
                checkPiece.isDeletePiece = true;

                return;
            }
            checkPos += delDir;
        }
        CreatePiece(pos);
    }
    private void CreatePiece(Vector2 pos)
    {
        if(IsLastBoard == true)
        {
            //Debug.Log("gley");
            board[(int)pos.x, (int)pos.y].piecekind = PieceKind.piecekind.Gley;
            board[(int)pos.x, (int)pos.y].isDeletePiece = false;
            board[(int)pos.x, (int)pos.y].setSprite();
            return;
        }
        if (boardDir == BoardKind.BoardDir.Front)
        {
            if (gameManager.boards.Count >=1 && gameManager.IsVertical)
            {
                UpFillPiece(pos);
                board[(int)pos.x, (int)pos.y].setAnimation(250, 0.3f, false);
            }
            else if (gameManager.boards.Count >= 2 && gameManager.IsHorizontal)
            {
                RightFillPiece(pos);
            }
            if (board[(int)pos.x, (int)pos.y].isDeletePiece)
            {
                board[(int)pos.x, (int)pos.y].piecekind = (PieceKind.piecekind)UnityEngine.Random.Range(1, Enum.GetNames(typeof(PieceKind.piecekind)).Length) - 1;
                board[(int)pos.x, (int)pos.y].isDeletePiece = false;

            }
        }
        else if(boardDir == BoardKind.BoardDir.Up)
        {
            board[(int)pos.x, (int)pos.y].piecekind = PieceKind.piecekind.Gley;
            board[(int)pos.x, (int)pos.y].isDeletePiece = false;
            if (IsDeletePiece() == 0)
            {
                gameManager.IsVertical = false;
            }
            else
            {
                Debug.Log("up");
            }
        }
        else if (boardDir == BoardKind.BoardDir.Right)
        {
            board[(int)pos.x, (int)pos.y].piecekind = PieceKind.piecekind.Gley;
            board[(int)pos.x, (int)pos.y].isDeletePiece = false; 
        }
        board[(int)pos.x, (int)pos.y].setSprite();

    }

    private IEnumerator WaitTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); 
    }

}
