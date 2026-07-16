using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class FightScene_Logic : MonoBehaviour, IReFitGyro
{
    public GameObject[] InGameUIs;

    public enum FightState
    {
        Loading,
        SkillSelect,
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
        Red,
        Green,
        Blue
    }

    public Animator PlayerAnimator;

    public FightScene_Player Player;
    public FightScene_Monster Enemy;

    public FightScene_Attack gaugeController;

    //---------------IReFitGyro------------------
    public void GyroInputUp()
    {
        switch (GameState)
        {
            case FightState.SkillSelect:
                SetFightState(FightState.Attack);
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
            case FightState.SkillSelect:
                ChangeSkillLeft();
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
            case FightState.SkillSelect:
                ChangeSkillRight();
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
        GameState = FightState.Loading;
        SetFightState(FightState.Loading);

        CurrentSkill = Skill.Green;
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
            case FightState.Loading:
                _fightCoroutine = StartCoroutine(LoadingCoroutine());
                break;
            case FightState.SkillSelect:
                _fightCoroutine = StartCoroutine(SkillSelectCoroutine());
                break;
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
    
    IEnumerator LoadingCoroutine()
    {
        ReFitLogger.Info("LoadingState 시작");
        AdventureManager adM = GameManager.instance.MyAdventureManager;
        yield return null;

        Enemy.SetMonster(adM.currentStageLevel, adM.RandomNode[adM.currentStageLevel - 1].attackType);
        SetFightState(FightState.SkillSelect);
    }

    IEnumerator SkillSelectCoroutine()
    {
        ReFitLogger.Info("SkillSelectCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUIs[0].SetActive(true);
        yield return null;
    }

    IEnumerator AttackCoroutine()
    {
        ReFitLogger.Info("AttackCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUIs[1].SetActive(true);
        yield return null;

        float damage = 5;
        int attackCount = 0;

        bool isMovingUp = false;

        
        gaugeController.SetFightUI(CurrentSkill);

        while (attackCount < 5)
        {
            //UI상태에 따라서 구분 -> 노차지, 차지 후 데미지 계산, Player.Attack(CurrentSkill), Enemy.Hurt(damage, CurrentSkill)
            Vector2 GyroData = GameManager.instance.GyroHud.TestGyro;

            // 1. 공격 대기(0.5초 이내) 창이 열려있고, 자이로 조건이 충족되면 즉시 취소 후 리셋
            if (gaugeController.uiState == FightScene_Attack.UIState.Charged && GyroData.y <= -0.2f)
            {
                gaugeController.CancelAndFastReset();
                gaugeController.uiState = FightScene_Attack.UIState.NoCharged;
                Player.Attack(CurrentSkill);
                Enemy.Hurt(damage, CurrentSkill); 
                
                if (Enemy.monsterHP <= 0)
                {
                    SetFightState(FightState.Win);
                    yield break; // 몬스터가 죽었다면 코루틴 즉시 완전 종료
                }

                attackCount++;
                isMovingUp = false;

                /*while (gaugeController.Gauge.rectTransform.sizeDelta.x > gaugeController.GaugeSmallSize.x + 1f)
                {
                    yield return null;
                }*/

                continue;
            }

            // 2. 대기 시간 중이 아닐 때의 일반적인 게이지 증감 제어
            if (gaugeController.uiState == FightScene_Attack.UIState.NoCharged)
            {
                if (GyroData.y >= 0.8f)
                {
                    if (!isMovingUp)
                    {
                        isMovingUp = true;
                        gaugeController.StartGaugeUp();
                    }
                }
                else
                {
                    if (isMovingUp)
                    {
                        isMovingUp = false;
                        gaugeController.StartGaugeDown();
                    }
                }
            }

            yield return null;
        }

        yield return null;
        SetFightState(FightState.Guard);
    }

    public FightScene_Guard fightScene_Guard;
    public FightScene_Shield fightScene_Shield;
    IEnumerator GuardCoroutine()
    {
        ReFitLogger.Info("GuardCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUIs[2].SetActive(true);
        yield return null;

        float damage = 50;
        int guardPoint = 0;

        fightScene_Guard.SetMonsterAttack();
        yield return null;

        yield return fightScene_Shield.guardRoutine();
        guardPoint = fightScene_Shield.guardPoint;

        Enemy.Attack();
        Player.Hurt(damage, guardPoint);
        yield return null;

        SetFightState(FightState.SkillSelect);
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
            case Skill.Red:
                CurrentSkill = Skill.Blue;
                break;
            case Skill.Green:
                CurrentSkill = Skill.Red;
                break;
            case Skill.Blue:
                CurrentSkill = Skill.Green;
                break;
        }

        SkillUI.SetSkillUI(CurrentSkill);
    }

    void ChangeSkillRight()
    {
        switch (CurrentSkill)
        {
            case Skill.Red:
                CurrentSkill = Skill.Green;
                break;
            case Skill.Green:
                CurrentSkill = Skill.Blue;
                break;
            case Skill.Blue:
                CurrentSkill = Skill.Red;
                break;
        }

        SkillUI.SetSkillUI(CurrentSkill);
    }
}
