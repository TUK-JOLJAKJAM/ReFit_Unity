using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text slowScoreText;
    [SerializeField] private Text goodScoreText;
    [SerializeField] private Text fastScoreText;

    [Header("Timing Settings")]
    private const float FAST_THRESHOLD = 0.12f;
    private const float GOOD_MIN = 0.12f;
    private const float GOOD_MAX = 0.24f;

    private int slowScore = 0;
    private int goodScore = 0;
    private int fastScore = 0;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void EvaluateTiming(float duration)
    {
        if (duration < FAST_THRESHOLD)
        {
            fastScore++;
        }
        else if (duration >= GOOD_MIN && duration <= GOOD_MAX)
        {
            goodScore++;
        }
        else
        {
            slowScore++;
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (slowScoreText != null)
            slowScoreText.text = $"Slow: {slowScore}";
        
        if (goodScoreText != null)
            goodScoreText.text = $"Good: {goodScore}";
        
        if (fastScoreText != null)
            fastScoreText.text = $"Fast: {fastScore}";
    }

    public void ResetScores()
    {
        slowScore = 0;
        goodScore = 0;
        fastScore = 0;
        UpdateScoreUI();
    }
}
