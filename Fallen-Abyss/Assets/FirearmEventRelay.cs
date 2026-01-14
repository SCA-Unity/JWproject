using UnityEngine;
using TwoBitMachines.FlareEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Unity Animation Event는 Animator가 붙은 GameObject에서만 메서드를 찾습니다.
/// 이 스크립트는 Animation Event를 받아서, 실제 Firearm 컴포넌트로 전달(릴레이)합니다.
/// </summary>
public class FirearmEventRelay : MonoBehaviour
{
    [Header("Drag & Drop the Firearm component here (Gun object)")]
    [SerializeField] private Firearm firearm;

    [Tooltip("If empty at runtime, try to find Firearm in this object's parents/children.")]
    [SerializeField] private bool autoFindIfMissing = true;

    public Firearm Firearm => firearm;

    private void Reset()
    {
        // 컴포넌트 추가 직후 자동으로 한번 찾아줌
        TryAutoAssign();
    }

    private void Awake()
    {
        if (firearm == null && autoFindIfMissing)
            TryAutoAssign();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 에디터에서 값이 비어있으면 자동으로 찾아서 채워줌
        if (!Application.isPlaying && firearm == null && autoFindIfMissing)
            TryAutoAssign();
    }
#endif

    private void TryAutoAssign()
    {
        // 1) 자기 자신/자식에서 찾기
        firearm = GetComponentInChildren<Firearm>(true);
        if (firearm != null) return;

        // 2) 부모에서 찾기(캐릭터 루트 방향)
        firearm = GetComponentInParent<Firearm>(true);
    }

    // ---- Animation Events ----

    // 애니메이션 마지막 프레임(혹은 Loop Once Event)에서 호출
    public void ShootAndAnimationComplete()
    {
        if (EnsureFirearm()) firearm.ShootAndAnimationComplete();
    }

    // 발사 프레임에서 호출
    public void ShootAndWaitForAnimation()
    {
        if (EnsureFirearm()) firearm.ShootAndWaitForAnimation();
    }

    // 애니메이션 마지막 프레임에서 호출
    public void AnimationComplete()
    {
        if (EnsureFirearm()) firearm.AnimationComplete();
    }

    private bool EnsureFirearm()
    {
        if (firearm != null) return true;

        if (autoFindIfMissing)
            TryAutoAssign();

        if (firearm == null)
        {
            Debug.LogError(
                $"FirearmEventRelay: Firearm reference is missing on '{name}'. " +
                $"Assign the Firearm component in the Inspector, or enable autoFindIfMissing and ensure a Firearm exists in parent/children.",
                this
            );
            return false;
        }
        return true;
    }
}
