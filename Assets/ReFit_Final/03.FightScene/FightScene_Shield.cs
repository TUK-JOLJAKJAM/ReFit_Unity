using System.Collections;
using UnityEngine;

public class FightScene_Shield : MonoBehaviour
{
    public FightScene_PlayerUI playerUI;

    public AudioSource audioSource;
    public AudioClip clip;
    // --- 적 UI 삭제 ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        guardPoint++;
        fightScene_Guard.EnemyCount--;
        playerUI.UpdateShieldBar(guardPoint);
        audioSource.PlayOneShot(clip);
    }

    // --- 가드포인트 ---
    Coroutine guardCoroutine;
    public int guardPoint = 0;
    public FightScene_Guard fightScene_Guard;

    public IEnumerator guardRoutine()
    {
        guardPoint = 0;

        yield return null;

        if (fightScene_Guard.EnemyCount == 0) yield return new WaitForSeconds(0.5f);

        while(fightScene_Guard.EnemyCount > 0)
        {
            yield return null;
        }
    }
}
