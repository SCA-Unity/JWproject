using UnityEngine;
using TwoBitMachines.FlareEngine;   // 플레어 엔진 Health 사용

public class FallDm : MonoBehaviour
{
    // 인스펙터에서 조절 가능
    public float damage = 999f;      // 줄 데미지 (플레이어를 한 번에 죽일 거면 체력보다 크게)
    public float damageForce = 5f;   // 피격 방향 힘 (넉백 등)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Health 컴포넌트가 등록된 객체만 데미지
        if (!Health.IsDamageable(other.transform))
            return;

        // 트랩 → 대상 방향 벡터
        Vector2 dir = (other.transform.position - transform.position).normalized;
        if (dir == Vector2.zero)
            dir = Vector2.up;

        // amount는 "체력 증감량"이라서, 데미지는 음수(-)로 넣어야 함
        Health.IncrementHealth(
            transform,              // 공격자(트랩)
            other.transform,        // 피해자(플레이어)
            -damage,                // 음수 = 데미지
            dir * damageForce       // 밀려나는 방향/힘
        );
    }
}