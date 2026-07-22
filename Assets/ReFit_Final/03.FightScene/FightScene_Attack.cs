using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightScene_Attack : MonoBehaviour
{
    private const float SensorSampleIntervalSec = 1f / 30f;
    private const int MaxSamplesPerAction = 1800; // 비정상 장시간 입력 시에도 동작당 최대 60초

    public GameObject SkillRed;
    public GameObject SkillGreen;
    public GameObject SkillBlue;

    public Color RedColor;
    public Color GreenColor;
    public Color BlueColor;
    Color DefaultColor;
    public Color GoldColor;

    public Image Gauge;
    public Vector2 GaugeFullSize;
    public Vector2 GaugeSmallSize;

    public UIState uiState;

    [System.Serializable]
    public enum UIState
    {
        NoCharged,
        Charged
    }

    public AudioSource SFX;
    public AudioClip MissSound;
    public AudioClip ChargeSound;

    // 내부 제어용 코루틴 참조 변수
    private Coroutine gaugeCoroutine;
    private Coroutine attackWaitCoroutine;
    private FightScene_Logic.Skill activeSkill;
    private bool isCapturingAction;
    private long actionStartedAtMs;
    private float nextSampleAt;
    private int nextActionId = 1;
    private readonly List<SensorSampleRecord> currentSamples = new List<SensorSampleRecord>();

    public GameObject[] Acts;
    public void SetFightUI(FightScene_Logic.Skill type)
    {
        activeSkill = type;
        uiState = UIState.NoCharged;

        GaugeFullSize = Gauge.rectTransform.sizeDelta;
        GaugeSmallSize = new Vector2(GaugeFullSize.x * 0.2f, GaugeFullSize.y);
        Gauge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GaugeFullSize.x * 0.5f);

        SkillRed.SetActive(false);
        SkillGreen.SetActive(false);
        SkillBlue.SetActive(false);

        Acts[0].SetActive(false);
        Acts[1].SetActive(false);
        Acts[2].SetActive(false);

        switch (type)
        {
            case FightScene_Logic.Skill.Red:
                Gauge.color = RedColor;
                DefaultColor = RedColor;
                SkillRed.SetActive(true);
                Acts[0].SetActive(true);
                break;
            case FightScene_Logic.Skill.Green:
                Gauge.color = GreenColor;
                DefaultColor = GreenColor;
                SkillGreen.SetActive(true);
                Acts[1].SetActive(true);
                break;
            case FightScene_Logic.Skill.Blue:
                Gauge.color = BlueColor;
                DefaultColor = BlueColor;
                SkillBlue.SetActive(true);
                Acts[2].SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 게이지를  천천히 증가시킵니다. (중복 호출 방지 내장)
    /// </summary>
    public void StartGaugeUp()
    {
        if (uiState == UIState.NoCharged)
        {
            Gauge.color = DefaultColor;
            if (gaugeCoroutine != null) StopCoroutine(gaugeCoroutine);
            gaugeCoroutine = StartCoroutine(Co_ChangeGaugeWidth(GaugeFullSize, 300f));
        }
    }

    /// <summary>
    /// 게이지를 천천히 감소시킵니다.
    /// </summary>
    public void StartGaugeDown()
    {
        if (uiState == UIState.NoCharged && Gauge.rectTransform.sizeDelta.x > GaugeSmallSize.x)
        {
            if (gaugeCoroutine != null) StopCoroutine(gaugeCoroutine);
            gaugeCoroutine = StartCoroutine(Co_ChangeGaugeWidth(GaugeSmallSize, 300f));
        }
    }

    /// <summary>
    /// 공격 대기 상태를 강제로 취소하고, 게이지를 매우 빠르게 최소치로 줄입니다.
    /// </summary>
    float attackTime = 0;
    public AttackGrade lastAttackGrade = AttackGrade.Miss;
    public int[] attackData = new int[5];
    public List<GameActionRecord> TotalAttackData = new List<GameActionRecord>();

    [System.Serializable]
    public enum AttackGrade
    {
        Miss,
        Bad,
        Normal,
        Good,
        Perfect
    }
    public void CancelAndFastReset()
    {
        if (attackWaitCoroutine != null) StopCoroutine(attackWaitCoroutine);

        uiState = UIState.NoCharged;

        if (gaugeCoroutine != null) StopCoroutine(gaugeCoroutine);
        gaugeCoroutine = StartCoroutine(Co_ChangeGaugeWidth(GaugeSmallSize, 1000f, DefaultColor)); // 빠르게 감소

        if(attackTimeCoroutine != null) StopCoroutine(attackTimeCoroutine);
        lastAttackGrade = attackTime <= waitTime / 4 ? AttackGrade.Perfect :
                attackTime <= waitTime / 2 ? AttackGrade.Good :
                attackTime <= waitTime * 3 / 4 ? AttackGrade.Normal :
                AttackGrade.Bad;
        attackText();

        attackData[(int)lastAttackGrade]++;
        CompleteActionCapture(lastAttackGrade, attackTime);
    }

    // ==========================================
    // [실제 동작 코루틴]
    // ==========================================

    private IEnumerator Co_ChangeGaugeWidth(Vector2 targetsize, float speed, Color? endColor = null)
    {
        Vector2 size = Gauge.rectTransform.sizeDelta;

        while (!Mathf.Approximately(size.x, targetsize.x))
        {
            size.x = Mathf.MoveTowards(size.x, targetsize.x, speed * Time.deltaTime);
            Gauge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            yield return null;
        }

        // 목표치에 도달했을 때
        if (Mathf.Approximately(size.x, GaugeFullSize.x))
        {
            Gauge.color = GoldColor;
            uiState = UIState.Charged;

            SFX.PlayOneShot(ChargeSound);

            // 게이지가 다 차면 자동으로 공격 대기 코루틴 시작
            attackWaitCoroutine = StartCoroutine(Co_AttackWait());
        }

        // 빠른 감소 등이 끝나고 최종 색상을 적용해야 할 때
        if (Mathf.Approximately(size.x, targetsize.x) && endColor.HasValue)
        {
            Gauge.color = endColor.Value;
        }

        gaugeCoroutine = null;
    }

    public float waitTime = 1.0f;
    private IEnumerator Co_AttackWait()
    {
        attackTimeCoroutine = StartCoroutine(AttackTime());
        // 대기 (이 사이에 외부에서 CancelAndFastReset이 호출되면 이 코루틴은 완전히 종료됨)
        yield return new WaitForSeconds(waitTime);

        // --------------------------------------------------
        // [만료 처리] 0.5초 동안 -0.2 밑으로 안 내려와서 타이머가 끝난 경우
        // --------------------------------------------------
        attackWaitCoroutine = null;

        // 상태를 충전 안 됨(NoCharged) 상태로 변경
        uiState = UIState.NoCharged;

        // 즉시 색상을 기본 색상으로 변경
        Gauge.color = DefaultColor;

        // 기존에 돌고 있을지 모를 게이지 변화 코루틴을 멈추고, 
        // 최소치(5f)까지 "빠른 속도(2000f)"로 감소시킵니다.
        if (gaugeCoroutine != null) StopCoroutine(gaugeCoroutine);
        gaugeCoroutine = StartCoroutine(Co_ChangeGaugeWidth(GaugeSmallSize, 2000f));

        if(attackTimeCoroutine != null) StopCoroutine(attackTimeCoroutine);
        lastAttackGrade = AttackGrade.Miss;
        attackText();
        attackData[(int)lastAttackGrade]++;

        SFX.PlayOneShot(MissSound);

        CompleteActionCapture(lastAttackGrade, attackTime);
    }

    Coroutine attackTimeCoroutine;
    IEnumerator AttackTime()
    {
        attackTime = 0;

        while (true)
        {
            attackTime += Time.deltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        if (!isCapturingAction || Time.unscaledTime < nextSampleAt) return;
        CaptureSensorSample();
        nextSampleAt = Time.unscaledTime + SensorSampleIntervalSec;
    }

    /// <summary>
    /// 게임 판정에는 관여하지 않고 현재 공격 동작의 센서 시퀀스만 수집합니다.
    /// 여러 번 호출되어도 이미 수집 중이면 무시합니다.
    /// </summary>
    public void BeginActionCapture(FightScene_Logic.Skill skill)
    {
        if (isCapturingAction) return;
        activeSkill = skill;
        isCapturingAction = true;
        actionStartedAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        nextSampleAt = Time.unscaledTime;
        currentSamples.Clear();
        CaptureSensorSample();
    }

    private void CompleteActionCapture(AttackGrade grade, float reactionTimeSec)
    {
        if (!isCapturingAction) BeginActionCapture(activeSkill);
        CaptureSensorSample();
        long endedAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        TotalAttackData.Add(new GameActionRecord
        {
            actionId = (nextActionId++).ToString(),
            actionType = "ATTACK",
            exerciseCode = ExerciseCode(activeSkill),
            direction = "OUT_AND_RETURN",
            startedAtMs = actionStartedAtMs,
            endedAtMs = endedAtMs,
            durationMs = Math.Max(endedAtMs - actionStartedAtMs, 0),
            success = grade != AttackGrade.Miss,
            attackGrade = grade.ToString().ToUpperInvariant(),
            reactionTimeMs = Mathf.Max(reactionTimeSec * 1000f, 0f),
            samples = new List<SensorSampleRecord>(currentSamples)
        });

        isCapturingAction = false;
        currentSamples.Clear();
    }

    private void CaptureSensorSample()
    {
        if (currentSamples.Count >= MaxSamplesPerAction) return;

        Quaternion q;
        if (GameManager.instance.MyTestHandler.isTestMode)
        {
            Vector2 test = GameManager.instance.GyroHud.TestGyro;
            q = Quaternion.Euler(test.y * 90f, test.x * 90f, 0f);
        }
        else
        {
            q = GameManager.instance.MyGyroManager.GetOffsetGyro();
        }

        currentSamples.Add(new SensorSampleRecord
        {
            timestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            qx = q.x,
            qy = q.y,
            qz = q.z,
            qw = q.w
        });
    }

    private static string ExerciseCode(FightScene_Logic.Skill skill)
    {
        switch (skill)
        {
            case FightScene_Logic.Skill.Red:
                return "BICEPS_CURL";
            case FightScene_Logic.Skill.Green:
                return "SHOULDER_FLEXION";
            case FightScene_Logic.Skill.Blue:
                return "WAIST_ROTATION";
            default:
                return "UNKNOWN";
        }
    }

    //판정 텍스트
    public GameObject[] attackTexts;

    public void attackText()
    {
        Destroy(Instantiate(attackTexts[(int)lastAttackGrade], this.transform), 0.7f);
    }
}
