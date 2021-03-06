using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePieceAnm : MonoBehaviour
{
    public Vector3 fromPosition;
    public Vector3 toPosition;
    public float duration;
    public RectTransform rectPos;

    private bool isTween;
    private float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        duration = 1.0f;
        rectPos = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            rectPos.position += new Vector3(0,10,0);
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

        //
        var moveProgress = elapsedTime / duration;
        rectPos.position = Vector3.Lerp(fromPosition, toPosition, moveProgress);
        Debug.Log((fromPosition, toPosition, rectPos.position));
    }

    public void SetMove(Vector3 from, Vector3 to, float dur)
    {
        fromPosition = from;
        toPosition = to;
        duration = dur;

        rectPos.position = from;
        elapsedTime = 0;
        isTween = true;
    }
}
