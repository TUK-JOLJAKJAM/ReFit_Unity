using System.Collections;
using UnityEngine;

public class FightScene_Logic : MonoBehaviour, IReFitGyro
{
    public GameObject[] InGameUIs;

    public enum FightState
    {
        Attack,
        Guard,
        Win,
        Lose
    }

    [SerializeField] public FightState GameState;
    Coroutine _fightCoroutine = null;

    public FightScene_Skill SkillUI;
    Skill CurrentSkill;

    public enum Skill
    {
        Arm,
        Shoulder,
        Waist
    }

    public Animator PlayerAnimator;

    //---------------IReFitGyro------------------
    public void GyroInputUp()
    {
        switch (GameState)
        {
            case FightState.Attack:
                PlayerAct();
                break;
            case FightState.Guard:
                break;
            case FightState.Win:
                break;
            case FightState.Lose:
                break;
        }
    }
    public void GyroInputDown()
    {
        switch (GameState)
        {
            case FightState.Attack:
                break;
            case FightState.Guard:
                break;
            case FightState.Win:
                break;
            case FightState.Lose:
                break;
        }
    }
    public void GyroInputLeft()
    {
        switch (GameState)
        {
            case FightState.Attack:
                ChangeSkillLeft();
                break;
            case FightState.Guard:
                break;
            case FightState.Win:
                break;
            case FightState.Lose:
                break;
        }
    }
    public void GyroInputRight()
    {

        switch (GameState)
        {
            case FightState.Attack:
                ChangeSkillRight();
                break;
            case FightState.Guard:
                break;
            case FightState.Win:
                break;
            case FightState.Lose:
                break;
        }
    }

    //-------------------------------------------
    void Awake()
    {
        ResetUI();
        GameState = FightState.Attack;
        SetFightState(FightState.Attack);

        CurrentSkill = Skill.Shoulder;
        SkillUI.SetSkillUI(CurrentSkill);
    }
    
    void SetFightState(FightState newState)
    {
        GameState = newState;
        
        if (_fightCoroutine != null)
        {
            StopCoroutine(_fightCoroutine);
        }

        switch (GameState)
        {
            case FightState.Attack:
                _fightCoroutine = StartCoroutine(AttackCoroutine());
                break;
            case FightState.Guard:
                _fightCoroutine = StartCoroutine(GuardCoroutine());
                break;
            case FightState.Win:
                _fightCoroutine = StartCoroutine(WinCoroutine());
                break;
            case FightState.Lose:
                _fightCoroutine = StartCoroutine(LoseCoroutine());
                break;
        }
    }

    IEnumerator AttackCoroutine()
    {
        ReFitLogger.Info("AttackCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUIs[0].SetActive(true);
        while (true)
        {
            //적 체력 검사 : 적 체력 0 이하면 Win 상태로 전환
            yield return null;
        }
    }

    IEnumerator GuardCoroutine()
    {
        ReFitLogger.Info("GuardCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUIs[1].SetActive(true);
        while (true)
        {
            //플레이어 체력 검사 : 체력 0 이하면 Win 상태로 전환
            yield return null;
        }
    }
    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(2f);
    }
    IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(2f);
    }

    void ResetUI()
    {
        foreach (var ui in InGameUIs)
        {
            ui.SetActive(false);
        }
    }

    //----------InFight상태-------------
    void ChangeSkillLeft() 
    {
        switch (CurrentSkill)
        {
            case Skill.Arm:
                CurrentSkill = Skill.Waist;
                break;
            case Skill.Shoulder:
                CurrentSkill = Skill.Arm;
                break;
            case Skill.Waist:
                CurrentSkill = Skill.Shoulder;
                break;
        }

        SkillUI.SetSkillUI(CurrentSkill);
    }

    void ChangeSkillRight()
    {
        switch (CurrentSkill)
        {
            case Skill.Arm:
                CurrentSkill = Skill.Shoulder;
                break;
            case Skill.Shoulder:
                CurrentSkill = Skill.Waist;
                break;
            case Skill.Waist:
                CurrentSkill = Skill.Arm;
                break;
        }

        SkillUI.SetSkillUI(CurrentSkill);
    }

    void PlayerAct()
    {
        switch (CurrentSkill)
        {
            //임시로 공격, 피격, 방어 애니메이션을 각각 Arm, Shoulder, Waist에 연결
            //변경 시에는 플레이어 스크립트의 함수 호출.(애니메이터도 플레이어스크립트에 연결)
            //플레이어 스크립트에서 CurrentSkill에 따라 다른 공격 이펙트 출력하게끔
            case Skill.Arm:
                PlayerAnimator.SetTrigger("Attack");
                break;
            case Skill.Shoulder:
                PlayerAnimator.SetTrigger("Hurt");
                break;
            case Skill.Waist:
                PlayerAnimator.SetTrigger("Guard");
                break;
        }
    }
}
