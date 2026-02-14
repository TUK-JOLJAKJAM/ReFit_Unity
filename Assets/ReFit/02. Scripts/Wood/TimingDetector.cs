using UnityEngine;

public class TimingDetector : MonoBehaviour
{
    private float enterTime;
    private bool isDetecting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AxeRotationController axeController = FindFirstObjectByType<AxeRotationController>();
            if (axeController != null && axeController.IsSwingingForward)
            {
                enterTime = Time.time;
                isDetecting = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isDetecting)
        {
            float duration = Time.time - enterTime;
            isDetecting = false;
            
            ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.EvaluateTiming(duration);
            }
        }
    }
}
