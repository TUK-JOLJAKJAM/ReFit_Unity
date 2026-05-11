using UnityEngine;

public class CastleGuard_EnemyDelete : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
