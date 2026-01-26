using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Elevator2D_TriggerMove : MonoBehaviour
{
    public float moveY = 5f;   // + 위로 / - 아래로
    public float speed = 2f;

    private Vector2 startPos;
    private Rigidbody2D rb;

    private bool isMoving = false;
    private bool finished = false;
    private bool triggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;

        startPos = rb.position;
    }

    void FixedUpdate()
    {
        if (!isMoving || finished) return;

        Vector2 targetPos = startPos + Vector2.up * moveY;

        rb.MovePosition(
            Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime)
        );

        if (Vector2.Distance(rb.position, targetPos) < 0.01f)
        {
            rb.MovePosition(targetPos);
            isMoving = false;
            finished = true;
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
