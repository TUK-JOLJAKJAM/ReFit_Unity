using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FightScene_Logic : MonoBehaviour, IReFitGyro
{
    public GameObject[] InGameUI;

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
                GameManager.instance.ChangeScene(GameManager.GameScene.AdventureScene);
                break;
            case FightState.Lose:
                GameManager.instance.ChangeScene(GameManager.GameScene.TitleScene);
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
                playTimeRoutine = StartCoroutine(playTimeCoroutine());
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
                StopCoroutine(playTimeRoutine);
                break;
            case FightState.Lose:
                _fightCoroutine = StartCoroutine(WinCoroutine());
                //Lose 개발한 뒤에 위에거 지우기
                //_fightCoroutine = StartCoroutine(LoseCoroutine());
                StopCoroutine(playTimeRoutine);
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

        InGameUI[0].SetActive(true);
        yield return null;
    }

    IEnumerator AttackCoroutine()
    {
        ReFitLogger.Info("AttackCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUI[1].SetActive(true);
        yield return null;

        float damage = 2;
        int attackCount = 0;

        bool isMovingUp = false;


        gaugeController.SetFightUI(CurrentSkill);

        while (attackCount < 5)
        {
            Vector2 GyroData;
            if (GameManager.instance.MyTestHandler.isTestMode)
                GyroData = GameManager.instance.GyroHud.TestGyro;
            else 
                //Y,X축
                GyroData = new Vector2(GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroY(), -GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroX());

            // 1. 공격 대기(0.5초 이내) 창이 열려있고, 자이로 조건이 충족되면 즉시 취소 후 리셋
            if (gaugeController.uiState == FightScene_Attack.UIState.Charged && GyroData.y <= -0.2f)
            {
                gaugeController.CancelAndFastReset();
                gaugeController.uiState = FightScene_Attack.UIState.NoCharged;
                Player.Attack(CurrentSkill);
                float TotalDamage = damage * (int)gaugeController.lastAttackGrade;
                Enemy.Hurt(TotalDamage, CurrentSkill);

                if (Enemy.monsterHP <= 0)
                {
                    SetFightState(FightState.Win);
                    yield break; // 몬스터가 죽었다면 코루틴 즉시 완전 종료
                }

                attackCount++;
                isMovingUp = false;

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
    public FightScene_PlayerUI playerUI;
    IEnumerator GuardCoroutine()
    {
        ReFitLogger.Info("GuardCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUI[2].SetActive(true);
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

        playerUI.UpdateShieldBar(0);
        SetFightState(FightState.SkillSelect);
    }

    IEnumerator WinCoroutine()
    {
        ReFitLogger.Info("WinCoroutine 시작");

        ResetUI();
        yield return null;

        InGameUI[3].SetActive(true);
        InGameUI[3]?.GetComponent<Animator>().SetTrigger("Win");
        InGameUI[3]?.GetComponent<FightScene_Win>().SetWinUI(gaugeController.attackData, playTime, Player.hp);
        yield return null;
    }
    IEnumerator LoseCoroutine()
    {

        yield return new WaitForSeconds(2f);
    }

    void ResetUI()
    {
        foreach (var ui in InGameUI)
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

    // --- 플레이타임 ---
    public float playTime;
    Coroutine playTimeRoutine;

    IEnumerator playTimeCoroutine()
    {
        playTime = 0;

        while (true)
        {
            playTime += Time.deltaTime;
            yield return null;
        }
    }
}
