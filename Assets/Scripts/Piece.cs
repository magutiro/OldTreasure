﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    private PieceKind  pieceKind;
    private Image pieceImage;
    public PieceKind.piecekind piecekind { get; set; }
    public RectTransform pieceRectTransform { get; set; }
    public Vector2 boardPos { get; set; }
    public bool isDeletePiece { get; set; }

    [SerializeField]
    public PieceKind.piecekind kind;
    public int x;
    public int y;
    public bool isdel;

    private void Update()
    {
        kind = piecekind;
        x =(int)boardPos.x;
        y = (int)boardPos.y;
        isdel = isDeletePiece;
    }
    private void Awake()
    {
        pieceImage = GetComponent<Image>();
        pieceRectTransform = GetComponent<RectTransform>();
        pieceKind = GameObject.Find("PieceKind").gameObject.GetComponent<PieceKind>();

    }

    public void setRectTransform(RectTransform rect)
    {
        pieceRectTransform.position = rect.position;
    }

    public void setSprite()
    {
        pieceKind.setSprite(pieceImage, piecekind);
    }
}