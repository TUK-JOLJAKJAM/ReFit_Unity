using System.Collections;
using UnityEngine;

public class CastleGuard_GameManager : MonoBehaviour
{
    [Header("********** Read Only **********")]
    [SerializeField] public GameState _currentState;

    

    //! ==================== Hidden Datas ====================
    public enum GameState
    { Start, Playing, GameOver, End }

    private float playTime = 0f;
    private int point = 0;

    Coroutine _currentCoroutine;

    private void Start()
    {
        
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

                if (point == 20)
                {
                    _currentState = GameState.GameOver;
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
}
