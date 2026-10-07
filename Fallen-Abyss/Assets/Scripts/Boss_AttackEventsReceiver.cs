using UnityEngine;
using TwoBitMachines.FlareEngine.AI;

public class Boss_AttackEventsReceiver : MonoBehaviour
{
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private Collider2D hitbox;

    private void Awake()
    {
        if (meleeAttack == null)
            meleeAttack = GetComponent<MeleeAttack>();

        if (meleeAttack == null)
            meleeAttack = GetComponentInParent<MeleeAttack>();

        if (hitbox == null && meleeAttack != null)
            hitbox = meleeAttack.colliderRef;

        if (hitbox != null)
            hitbox.enabled = false;
    }

    // 공격 시작
    public void StartAttack()
    {
        DisableHitbox();
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
        DisableHitbox();

        if (meleeAttack != null)
            meleeAttack.CompleteAttack();

        Debug.Log("Boss Attack Complete");
    }
}
