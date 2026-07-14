using UnityEngine;

public class FightScene_Player : MonoBehaviour
{
    //오브젝트 데이터
    public float hp = 100.0f;

    public Animator animator;

    public GameObject AttackEffectRed;
    public GameObject AttackEffectGreen;
    public GameObject AttackEffectBlue;

    public GameObject HurtEffect;
    public void Attack(FightScene_Logic.Skill type)
    {
        animator.SetTrigger("Attack");

        if (AttackEffectRed == null || AttackEffectBlue == null || AttackEffectGreen == null) return;

        switch (type)
        {
            case FightScene_Logic.Skill.Red:
                Destroy(Instantiate(AttackEffectRed, this.transform), 1.0f);
                break;
            case FightScene_Logic.Skill.Blue:
                Destroy(Instantiate(AttackEffectBlue, this.transform), 1.0f);
                break;
            case FightScene_Logic.Skill.Green:
                Destroy(Instantiate(AttackEffectGreen, this.transform), 1.0f);
                break;
        }
    }

    public void Hurt(float damage, float guardPoint)
    {
        if (guardPoint > 5)
        {
            animator.SetTrigger("Guard");
        }
        else
        {
            animator.SetTrigger("Hurt");
        }

        hp -= damage * (guardPoint / 10);
    }
}
