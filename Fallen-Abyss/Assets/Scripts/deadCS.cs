using UnityEngine;
using UnityEngine.SceneManagement;

public class deadCS : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger2D: " + other.name);

        if (other.CompareTag("Player"))
        {
            // 1) 플레이어 스크립트에 죽는 함수가 있을 때 (예: PlayerV3, PlayerController 등)
            /*
            PlayerV3 player = other.GetComponent<PlayerV3>();
            if (player != null)
            {
                player.Die();    // 실제 함수 이름에 맞게 수정
                return;
            }
            */

            // 2) 그냥 즉시 씬 리셋 (무조건 즉사)
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.buildIndex);
        }
    }
}