using System.Collections;
using UnityEngine;

public class FightScene_Guard : MonoBehaviour
{
    public GameObject[] MonsterAttackUI;
    Coroutine monsterAttackCoroutine;

    public RectTransform GuardUIRect;

    public void SetMonsterAttack()
    {
        monsterAttackCoroutine = StartCoroutine(monsterAttack());
    }

    IEnumerator monsterAttack()
    {
        Vector2 guardUI = GuardUIRect.sizeDelta;
        while (true)
        {
            int attackType = Random.Range(0, MonsterAttackUI.Length);
            float attackPosX = Random.Range(-guardUI.x / 2, guardUI.x / 2);
            yield return null;

            Destroy(Instantiate(MonsterAttackUI[attackType], GuardUIRect), 4f);
        }
    }
}
