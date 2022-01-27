using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PieceTweenAnimation : MonoBehaviour
{
    public float AnimeSpeed;
    public void StartAnimation(GameObject gameObject,Vector3 fromPos)
    {
        gameObject.transform.DOMove(fromPos, AnimeSpeed);
    }
}
