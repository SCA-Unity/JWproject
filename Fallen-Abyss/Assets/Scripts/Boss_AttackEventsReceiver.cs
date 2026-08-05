using UnityEngine;
using TwoBitMachines.FlareEngine;

public class Boss_AttackEventsReceiver : MonoBehaviour
{
    [SerializeField] private Melee melee;
    [SerializeField] private Collider2D hitbox;

    private void Awake()
    {
        if (melee == null)
            melee = GetComponent<Melee>();

        if (melee == null)
            melee = GetComponentInParent<Melee>();

        if (hitbox != null)
            hitbox.enabled = false;
    }

    // 공격 시작
    public void StartAttack()
    {
        Debug.Log("Boss Attack Start");
    }

    // 히트박스 ON
    public void EnableHitbox()
    {
        if (hitbox != null)
            hitbox.enabled = true;
    }

    // 히트박스 OFF
    public void DisableHitbox()
    {
        if (hitbox != null)
            hitbox.enabled = false;
    }

    // 공격 종료 (Flare Engine에 알림)
    public void CompleteAttack()
    {
        if (melee != null)
            melee.CompleteAttack();

        Debug.Log("Boss Attack Complete");
    }
}