/*
 * Author: Dimitrios Gkaltsidis (Original), Gemini (Modification)
 * Date: 27 Sept 2023 (Original) / Nov 10 2025 (Modification)
 * Disclaimer: This code is not fully optimized. For production-level 2D character functionality, consider crafting your own.
 * Version: 1.0.2 (Modified for Timed Block)
 * Note: Death and related inputs/logic have been removed. All other inputs are now public KeyCodes.
 * Block is now a timed action, not a hold action.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldHunter_PixelArt : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Animator animator;

    // VFX
    [SerializeField] GameObject vfxObject;

    // --- PUBLIC KEY INPUTS (Inspector에서 설정 가능) ---
    [Header("Input Key Bindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode rollKey = KeyCode.E;
    public KeyCode getHitKey = KeyCode.T;
    public KeyCode blockKey = KeyCode.Q;
    public KeyCode attackKey = KeyCode.P;
    public KeyCode specialAttackKey = KeyCode.O;
    // 움직임 키(A, D)는 Input.GetAxis("Horizontal")을 사용하므로 별도 KeyCode는 불필요합니다.

    // Animation names for animator
    private string idleAnim = "Idle";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string rollAnim = "Roll";
    private string hurtAnim = "Hurt";
    private string blockAnim = "Block";
    private string blockImpactAnim = "BlockImpact";
    private string attack1Anim = "Attack1";
    private string attack2Anim = "Attack2";
    private string attack3Anim = "Attack3";
    private string specialAttack = "SpecialAttack";

    // Movement variables
    private float moveSpeed = 8f;
    private bool isRunningLeft;
    private bool isRunningRight;

    // Jumping variables
    private float jumpForce = 8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] float groundCheckRadius = 0.2f;

    // Rolling variables
    private float rollForce = 25f;

    // --- MODIFICATION START ---
    [Header("Action Settings")]
    [Tooltip("막기(Block)가 자동으로 해제되기까지 걸리는 시간(초)")]
    [SerializeField] private float blockDuration = 0.5f; // 막기 지속 시간
    // --- MODIFICATION END ---

    // Other variables
    private bool isGrounded;
    private bool isHoldingBlock;
    private bool canContinueAttackCombo;
    private int currentAttackAnim;

    // Can receive input
    private bool canReceiveInput;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        canReceiveInput = true;
        isHoldingBlock = false;
        canContinueAttackCombo = false;
        currentAttackAnim = 0;
    }

    private void Update()
    {
        GetMoveInput();
        GetJumpInput();
        GetRollInput();
        GetGetHitInput();
        GetHoldBlockInput();
        GetAttackInput();
        GetSpecialAttackInput();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        Run();
        CheckIfGrounded();
    }

    #region INPUTS
    private void GetMoveInput()
    {
        if (canReceiveInput)
        {
            float moveForce = (Input.GetAxis("Horizontal"));

            if (moveForce < 0)
            {
                isRunningLeft = true;
                isRunningRight = false;
            }
            else if (moveForce > 0)
            {
                isRunningRight = true;
                isRunningLeft = false;
            }

            // Input.GetAxis는 키를 떼도 서서히 0으로 돌아오므로, 키를 눌렀는지 명시적으로 확인하여 멈춤 처리를 합니다.
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                StopMovement();
            }
        }
    }
    private void GetJumpInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                if (isGrounded)
                {
                    Jump();
                }
            }
        }
    }
    private void GetRollInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(rollKey))
            {
                Roll();
            }
        }
    }
    private void GetGetHitInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(getHitKey))
            {
                GetHit();
            }
        }
    }

    // --- MODIFICATION START ---
    private void GetHoldBlockInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(blockKey))
            {
                HoldBlock();
            }
        }
        else
        {
            // 블록 중 블록 임팩트 처리는 canReceiveInput이 false일 때도 가능해야 합니다.
            if (isHoldingBlock && Input.GetKeyDown(rollKey)) // 롤 키(E)를 블록 임팩트 테스트용으로 활용
            {
                GetHitWhileBlocking();
            }
            
            // Input.GetKeyUp(blockKey)로 EndBlocking()을 호출하던 로직을 제거했습니다.
            // 이제 HoldBlock() 내부의 Invoke가 자동으로 EndBlocking()을 호출합니다.
        }
    }
    // --- MODIFICATION END ---

    private void GetAttackInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(attackKey))
            {
                Attack();
            }
        }
        else
        {
            // 공격 중 연계 입력은 canReceiveInput이 false일 때도 받아야 합니다.
            if (Input.GetKeyDown(attackKey) && canContinueAttackCombo)
            {
                Attack();
            }
        }
    }
    private void GetSpecialAttackInput()
    {
        if (canReceiveInput)
        {
            if (Input.GetKeyDown(specialAttackKey))
            {
                SpecialAttack();
            }
        }
    }

    private void ReEnableInput()
    {
        canReceiveInput = true;
    }
    #endregion

    #region IDLE & RUN LOGIC
    private void Run()
    {
        // ... (Run 로직은 변경 없음)
        if (isRunningLeft)
        {
            rb2D.velocity = new Vector2(-moveSpeed, rb2D.velocity.y);
            if (canReceiveInput)
            {
                animator.Play(runAnim);
            }
        }
        else if (isRunningRight)
        {
            rb2D.velocity = new Vector2(moveSpeed, rb2D.velocity.y);
            if (canReceiveInput)
            {
                animator.Play(runAnim);
            }
        }
        else if (!isRunningLeft && !isRunningRight && canReceiveInput)
        {
            if (canReceiveInput)
            {
                animator.Play(idleAnim);
            }
        }
    }
    private void StopMovement()
    {
        // ... (StopMovement 로직은 변경 없음)
        if (isRunningLeft || isRunningRight)
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }

        isRunningLeft = false;
        isRunningRight = false;
    }
    #endregion

    #region JUMPING LOGIC
    private void Jump()
    {
        canReceiveInput = false;
        rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
        animator.Play(jumpAnim);
        InvokeRepeating("EndJump", 0.1f, 0.1f);
    }
    private void EndJump()
    {
        if (isGrounded)
        {
            ReEnableInput();
            CancelInvoke();
        }
    }
    private void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }
    #endregion

    #region ROLLING LOGIC
    private void Roll()
    {
        canReceiveInput = false;
        StopMovement();
        rb2D.velocity = new Vector2(rollForce * GetPlayerDirection(), rb2D.velocity.y);
        animator.Play(rollAnim);
        // 애니메이션 이벤트나 Invoke를 사용하여 ReEnableInput() 호출 필요
    }
    #endregion

    #region GETHIT LOGIC
    private void GetHit()
    {
        canReceiveInput = false;
        StopMovement();
        animator.Play(hurtAnim);
        // 애니메이션 이벤트나 Invoke를 사용하여 ReEnableInput() 호출 필요
    }
    #endregion

    #region HOLDBLOCK LOGIC

    // --- MODIFICATION START ---
    private void HoldBlock()
    {
        isHoldingBlock = true;
        canReceiveInput = false;
        StopMovement();
        animator.Play(blockAnim);

        // blockDuration (예: 0.5초) 후에 EndBlocking을 호출하여 막기를 자동으로 해제합니다.
        Invoke("EndBlocking", blockDuration);
    }
    // --- MODIFICATION END ---

    private void GetHitWhileBlocking()
    {
        // EndBlocking을 호출하는 Invoke가 예약되어 있다면 취소합니다.
        // (피격 임팩트 애니메이션이 끝나고 ContinueBlocking이나 EndBlocking이 
        //  애니메이션 이벤트로 호출될 것을 예상)
        CancelInvoke("EndBlocking");
        animator.Play(blockImpactAnim);
    }
    private void ContinueBlocking()
    {
        // BlockImpact 애니메이션이 끝난 후 다시 Block 애니메이션을 재생할 때 호출됩니다.
        // (이 함수가 애니메이션 이벤트로 연결되어 있다고 가정)
        // timed block으로 변경되었으므로, 여기서 다시 EndBlocking 타이머를 걸 수 있습니다.
        // 하지만, 원래 로직(HoldBlock)을 최대한 유지하기 위해 여기서는 애니메이션만 재생합니다.
        // 만약 BlockImpact 후에도 다시 정해진 시간만큼 막기를 원한다면
        // 여기서 Invoke("EndBlocking", blockDuration); 를 다시 호출해야 합니다.
        animator.Play(blockAnim);

        // BlockImpact 후에도 정해진 시간만큼 막기를 유지하도록 수정합니다.
        Invoke("EndBlocking", blockDuration);
    }
    private void EndBlocking()
    {
        // 다른 Invoke 호출에 의해 이미 EndBlocking이 실행되었을 수 있으므로,
        // isHoldingBlock 상태를 확인합니다.
        if (isHoldingBlock)
        {
            ReEnableInput();
            isHoldingBlock = false;
        }
    }
    #endregion

    #region ATTACK LOGIC
    private void Attack()
    {
        if (!canContinueAttackCombo)
        {
            canReceiveInput = false;
            StopMovement();
            currentAttackAnim = 0;
            DisableCanContinueAttackCombo();
            animator.Play(attack1Anim);
            currentAttackAnim++;
        }
        else
        {
            if (currentAttackAnim == 0) // 이 부분은 첫 공격이 끝난 후 Combo가 true일 때만 실행되어야 하지만, 원본 로직을 유지
            {
                DisableCanContinueAttackCombo();
                animator.Play(attack1Anim);
                currentAttackAnim++;
            }
            else if (currentAttackAnim == 1)
            {
                DisableCanContinueAttackCombo();
                animator.Play(attack2Anim);
                currentAttackAnim++;
            }
            else if (currentAttackAnim == 2)
            {
                DisableCanContinueAttackCombo();
                animator.Play(attack3Anim);
                currentAttackAnim = 0;
            }
        }
    }
    private void EnableCanContinueAttackCombo()
    {
        canContinueAttackCombo = true;
    }
    private void DisableCanContinueAttackCombo()
    {
        canContinueAttackCombo = false;
    }
    #endregion

    #region SPECIAL ATTACK LOGIC
    private void SpecialAttack()
    {
        canReceiveInput = false;
        StopMovement();
        animator.Play(specialAttack);
        // 애니메이션 이벤트나 Invoke를 사용하여 ReEnableInput() 호출 필요
    }
    #endregion

    #region SPAWN VFX OBJECT
    private void SpawnVFX()
    {
        GameObject myVfx = Instantiate(vfxObject);
        myVfx.transform.position = gameObject.transform.position;
        myVfx.transform.localScale = gameObject.transform.localScale;
    }
    #endregion

    private float GetPlayerDirection()
    {
        return transform.localScale.x;
    }

    private void FlipSprite()
    {
        if (rb2D.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb2D.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}