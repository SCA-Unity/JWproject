using UnityEngine;
using System.Diagnostics;

public class RotationStackTraceProbe : MonoBehaviour
{
    float prevY;

    void Start()
    {
        prevY = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        float y = transform.eulerAngles.y;

        // 0 <-> 180처럼 큰 변화만 잡기
        if (Mathf.Abs(Mathf.DeltaAngle(prevY, y)) > 90f)
        {
            var st = new StackTrace(true);
            UnityEngine.Debug.LogError(
                $"[ROTATION CHANGED] {prevY} -> {y}  pos={transform.position}\n{st}"
            );
        }

        prevY = y;
    }
}
