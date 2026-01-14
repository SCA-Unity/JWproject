using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("엘리베이터 설정")]
    public float moveY = 5f;   // 인스펙터에서 입력
    public float speed = 2f;   // 인스펙터에서 입력

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + new Vector3(0f, moveY, 0f);
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isMoving = true;
        }
    }
}
