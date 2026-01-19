using System.Collections;
using UnityEngine;

public class Elevator2D_TriggerMove : MonoBehaviour
{
    public float moveY = 5f;   // + 위로 / - 아래로
    public float speed = 2f;

    private Vector3 startPos;
    private bool isMoving = false;
    private bool finished = false;
    private bool triggered = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isMoving || finished) return;

        // 이동 방향은 moveY의 부호로 결정
        transform.position += Vector3.up * Mathf.Sign(moveY) * speed * Time.deltaTime;

        float moved = Mathf.Abs(transform.position.y - startPos.y);

        // 목표 거리 도달
        if (moved >= Mathf.Abs(moveY))
        {
            transform.position = new Vector3(
                transform.position.x,
                startPos.y + moveY,
                transform.position.z
            );

            finished = true;
            isMoving = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(StartMoveAfterDelay());
        }
    }

    IEnumerator StartMoveAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isMoving = true;
    }


}
