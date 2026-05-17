using UnityEngine;

public class CastleGuard_Life : MonoBehaviour
{
    public CastleGuard_GameManager gameManagaer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            gameManagaer.DecreaseLife();
            Destroy(other.gameObject);
        }
    }
}
