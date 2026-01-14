using UnityEngine;

public class TilemapTriggerMoveY : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveY = -1f;          // 한 번에 이동할 Y값 (위:+ / 아래:-)
    public float speed = 1f;           // 이동 속도
    public float maxMoveDistance = 5f; // 총 최대 이동 거리

    private Vector3 targetPos;
    private bool isMoving = false;
    private float movedDistance = 0f;

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (transform.position == targetPos)
        {
            isMoving = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isMoving) return;

        float remaining = maxMoveDistance - movedDistance;
        if (remaining <= 0f) return;

        float step = Mathf.Clamp(moveY, -remaining, remaining);

        targetPos = transform.position + new Vector3(0f, step, 0f);
        movedDistance += Mathf.Abs(step);
        isMoving = true;
    }
}
