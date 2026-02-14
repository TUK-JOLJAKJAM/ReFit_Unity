using Unity.Jobs;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // ================ Inspector UI ===================
    [Header("---UI Prefabs---")]
    [SerializeField] private GameObject[] menus;

    [Space(5)]
    [Header("---Canvas---")]
    [SerializeField] private Transform _uiParent = null;

    [Space(20)]
    [Header("***Read Only***")]
    [SerializeField] MenuState _currentState;
    [SerializeField] GameObject _currentUI;

    // ============== Hidden Data ==================
    public enum MenuState { Start, GameSelect, Options, GameInfo }
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
    /// 0: 메인메뉴, 1: 마을, 2: 옵션, 3: 게임선택
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
        foreach (Transform child in _uiParent)
        {
            if(!IsOverlayMenu(newState)) Destroy(child.gameObject);
        }

        //메뉴 생성
        GameObject uiPrefab = menus[(int)_currentState];
        if (uiPrefab != null)
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
}