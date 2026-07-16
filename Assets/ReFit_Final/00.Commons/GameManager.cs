using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool isFirstLoad = true;
    public static GameManager instance { get; private set; }

    [Header("Sub Managers")]
    [SerializeField] public Z_UIManager MyUIManager;
    [SerializeField] public Z_GyroManager MyGyroManager;
    [SerializeField] public ProfileManager MyProfileManager;
    [SerializeField] public DataManager MyDataManager;
    [SerializeField] public AdventureManager MyAdventureManager;
    [SerializeField] public TestHandler MyTestHandler;

    [Header("Important UI")]
    [SerializeField] public GyroHud GyroHud;

    [Header("Game Logic")]
    [SerializeField] public IReFitGyro GameLogic;

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

    public enum GameScene
    {
        TitleScene,
        AdventureScene,
        FightScene,
        AxeScene,
        SwimScene
    }

    private GameScene _currentScene;
    public GameScene CurrentScene => _currentScene;

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

        Screen.SetResolution(1920, 1080, true);
    }

    private void Start()
    {
        ChangeState(GameState.Loading);
        _currentScene = GameScene.TitleScene;
    }

    private void Update()
    {
        MyUIManager.UpdateReFitManager();
        MyGyroManager.UpdateReFitManager();
        MyProfileManager.UpdateReFitManager();
        MyDataManager.UpdateReFitManager();
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
        ReFitLogger.Info("로딩 및 초기화 시작...");

        MyUIManager.ResetReFitManager();
        MyGyroManager.ResetReFitManager();
        MyProfileManager.ResetReFitManager();
        MyDataManager.ResetReFitManager();
        MyAdventureManager.ResetReFitManager();

        MyUIManager.CloseAllUI();

        MyUIManager.OpenUI(Z_UIManager.UIType.Loading);

        yield return new WaitForSeconds(2f); // 로딩 시뮬레이션 (혹은 실제 비동기 로드)

        ReFitLogger.Info("로딩 완료 -> 타이틀 화면으로 전환");

        ChangeState(GameState.Title);
    }
    
    //2. 타이틀 화면 루틴
    private IEnumerator TitleRoutine()
    {
        ReFitLogger.Info("타이틀 화면 진입: 메뉴 대기");
        MyUIManager.CloseAllUI();

        //필요 UI 열기        
        var titleMenu = MyUIManager.OpenUI(Z_UIManager.UIType.TitleMenu);
        MyUIManager.OpenUI(Z_UIManager.UIType.Gyro);

        //자이로 입력 관리
        var gyroUI = MyUIManager.GetGyroUI(titleMenu);
        SetGyroInput(gyroUI, 2.0f);

        //유저 프로필 가져오기
        MyProfileManager.Login("testReFit@gmail.com", "testReFit", SystemInfo.deviceUniqueIdentifier, onLoginSuccess: () => {
            MyProfileManager.CreateTestProfile(onComplete: () => {

                // 2. 생성이 완벽히 끝난 시점에 비로소 프로필 조회를 실행합니다.
                MyProfileManager.FetchMyProfile();

            });
        });

        yield return null;
    }

    // 3. 인게임 상태 루틴
    private IEnumerator InGameRoutine()
    {
        ReFitLogger.Info("인게임 진입: 게임 루프 시작");
        MyUIManager.CloseAllUI();
        FindGameLogic();

        //필요 UI 열기
        MyUIManager.OpenUI(Z_UIManager.UIType.Gyro);

        //자이로 입력 대상 전달
        SetGyroInput(GameLogic, 1.0f);

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
        GyroHud.gyroInput = gyroInput;
        GyroHud.inputTriggerTime = triggerTime;
    }

    //게임 로직 찾기(씬 변환 시 호출)
    public void FindGameLogic()
    {
        //씬이 변환될 때마다 게임 로직을 찾아서 연결하는 작업을 수행
        GameLogic = GameObject.FindWithTag("GameLogic").GetComponent<IReFitGyro>();
        if (GameLogic == null)
        {
            ReFitLogger.Error("씬에서 GameLogic을 찾을 수 없습니다. GameLogic 오브젝트에 IReFitGyro 컴포넌트가 있는지 확인하세요.");
        }
    }

    public void ChangeScene(GameScene gameScene)
    {
        MyUIManager.CloseAllUI();

        switch (gameScene)
        {
            case GameScene.TitleScene:
                _currentScene = GameScene.TitleScene;
                SceneManager.LoadScene(0);
                break;
            case GameScene.AdventureScene:
                _currentScene = GameScene.AdventureScene;
                SceneManager.LoadScene(1);
                break;
            case GameScene.FightScene:
                _currentScene = GameScene.FightScene;
                SceneManager.LoadScene(2);
                break;
            case GameScene.AxeScene:
                _currentScene = GameScene.AxeScene;
                SceneManager.LoadScene(3);
                break;
            case GameScene.SwimScene:
                _currentScene = GameScene.SwimScene;
                SceneManager.LoadScene(4);
                break;
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isFirstLoad) isFirstLoad = false;
        else
        {
            MyUIManager.FindCanvas();
            ChangeState(_gameState);
        }
    }
}