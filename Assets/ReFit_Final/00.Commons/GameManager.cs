using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Sub Managers")]
    [SerializeField] public Z_UIManager UIManager;
    [SerializeField] public Z_GyroManager GyroManager;
    [SerializeField] public ProfileManager ProfileManager;
    [SerializeField] public DataManager DataManager;
    [SerializeField] public TestHandler TestHandler;

    [Header("Important UI")]
    [SerializeField] public GyroHud gyroHud;

    public enum GameState
    {
        Loading, // 매니저 초기화, 필요한 리소스 로드
        Title,   // 타이틀 화면, 메뉴 등 (필요 시 추가)
        InGame,  // 게임 플레이 중
        GameOver // 게임 종료, 결과 화면 등
    }

    private GameState _gameState;
    public GameState CurrentState => _gameState; // 외부에서 현재 상태를 읽을 수만 있게 프로퍼티 제공

    private Coroutine _stateCoroutine = null;

    private void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ChangeState(GameState.Loading);
    }

    private void Update()
    {
        UIManager.UpdateReFitManager();
        GyroManager.UpdateReFitManager();
        ProfileManager.UpdateReFitManager();
        DataManager.UpdateReFitManager();
    }

    public void ChangeState(GameState newState)
    {
        _gameState = newState;

        if (_stateCoroutine != null)
        {
            StopCoroutine(_stateCoroutine);
        }

        switch (_gameState)
        {
            case GameState.Loading:
                _stateCoroutine = StartCoroutine(LoadingRoutine());
                break;
            case GameState.Title:
                _stateCoroutine = StartCoroutine(TitleRoutine());
                break;
            case GameState.InGame:
                _stateCoroutine = StartCoroutine(InGameRoutine());
                break;
            case GameState.GameOver:
                _stateCoroutine = StartCoroutine(GameOverRoutine());
                break;
        }
    }

    // 1. 로딩 상태 루틴
    private IEnumerator LoadingRoutine()
    {
        ReFItLogger.Info("로딩 및 초기화 시작...");

        UIManager.ResetReFitManager();
        GyroManager.ResetReFitManager();
        ProfileManager.ResetReFitManager();
        DataManager.ResetReFitManager();

        UIManager.CloseAllUI();

        UIManager.OpenUI(Z_UIManager.UIType.Loading);

        yield return new WaitForSeconds(2f); // 로딩 시뮬레이션 (혹은 실제 비동기 로드)

        ReFItLogger.Info("로딩 완료 -> 타이틀 화면으로 전환");

        ChangeState(GameState.Title);
    }
    
    //2. 타이틀 화면 루틴
    private IEnumerator TitleRoutine()
    {
        ReFItLogger.Info("타이틀 화면 진입: 메뉴 대기");
        UIManager.CloseAllUI();

        //필요 UI 열기        
        var titleMenu = UIManager.OpenUI(Z_UIManager.UIType.TitleMenu);
        UIManager.OpenUI(Z_UIManager.UIType.Gyro);

        //자이로 입력 관리
        var gyroUI = UIManager.GetGyroUI(titleMenu);
        SetGyroInput(gyroUI, 2.0f);
        ReFItLogger.Info("자이로 입력 전달, triggerTime: 2.0f");


        yield return null;
    }

    // 3. 인게임 상태 루틴
    private IEnumerator InGameRoutine()
    {
        ReFItLogger.Info("인게임 진입: 게임 루프 시작");
        UIManager.CloseAllUI();

        //필요 UI 열기

        //자이로 입력 대상 전달

        while (_gameState == GameState.InGame)
        {
            //자이로 센서 받아서 입력처리 내용 추가
            yield return null;
        }
    }

    // 4. 게임 오버 상태 루틴
    private IEnumerator GameOverRoutine()
    {
        Debug.Log("게임 오버 진입");
        // 결과창 UI 띄우기, 데이터 송신 내용 추가
        yield return null;
    }

    // 자이로 센서 입력 대상 전달
    public void SetGyroInput(IReFitGyro gyroInput, float triggerTime)
    {
        gyroHud.gyroInput = gyroInput;
        gyroHud.inputTriggerTime = triggerTime;
    }
}