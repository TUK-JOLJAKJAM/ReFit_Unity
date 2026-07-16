using System.Collections;
using UnityEngine;

public class FightScene_Guard : MonoBehaviour
{
    public GameObject[] MonsterAttackUI;
    Coroutine monsterAttackCoroutine;
    public int EnemyCount = 0;

    public RectTransform GuardUIRect;

    public void SetMonsterAttack()
    {
        monsterAttackCoroutine = StartCoroutine(monsterAttack());
    }

    IEnumerator monsterAttack()
    {
        Vector2 guardUI = GuardUIRect.sizeDelta;
        int spawnCount = 0;
        int spawnMax = 5;
        EnemyCount = spawnMax;

        yield return null;

        while (spawnCount < spawnMax)
        {
            float spawnDelay = Random.Range(1.0f, 2.0f);
            int attackType = Random.Range(0, MonsterAttackUI.Length);
            float attackPosX = Random.Range(-guardUI.x / 2, guardUI.x / 2);
            yield return null;

            GameObject newAttack = Instantiate(MonsterAttackUI[attackType], GuardUIRect);
            newAttack.GetComponent<RectTransform>().anchoredPosition = new Vector2(attackPosX, guardUI.y/2);
            spawnCount++;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReFitLogger.Info("적공격충돌");
        Destroy(collision.gameObject);
        EnemyCount--;
    }
}
