using UnityEngine;
using TwoBitMachines.FlareEngine;

public class Boss_AttackEventsReceiver : MonoBehaviour
{
    [Header("Boss Melee")]
    public Melee melee;

    // 공격 시작
    public void StartAttack()
    {
        Debug.Log("보스 공격 시작");
    }

    // 무기 판정 ON
    public void EnableHitbox()
    {
        Debug.Log("Hitbox ON");
        // swordCollider.enabled = true;
    }

    // 무기 판정 OFF
    public void DisableHitbox()
    {
        Debug.Log("Hitbox OFF");
        // swordCollider.enabled = false;
    }

    // 공격 종료
    public void CompleteAttack()
    {
        if (melee == null)
            melee = GetComponentInParent<Melee>();

        if (melee != null)
            melee.CompleteAttack();
    }
}