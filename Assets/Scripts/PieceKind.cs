using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceKind : MonoBehaviour
{
    [SerializeField]
    Sprite[] pieceSprite;
    public enum piecekind
    {
        Red = 0,
        Blue,
        Green,
        Yellow,
        Pink,
        Puple = 5,
        None,
    }

    public void setSprite(Image pieceImage,piecekind pk)
    {
        if(pk == piecekind.None)
        {
            pieceImage.sprite = null;
            return;
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                //Debug.Log(pk.ToString()+pieceSprite[i].name);
                if (pk.ToString() == pieceSprite[i].name)
                {
                    pieceImage.sprite = pieceSprite[i];
                }
            }
        }
    }
}