using UnityEngine;

public class Elevator2D_AutoStart : MonoBehaviour
{
    public float moveY = 5f;   // 이동할 총 거리
    public float speed = 2f;   // 이동 속도

    private Vector3 startPos;
    private bool finished = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (finished) return;

        // 이동
        transform.position += Vector3.up * Mathf.Sign(moveY) * speed * Time.deltaTime;

        // 지금까지 이동한 거리
        float moved = Mathf.Abs(transform.position.y - startPos.y);

        // 지정한 거리 도달하면 정지
        if (moved >= Mathf.Abs(moveY))
        {
            transform.position = new Vector3(
                transform.position.x,
                startPos.y + moveY,
                transform.position.z
            );

            finished = true;
            Debug.Log("정확히 지정한 거리만큼 이동 후 정지");
        }
    }
}
