using System.Collections;
using UnityEngine;

public class StartArea_Wood : MonoBehaviour
{
    // ==================== Inspector UI ====================
    [Header("--- Object ---")]
    [SerializeField] public PointManager_Wood PointManager;
    [SerializeField] public ReFit_G01_Wood GameUI;

    [Header("--- 제어 수치 ---")]
    [SerializeField] private float _successTime = 3f;

    [Header("*** Read Only***")]
    [SerializeField] private float _currentTime = 0f;

    // ==================== Hidden Datas ====================
    private bool _timerStart = false;

    // ==================== Functions =======================
    private void Start()
    {
        _currentTime = 0f;
        _timerStart = false;
        PointManager.SetSwinging(false);

    }

    //이거 사실 포인트매니저에 있어야됨. _successTime이랑 _currentTime도 포인트매니저에 있어야됨. 근데 일단은 여기다가 둠 시간없음
    public void ResetTimer()
    {
        _currentTime = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PointManager.isSwinging())
        {
            _timerStart = true;
            StartTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _timerStart = false;

        if (PointManager.isSwinging())
        {
            PointManager.SetValidSwing(true);
        }
        else
        {
            _currentTime = 0f;
            GameUI.SetSlider(0f);
            PointManager.SetValidSwing(false);
        }
    }

    private void StartTimer()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        Debug.Log("Timer Start");

        while (_timerStart)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime <= _successTime)
            {
                GameUI.SetSlider(_currentTime / _successTime);
            }
            else
            {
                GameUI.SetSlider(1f);
                PointManager.SetSwinging(true);
            }

            yield return null;
        }

        Debug.Log("Timer Stop");
    }
}
