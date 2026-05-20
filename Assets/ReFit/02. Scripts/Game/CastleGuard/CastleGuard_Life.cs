using TMPro;
using UnityEngine;

public class CastleGuard_Life : MonoBehaviour
{
    public CastleGuard_GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            gameManager.DecreaseLife();

            // "Enemy" 태그를 가진 가장 가까운 조상을 찾아 제거합니다.
            // transform.root 대신 태그 기반으로 탐색하여 부모 씬 오브젝트가 삭제되는 것을 방지합니다.
            Destroy(FindEnemyRoot(other.transform));
        }
    }

    /// <summary>
    /// "Enemy" 태그를 가진 가장 가까운 조상(또는 자기 자신)을 반환합니다.
    /// </summary>
    private GameObject FindEnemyRoot(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag("Enemy"))
                return t.gameObject;

            t = t.parent;
        }

        return null;
    }
}
