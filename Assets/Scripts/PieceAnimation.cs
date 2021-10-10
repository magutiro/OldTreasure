using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceAnimation : MonoBehaviour
{
    public Vector3 fromPosition;
    public Vector3 toPosition;
    public float duration;
    public RectTransform rectPos;

    private bool isTween;
    private float elapsedTime;
    void Awake()
    {
        duration = 1.0f;
        rectPos = GetComponent<RectTransform>();

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            rectPos.position += new Vector3(0, 10, 0);
        }
        if (!isTween)
        {
            return;
        }

        // アニメーション開始時からの経過時間
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration)
        {
            // アニメーションの終了処理
            rectPos.position = toPosition;
            isTween = false;
            return;
        }

        //アニメーションの進行％
        var moveProgress = elapsedTime / duration;
        rectPos.position = Vector3.Lerp(fromPosition, toPosition, moveProgress);
    }

    public void SetMove(Vector3 from, Vector3 to, float dur)
    {
        if (isTween) return;
        fromPosition = from;
        toPosition = to;
        duration = dur;

        rectPos.position = from;
        elapsedTime = 0;
        isTween = true;
    }
}
