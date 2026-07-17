using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    int[] SkillSelectCount = { 0, 0, 0 };

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

        StartTime = 0;
        EndTime = 0;
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

        StartTime = GetTime();
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

        int blueAttackDir = 1;

        gaugeController.SetFightUI(CurrentSkill);
        SkillSelectCount[(int)CurrentSkill]++;

        while (attackCount < 5)
        {
            //수정 전 코드(혹시모르니 남겨놓음 일단)
            /*Vector2 GyroData;

            if (GameManager.instance.MyTestHandler.isTestMode)
                GyroData = GameManager.instance.GyroHud.TestGyro;
            else 
                //Y,X축
                GyroData = new Vector2(GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroY(),
                    -GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroX());*/

            float GyroValue = 0;
            if (GameManager.instance.MyTestHandler.isTestMode) GyroValue = GameManager.instance.GyroHud.TestGyro.y;
            else
            {
                switch (CurrentSkill)
                {
                    //[수정필요]각 관절 별 보정값 가져오는 함수로 수정해야함.
                    case Skill.Red:
                        GyroValue = GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroY();
                        break;
                    case Skill.Green:
                        GyroValue = GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroY();
                        break;
                    case Skill.Blue://[수정필요]오른쪽이 시작임을 표시하는 UI가 없음. 방향UI추가해야됨.
                        GyroValue = GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroX() * blueAttackDir;
                        break;
                }
            }

            // 기존 판정에는 관여하지 않고, 움직임이 시작된 순간부터 센서 시퀀스만 수집합니다.
            if (gaugeController.uiState == FightScene_Attack.UIState.NoCharged && Mathf.Abs(GyroValue) >= 0.1f)
            {
                gaugeController.BeginActionCapture(CurrentSkill);
            }

            // 1. 공격 대기(0.5초 이내) 창이 열려있고, 자이로 조건이 충족되면 즉시 취소 후 리셋
            if (gaugeController.uiState == FightScene_Attack.UIState.Charged && GyroValue <= 0.1f)
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
                blueAttackDir *= -1;
                continue;
            }

            // 2. 대기 시간 중이 아닐 때의 일반적인 게이지 증감 제어
            if (gaugeController.uiState == FightScene_Attack.UIState.NoCharged)
            {
                if (GyroValue >= 0.8f)
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

        EndTime = GetTime();
        yield return null;

        InGameUI[3].SetActive(true);
        InGameUI[3]?.GetComponent<Animator>().SetTrigger("Win");
        InGameUI[3]?.GetComponent<FightScene_Win>().SetWinUI(gaugeController.attackData, playTime, Player.hp);
        yield return null;

        SaveData();
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

    void SaveData()
    {
        // 값 계산
        int maxSelected = Mathf.Max(SkillSelectCount);
        string primaryPart = maxSelected == SkillSelectCount[0] ? "BICEPS_BRACHII" :
            maxSelected == SkillSelectCount[1] ? "SHOULDER" :
            "WAIST";
        int score = gaugeController.attackData[1] + gaugeController.attackData[2] * 2 + gaugeController.attackData[3] * 3 + gaugeController.attackData[4] * 4; //레전드 대충 점수계산.
        int actionCount = gaugeController.attackData.Sum();
        int successCount = gaugeController.attackData.Sum() - gaugeController.attackData[0];
        int failCount = gaugeController.attackData[0];

        // 1. 내부 딕셔너리 및 리스트 정의
        var summary = new Dictionary<string, object> {
            { "stageLevel", GameManager.instance.MyAdventureManager.currentStageLevel },
            { "protocolVersion", "2.0" },
            { "sensorSampleRateHz", 30 },
            { "testMode", GameManager.instance.MyTestHandler.isTestMode }
        };

        var bodyParts = new List<BodyPartSummary>
        {      
            new BodyPartSummary
            {
                bodyPart = primaryPart,
                side = "BOTH",
                // 측정하지 않은 통증과 ROM은 0이나 고정값으로 만들지 않습니다.
                metrics = new Dictionary<string, object>()
            }  
        
        };
        var dataList = gaugeController.TotalAttackData;

        // 2. 함수 호출 한 번으로 전송 완료
        GameManager.instance.MyDataManager.SaveGameHistory(
            "Adventure",
            "Fight",
            GameManager.instance.GameVersion,
            primaryPart,
            SystemInfo.deviceUniqueIdentifier,
            StartTime, // startedAtMs
            EndTime, // endedAtMs
            score,
            actionCount,
            successCount,
            failCount,
            summary,
            bodyParts,
            dataList,
            (success, result) => {
                if (success) Debug.Log("저장 완료!");
            }
        );
    }

    long StartTime;
    long EndTime;
    long GetTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
