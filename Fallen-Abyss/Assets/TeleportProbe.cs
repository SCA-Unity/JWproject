using UnityEngine;

public class TeleportProbe : MonoBehaviour
{
    public Transform oldhunter; // 드래그해서 연결
    public float jumpThreshold = 1.0f; // 1 유닛 이상 튀면 경고

    Vector3 prevPlayerPos;
    Vector3 prevHunterPos;

    void Start()
    {
        prevPlayerPos = transform.position;
        if (oldhunter != null) prevHunterPos = oldhunter.position;
    }

    void LateUpdate()
    {
        var p = transform.position;
        var dp = p - prevPlayerPos;

        Vector3 h = Vector3.zero;
        Vector3 dh = Vector3.zero;
        if (oldhunter != null)
        {
            h = oldhunter.position;
            dh = h - prevHunterPos;
        }

        // 큰 점프 감지
        if (dp.magnitude >= jumpThreshold || dh.magnitude >= jumpThreshold)
        {
            Debug.LogWarning(
                $"[TELEPORT] Player Δ={dp} pos={p} rot={transform.eulerAngles} scale={transform.localScale}\n" +
                $"           Hunter Δ={dh} pos={(oldhunter ? h.ToString() : "null")} rot={(oldhunter ? oldhunter.eulerAngles.ToString() : "null")} scale={(oldhunter ? oldhunter.localScale.ToString() : "null")}"
            );
        }

        prevPlayerPos = p;
        if (oldhunter != null) prevHunterPos = h;
    }
}
