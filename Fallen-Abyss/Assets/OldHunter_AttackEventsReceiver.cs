using UnityEngine;
using TwoBitMachines.FlareEngine;

public class OldHunter_AttackEventsReceiver : MonoBehaviour
{
    [Header("Optional: if you want to control Flare Melee completion")]
    public Melee melee; // 필요하면 연결

    // 공격 애니메이션에서 "다음 콤보 입력 가능" 창 열기
    public void EnableCanContinueAttackCombo()
    {
        // 여기서 '다음 입력 허용' 플래그를 켜는 용도
        // 지금은 최소 구현: 비워둬도 이벤트 수신 에러는 사라짐
        // (추가로 Flare Melee 타이밍을 쓰면 이 함수는 없어도 됨)
    }

    // 공격 애니메이션에서 "다음 콤보 입력 불가"로 닫기
    public void DisableCanContinueAttackCombo()
    {
        // 입력 창 닫기 용도
    }

    // 입력 재활성화(경직/입력락 해제 등)
    public void ReEnableInput()
    {
        // 입력락을 쓰는 경우에만 구현
    }

    // Flare Melee를 쓰는 경우 공격 종료 신호(권장)
    public void CompleteAttack()
    {
        if (melee == null) melee = GetComponentInParent<Melee>();
        if (melee != null) melee.CompleteAttack();
    }
}
