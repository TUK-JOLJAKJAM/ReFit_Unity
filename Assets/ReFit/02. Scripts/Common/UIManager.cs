using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // ================ Inspector UI ===================
    [Header("---UI Prefabs---")]
    [SerializeField] private GameObject[] menus;

    [Space(5)]
    [Header("---Canvas---")]
    [SerializeField] private RectTransform _uiParent = null;

    [Space(20)]
    [Header("***Read Only***")]
    [SerializeField] MenuState _currentState;
    [SerializeField] GameObject _currentUI;

    // ============== Hidden Data ==================
    public enum MenuState { Start, GameSelect, Options, GameInfo, Wood }
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
        }

        //처음 진입 시 메뉴로 입장
        ChangeMenu(MenuState.Start);
    }

    /// <summary>
    /// [UI의 상태를 변경해주는 함수]
    /// 0: 메인메뉴, 1: 마을, 2: 옵션, 3: 게임선택, 4: 장작패기
    /// </summary>
    public void ButtonDown_MenuSelect(MenuState menuIndex)
    {
        ChangeMenu(menuIndex);
    }
    public void ButtonDown_Exit() => Application.Quit();

    void ChangeMenu(MenuState newState)
    {

        _currentState = newState;

        //SingleTone
        if (_uiParent != null)
        {
            foreach (Transform child in _uiParent)
            {
                if (!IsOverlayMenu(newState)) Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogError("UI Parent가 할당되지 않았습니다. UiParent태그 캔버스에 붙이기");
        }
            //메뉴 생성
            GameObject uiPrefab = menus[(int)_currentState];
        if (uiPrefab != null && _uiParent != null)
        {
            GameObject newMenu = Instantiate(uiPrefab, _uiParent);
            _currentUI = newMenu;
        }

    }

    public void SetGameInfo(ReFit_U01_GameSelect.GameInfo gameInfo)
    {
        if(_currentState != MenuState.GameInfo)
        {
            Debug.LogError("현재 메뉴 상태가 게임 정보가 아닙니다.");
            return;
        }
        else
        {
            _currentUI.TryGetComponent<ReFit_U03_GameInfo>(out ReFit_U03_GameInfo gameInfoUI);
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

    bool IsOverlayMenu(MenuState newState)
    {
        bool isOverlay = false;
        
        switch(newState)
        {
            case MenuState.GameInfo:
                isOverlay = true;
                break;
        }

        return isOverlay;
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
        ChangeMenu(_currentState);
    }

    // 게임 씬으로 진입할 때 UI 상태를 게임 UI로 변경하는 함수
    public void SetGameUI(MenuState gameIndex)
    {
        _currentState = gameIndex;
    }
}