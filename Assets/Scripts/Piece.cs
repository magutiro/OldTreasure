using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    private PieceKind  pieceKind;
    private Image pieceImage;
    private PieceAnimation pieceAnimation;
    public PieceKind.piecekind piecekind { get; set; }
    public RectTransform pieceRectTransform { get; set; }
    public Vector2 boardPos { get; set; }
    public bool isDeletePiece { get; set; }

    [SerializeField]
    public PieceKind.piecekind kind;
    public Vector2 pos;
    public bool isdel;

    private void Update()
    {
        kind = piecekind;
        pos = boardPos;
        isdel = isDeletePiece;
    }
    private void Awake()
    {
        pieceAnimation = GetComponent<PieceAnimation>();
        pieceImage = GetComponent<Image>();
        pieceRectTransform = GetComponent<RectTransform>();
        pieceKind = GameObject.Find("PieceKind").gameObject.GetComponent<PieceKind>();

    }

    public void setAnimation(float dur)
    {
        pieceAnimation.SetMove(pieceRectTransform.position + new Vector3(0, 250, 0), pieceRectTransform.position, dur);
    }
    public void setAnimation(float dir,float dur,bool isHorizontal)
    {
        if (isHorizontal)
        {

            pieceAnimation.SetMove(pieceRectTransform.position + new Vector3(dir, 0, 0), pieceRectTransform.position, dur);
        }
        else
        {
            pieceAnimation.SetMove(pieceRectTransform.position + new Vector3(0, dir, 0), pieceRectTransform.position, dur);
        }
    }
    public void setSprite()
    {
        pieceKind.setSprite(pieceImage, piecekind);
    }

    
}
