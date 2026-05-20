using UnityEngine;

public class CastleGuard_EnemyDelete : MonoBehaviour
{
    public CastleGuard_GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<CastleGuard_GameManager>();

        if (gameManager == null)
            Debug.LogError("[CastleGuard_EnemyDelete] CastleGuard_GameManager를 씬에서 찾을 수 없습니다.", this);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent.CompareTag("Bullet"))
        {
            gameManager.AddPoint();
            Destroy(gameObject);
            Destroy(collision.transform.parent.gameObject);
        }
    }
}