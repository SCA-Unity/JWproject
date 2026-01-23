using UnityEngine;

public class PlayerElevatorLock : MonoBehaviour
{
    [Header("Elevator Lock")]
    public bool lockJumpOnElevator = true;

    private bool onElevator = false;
    private Vector2 velocity;

    void FixedUpdate()
    {
        if (onElevator)
        {
            // 바닥에 강제로 붙임
            velocity.y = -2f;
        }

        // 이동 적용 (기존 이동 로직과 병합 가능)
        transform.position += (Vector3)(velocity * Time.fixedDeltaTime);
    }

    // 엘리베이터에 닿으면
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Elevator"))
        {
            onElevator = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Elevator"))
        {
            onElevator = false;
        }
    }

    // 🔒 점프 차단용 함수
    public bool CanJump()
    {
        if (lockJumpOnElevator && onElevator)
            return false;

        return true;
    }
}
