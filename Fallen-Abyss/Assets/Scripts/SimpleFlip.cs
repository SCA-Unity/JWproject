using UnityEngine;

public class SimpleFlip : MonoBehaviour
{
    public Rigidbody2D rb; // Player 쪽 Rigidbody2D 넣을 자리
    /*
    void LateUpdate()
    {
        if (rb == null) return;

        // 살짝 여유를 둔 임계값
        if (rb.velocity.x > 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x < -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }*/
    public void Flip()
    {
        if (rb == null) return;

        // 살짝 여유를 둔 임계값
        if (rb.velocity.x > 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x < -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
