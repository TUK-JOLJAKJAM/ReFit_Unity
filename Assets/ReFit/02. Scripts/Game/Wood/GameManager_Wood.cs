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
    [SerializeField] public PointManager_Wood PointManager;
    [SerializeField] public WoodPoint WoodPoint;

    [Space(30)]

    [Header("********** Read Only **********")]
    [SerializeField] private GameState _currentState;

    //! ==================== Hidden Datas ====================
    enum GameState
    { Start, Playing, GameOver, End }

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
                //if (_currentCoroutine == null) _currentCoroutine = StartCoroutine(DeviceManager.RotationAxe());
                if (PointManager.GetPoint() == 5)
                {
                    _currentState = GameState.GameOver;
                }
                break;
            case GameState.GameOver:
                Debug.Log("게임 오버 상태");
                _currentState = GameState.End;

                WoodPoint.SaveData();
                UIManager.Instance.ButtonDown_MenuSelect(UIManager.MenuState.GameResult);
                break;
            case GameState.End:
                //게임오버 후 아무 동작 안하는 상태
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
