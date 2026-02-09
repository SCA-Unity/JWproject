using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectRange = 5f;
    public float attackRange = 1f;

    private Transform player;
    private Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
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

        // 방향 전환
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
