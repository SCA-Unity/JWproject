using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Elevator2D_Controller : MonoBehaviour
{
    public float moveY = -5f;   // 내려가는 거리 (음수면 아래)
    public float speed = 2f;
    public float delay = 0.5f;

    [Header("Passenger Carry")]
    public LayerMask passengerMask;     // 플레이어 레이어 넣기 권장 (예: Player)
    public float passengerCheckHeight = 0.1f; // 플랫폼 위 얇게 체크

    private Vector2 startPos;
    private Rigidbody2D rb;

    private bool isMoving;
    private bool finished;

    // 플랫폼 위 승객들
    private readonly HashSet<Rigidbody2D> passengers = new HashSet<Rigidbody2D>();

    // 프레임 간 이동량 계산용
    private Vector2 lastRbPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        startPos = rb.position;
        lastRbPos = rb.position;
    }

    void FixedUpdate()
    {
        if (!isMoving || finished)
        {
            lastRbPos = rb.position;
            return;
        }

        Vector2 targetPos = startPos + Vector2.up * moveY;

        Vector2 nextPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(nextPos);

        // 이번 프레임 플랫폼이 실제로 움직인 델타
        Vector2 delta = nextPos - lastRbPos;

        // 승객 갱신 + 델타 적용
        UpdatePassengers();
        CarryPassengers(delta);

        lastRbPos = nextPos;

        if (Vector2.Distance(nextPos, targetPos) < 0.001f)
        {
            rb.MovePosition(targetPos);

            // 마지막 스냅 델타도 적용(혹시 남으면)
            Vector2 snapDelta = targetPos - lastRbPos;
            if (snapDelta.sqrMagnitude > 0f)
            {
                UpdatePassengers();
                CarryPassengers(snapDelta);
            }

            isMoving = false;
            finished = true;
            lastRbPos = targetPos;
        }
    }

    // 🔹 외부 Trigger에서 호출
    public void MoveElevator()
    {
        if (isMoving || finished) return;
        StartCoroutine(StartMoveAfterDelay());
    }

    IEnumerator StartMoveAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        isMoving = true;
        lastRbPos = rb.position;
    }

    // 플랫폼 위(또는 접촉 중인) 승객 탐색
    private void UpdatePassengers()
    {
        passengers.Clear();

        // 플랫폼 콜라이더 바운드 기준으로 위쪽 얇게 박스 체크
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Bounds b = col.bounds;

        Vector2 boxCenter = new Vector2(b.center.x, b.max.y + passengerCheckHeight * 0.5f);
        Vector2 boxSize = new Vector2(b.size.x * 0.95f, passengerCheckHeight);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, passengerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            Rigidbody2D prb = hits[i].attachedRigidbody;
            if (prb != null && prb.bodyType == RigidbodyType2D.Dynamic)
                passengers.Add(prb);
        }
    }

    // 플랫폼 델타만큼 승객도 같이 이동
    private void CarryPassengers(Vector2 delta)
    {
        if (delta == Vector2.zero) return;

        foreach (var prb in passengers)
        {
            // FlareEngine Character는 velocity를 직접 관리하는 경우가 많아서
            // Rigidbody2D.MovePosition보다 transform 보정이 덜 튐
            prb.transform.position += (Vector3)delta;
        }
    }

#if UNITY_EDITOR
    // 승객 체크 박스 시각화
    void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Bounds b = col.bounds;
        Vector2 boxCenter = new Vector2(b.center.x, b.max.y + passengerCheckHeight * 0.5f);
        Vector2 boxSize = new Vector2(b.size.x * 0.95f, passengerCheckHeight);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
#endif
}