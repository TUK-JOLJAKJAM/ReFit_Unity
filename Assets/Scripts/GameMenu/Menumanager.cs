using UnityEngine;

public class Menumanager : MonoBehaviour
{
    [SerializeField] GameObject[] menus; // 0: Start, 1: Options, 2: GameSelect
    [SerializeField] MenuState currentState;

    enum MenuState { Start, Options, GameSelect }

    void Start()
    {
        // 처음 시작 시 Start 메뉴 표시
        ChangeMenu(MenuState.Start);
    }

    // 버튼에서 호출할 함수들
    public void ButtonDown_Start() => ChangeMenu(MenuState.GameSelect);
    public void ButtonDown_Options() => ChangeMenu(MenuState.Options);
    public void ButtonDown_Back() => ChangeMenu(MenuState.Start);
    public void ButtonDown_Exit() => Application.Quit();

    // 메뉴를 교체하는 핵심 로직
    void ChangeMenu(MenuState newState)
    {
        currentState = newState;

        // 1. 기존에 생성된 자식(이전 메뉴)이 있다면 모두 삭제
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 2. 새로운 메뉴 프리팹 생성
        GameObject prefabToInstantiate = menus[(int)currentState];
        if (prefabToInstantiate != null)
        {
            GameObject newMenu = Instantiate(prefabToInstantiate, transform);
            // Instantiate의 두 번째 인자로 transform을 넣으면 즉시 자식으로 생성됩니다.
        }
    }
}