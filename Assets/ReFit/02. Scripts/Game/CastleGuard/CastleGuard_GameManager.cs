using System.Collections;
using UnityEngine;

public class CastleGuard_GameManager : MonoBehaviour
{
    [Header("********** Read Only **********")]
    [SerializeField] public GameState _currentState;
    [SerializeField] public Phase _currentPhase;

    [SerializeField] private int point = 0;
    [SerializeField] private int life = 10;
    [SerializeField] private float playTime = 0f;

    //! ==================== Hidden Datas ====================
    public enum GameState
    { Start, Playing, GameOver, End }
    public enum Phase
    { Phase1, Phase2 }

    Coroutine _currentCoroutine;

    private void Start()
    {
        _currentState = GameState.Start;
        _currentPhase = Phase.Phase1;
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

                playTime += Time.deltaTime;

                if (life <= 0)
                {
                    _currentState = GameState.GameOver;
                }

                if (point >= 20 && _currentPhase == Phase.Phase1)
                {
                    _currentPhase = Phase.Phase2;
                }
                
                break;

            case GameState.GameOver:
                Debug.Log("게임 오버 상태");
                _currentState = GameState.End;

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
     
    public void AddPoint()
    {
        point++;
    }

    public void DecreaseLife()
    {
        life--;
    }

    //월드핸들러에서 게임 리셋할 때 사용
    public void ResetGame()
    {
        point = 0;
        life = 10;
        playTime = 0f;
        _currentState = GameState.Start;
        _currentPhase = Phase.Phase1;
    }
}