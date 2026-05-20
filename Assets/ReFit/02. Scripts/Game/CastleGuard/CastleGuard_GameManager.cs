using System.Collections;
using UnityEngine;

public class CastleGuard_GameManager : MonoBehaviour
{
    public Transform[] lifeTriggers;
    public ReFit_G03_CastleGuard mainGameUI;

    [Header("********** Read Only **********")]
    [SerializeField] public GameState _currentState;
    [SerializeField] public Phase _currentPhase;

    [SerializeField] private int point = 0;
    [SerializeField] private int life = 5;
    [SerializeField] private float playTime = 0f;

    //! ==================== Hidden Datas ====================
    public enum GameState
    { Start, Playing, GameOver, End }
    public enum Phase
    { Phase1, Phase2 }

    Coroutine _currentCoroutine;

    private void Start()
    {
        ResetGame();
    }

    private void Update()
    {

         switch (_currentState)
        {
            case GameState.Start:
                Debug.Log("시작단계");

                if(_currentCoroutine == null)_currentCoroutine = StartCoroutine(StartState());

                break;

            case GameState.Playing:

                playTime += Time.deltaTime;


                if (life <= 0)
                {
                    _currentState = GameState.GameOver;
                }

                if (point >= 5 && _currentPhase == Phase.Phase1)
                {
                    Debug.Log("5점획득! Phase 2진입");
                    _currentPhase = Phase.Phase2;
                    ChangePhase();
                }
                
                break;

            case GameState.GameOver:
                Debug.Log("게임종료");
                _currentState = GameState.End;

                UIManager.Instance.ButtonDown_MenuSelect(UIManager.MenuState.GameResult);
                break;

            case GameState.End:
                //게임끝나고 아무것도 없는곳
                ResetGame();
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
        mainGameUI?.UpdatePoint(point);
    }

    public void DecreaseLife()
    {
        Debug.Log("라이프 감소");
        life--;
        mainGameUI?.UpdateLife(life);
    }

    //�����ڵ鷯���� ���� ������ �� ���
    public void ResetGame()
    {
        point = 0;
        life = 10;
        playTime = 0f;
        _currentState = GameState.Start;
        _currentPhase = Phase.Phase1;

        mainGameUI?.UpdatePoint(point);
        mainGameUI?.UpdateLife(life);
        foreach (Transform child in lifeTriggers)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ChangePhase()
    {
        foreach (Transform child in lifeTriggers)
        {
            child.gameObject.SetActive(false);
        }
    }
}