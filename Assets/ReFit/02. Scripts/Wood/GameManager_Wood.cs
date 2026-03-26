using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_Wood : MonoBehaviour
{
    //! ==================== Inspector UI =====================
    [Header("-----리소스 -----")]
    [SerializeField] public Canvas GameCanvas;

    [Header("----- 오브젝트 -----")]
    [Header("매니저들")]
    [SerializeField] public DeviceManager_Wood DeviceManager;
    [SerializeField] public PointManager_Wood PointManager;
    [SerializeField] public WoodPoint WoodPoint;
    //WoodPoint는 PointManager로 기능 이관 해야하고, DeviceManager는 Cube에서 동작하는거 DeviceManager로 옮겨야함

    [Space(30)]

    [Header("********** Read Only **********")]
    [SerializeField] private GameState _currentState;

    //! ==================== Hidden Datas ====================
    enum GameState
    { Start, Playing, GameOver }

    Coroutine _currentCoroutine;

    //! ==================== Functions =======================

    private void Awake()
    {
        _currentState = GameState.Start;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case GameState.Start:
                Debug.Log("게임 시작 상태");
                if(_currentCoroutine == null)_currentCoroutine = StartCoroutine(StartState());
                break;
            case GameState.Playing:
                Debug.Log("게임 플레이 상태");
                if (_currentCoroutine == null) _currentCoroutine = StartCoroutine(DeviceManager.RotationAxe());
                if (PointManager.GetPoint() == 5)
                {
                    _currentState = GameState.GameOver;
                    StopCoroutine(_currentCoroutine);
                    _currentCoroutine = null;
                }
                break;
            case GameState.GameOver:
                Debug.Log("게임 오버 상태");
                _currentState = GameState.Start;
                WoodPoint.SaveData();
                UIManager.Instance.ReturnToMainMenu();
                break;
        }
    }

    IEnumerator StartState()
    {
        yield return StartCoroutine(UIManager.Instance.SetGameStartEffect());

        _currentState = GameState.Playing;
        _currentCoroutine = null;
    }

    
    
    

}
