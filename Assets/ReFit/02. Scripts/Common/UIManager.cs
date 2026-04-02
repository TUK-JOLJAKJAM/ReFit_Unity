using System.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

//동적 UI 관리하는 역할
public class UIManager : MonoBehaviour
{
    // ================ Inspector UI ===================
    [Header("---UI Prefabs---")]
    [SerializeField] private GameObject[] menus;

    [Space(5)]
    [Header("---Canvas---")]
    [SerializeField] private RectTransform _uiParent = null;

    [Space(5)]
    [Header("---StartScene---")]
    [SerializeField] private MenuState _startSceneState = MenuState.Start;

    [Header("---Effects---")]
    [SerializeField] Animation _gameStartEffect = null;

    [Space(20)]
    [Header("***Read Only***")]
    [SerializeField] MenuState _currentState = MenuState.None;
    [SerializeField] GameObject _currentUI = null;

    // ============== Hidden Data ==================
    public enum MenuState { Start, Options, GameInfo, GameResult, None }
    public static UIManager Instance;

    // =============== Functions ==================
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //매니저는 삭제되지 않게
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //처음 진입 시 메뉴로 입장
        ChangeMenu(_startSceneState);
    }

    /// <summary>
    /// [UI의 상태를 변경해주는 함수]
    /// 0: 메인메뉴, 1: 옵션, 2. 게임 정보, 3. 게임 결과, 4: 없음
    /// </summary>
    public void ButtonDown_MenuSelect(MenuState menuIndex)
    {
        ChangeMenu(menuIndex);
    }
    public void ButtonDown_Exit() => Application.Quit();

    void ChangeMenu(MenuState newState)
    {
        if(newState == MenuState.None || newState == _currentState)
        {
            if(newState == _currentState) Debug.Log("이미 해당 메뉴 상태입니다.");
            return;
        }

        _currentState = newState;

        //메뉴 생성
        GameObject uiPrefab = menus[(int)_currentState];

        if (uiPrefab != null && _uiParent != null)
        {
            GameObject newMenu = Instantiate(uiPrefab, _uiParent);
            _currentUI = newMenu;
            newMenu.SetActive(true);
        }

    }

    public void ExitMenu(GameObject uiSelf)
    {
        Destroy(uiSelf);
        _currentState = MenuState.None;
    }

    public void SetGameInfo(ReFit_G00_GameSelect.GameInfo gameInfo)
    {
        if(_currentState != MenuState.GameInfo)
        {
            Debug.LogError("현재 메뉴 상태가 게임 정보가 아닙니다.");
            return;
        }
        else
        {
            if(_currentUI == null)
            {
                Debug.LogError("현재 활성화된 UI가 없습니다.");
                return;
            }
            _currentUI.TryGetComponent<ReFit_U02_GameInfo>(out ReFit_U02_GameInfo gameInfoUI);
            if (gameInfoUI != null)
            {
                //게임 정보 UI에 게임 정보를 전달하여 업데이트
                gameInfoUI.SetGameInfo(gameInfo);
            }
            else
            {
                Debug.LogError("게임 정보 UI 컴포넌트를 찾을 수 없습니다.");
            }
        }
        
        
    }

    // 씬이 로드될 때마다 UI Parent를 다시 찾도록 설정
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _uiParent = GameObject.FindWithTag("UiParent").GetComponent<RectTransform>();
        //_currentState = MenuState.None;
    }

    public RectTransform GetUIParent()
    {
        return _uiParent;
    }

    public IEnumerator SetGameStartEffect()
    {
        //게임 시작 효과를 설정하는 로직을 여기에 추가
        //예: 화면 전환, 애니메이션 재생 등
        Debug.Log("게임 시작 효과 설정");
        yield return null;
        
        if(_gameStartEffect == null)
        {
            Debug.LogWarning("게임 시작 효과가 할당되지 않았습니다.");
            yield break;
        }

        if (_gameStartEffect != null && _gameStartEffect.isPlaying != true)
        {
            _gameStartEffect.Play();
        }

        while (_gameStartEffect.isPlaying)
        {
            yield return null;
        }

        yield return null;
    }

    //게임에서 다시 메인으로 돌아올 때 사용하는 함수
    public void ReturnToMainMenu()
    {
        //메인 메뉴 씬으로 이동
        SceneManager.LoadScene(0);
        _currentState = MenuState.Start;
    }
}