using UnityEngine;
using TwoBitMachines.FlareEngine.ThePlayer;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectRange = 5f;
    public float attackRange = 1f;

    private Transform player;
    private Animator anim;

    void Start()
    {
        // FlareEngine Player 시스템과 우선 연동
        Transform mainPlayer = Player.PlayerTransform();
        if (mainPlayer != null)
        {
            player = mainPlayer;
        }
        else
        {
            // 예전 방식(태그)도 백업용으로 유지
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null)
        {
            // 플레이어가 사라졌거나 아직 초기화 안 되었을 때는 다시 시도
            Transform mainPlayer = Player.PlayerTransform();
            if (mainPlayer != null)
                player = mainPlayer;
            else
                return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= detectRange)
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    void Idle()
    {
        anim.SetBool("isWalking", false);
    }

    void Chase()
    {
        anim.SetBool("isWalking", true);

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(player.position.x, transform.position.y),
            moveSpeed * Time.deltaTime
        );

        // ???? ???
        if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Attack()
    {
        anim.SetBool("isWalking", false);
        anim.SetTrigger("attack");
    }
}
