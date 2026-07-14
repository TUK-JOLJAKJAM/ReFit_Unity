using UnityEngine;

public class FightScene_Monster : MonoBehaviour
{
    float defaultHp = 100f;
    public float monsterHP = 0;
    public FightScene_Logic.Skill weakNess;

    public Animator animator;

    public GameObject AttackEffect;

    public GameObject HurtEffectRed;
    public GameObject HurtEffectBlue;
    public GameObject HurtEffectGreen;

    public void SetMonster(int level, FightScene_Logic.Skill type)
    {
        monsterHP = level * 0.3f * defaultHp;//레벨디자인 할 떄 고치기
        weakNess = type;
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        Destroy(Instantiate(AttackEffect, this.transform), 1.0f);
    }

    public void Hurt(float damage, FightScene_Logic.Skill type)
    {
        switch (type)
        {
            case FightScene_Logic.Skill.Red:
                Destroy(Instantiate(HurtEffectRed, this.transform), 1.0f);
                break;
            case FightScene_Logic.Skill.Blue:
                Destroy(Instantiate(HurtEffectBlue, this.transform), 1.0f);
                break;
            case FightScene_Logic.Skill.Green:
                Destroy(Instantiate(HurtEffectGreen, this.transform), 1.0f);
                break;
        }

        monsterHP -= damage;
    }
}
