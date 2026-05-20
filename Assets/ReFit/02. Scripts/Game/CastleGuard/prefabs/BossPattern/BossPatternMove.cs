using UnityEngine;

public class BossPatternMove : MonoBehaviour
{
    public float speed = 2.0f;

    private CastleGuard_GameManager _gameManager;

    private const string PlayerTag = "Player";

    private void Start()
    {
        _gameManager = FindAnyObjectByType<CastleGuard_GameManager>();
        Destroy(gameObject, 14f);
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    /// <summary>
    /// 플레이어와 트리거 충돌 시 라이프를 하나 감소시키고 패턴 오브젝트를 제거합니다.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag)) return;

        _gameManager?.DecreaseLife();
        Destroy(gameObject);
    }

   /* /// <summary>
    /// 플레이어와 물리 충돌 시 라이프를 하나 감소시키고 패턴 오브젝트를 제거합니다.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(PlayerTag)) return;

        _gameManager?.DecreaseLife();
        Destroy(gameObject);
    }*/
}
