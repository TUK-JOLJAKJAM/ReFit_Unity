using UnityEngine;

public class MainScene_Logic : MonoBehaviour, IReFitGyro
{
    [SerializeField] public MainScene_Player Player;

    enum MenuState
    {
        None, // 초기 상태
        Adventure, // 주요 컨텐츠 : 모험
        Profile,
        Web
    }

    MenuState currentMenu = MenuState.None;

    //-----------------IReFitGyro----------------
    bool isProfileOptionUIActive = false;
    public void GyroInputLeft()
    {
        if (isProfileOptionUIActive) return;

        MainScene_Player.PathEnum targetPath = MainScene_Player.PathEnum.NoneToProfile;
        bool Movable = true;

        //메뉴 상태에 따라 좌측 입력 시 동작 다르게 구현
        //왼쪽 메뉴로 이동
        switch (currentMenu)
        {
            case MenuState.None:
                currentMenu = MenuState.Profile;
                targetPath = MainScene_Player.PathEnum.NoneToProfile;
                break;
            case MenuState.Adventure:
                Movable = false;
                break;
            case MenuState.Profile:
                currentMenu = MenuState.Adventure;
                targetPath = MainScene_Player.PathEnum.ProfileToAdventure;
                break;
            case MenuState.Web:
                currentMenu = MenuState.Profile;
                targetPath = MainScene_Player.PathEnum.OptionsToProfile;
                break;
        }

        //플레이어 이동 코루틴
        if(Movable)Player.MoveToPosition(targetPath);
    }

    public void GyroInputRight()
    {
        if (isProfileOptionUIActive) return;

        MainScene_Player.PathEnum targetPath = MainScene_Player.PathEnum.NoneToProfile;
        bool Movable = true;

        //메뉴 상태에 따라 좌측 입력 시 동작 다르게 구현
        //오쪽 메뉴로 이동
        switch (currentMenu)
        {
            case MenuState.None:
                currentMenu = MenuState.Web;
                targetPath = MainScene_Player.PathEnum.NoneToOptions;
                break;
            case MenuState.Adventure:
                currentMenu = MenuState.Profile;
                targetPath = MainScene_Player.PathEnum.AdventureToProfile;
                break;
            case MenuState.Profile:
                currentMenu = MenuState.Web;
                targetPath = MainScene_Player.PathEnum.ProfileToOptions;
                break;
            case MenuState.Web:
                Movable = false;
                break;
        }

        //플레이어 이동 코루틴
        if (Movable) Player.MoveToPosition(targetPath);
    }

    public GameObject ProfileUI;
    public void GyroInputUp()
    {
        switch (currentMenu)
        {
            case MenuState.None:
                break;
            case MenuState.Adventure:
                GameManager.instance.MyAdventureManager.ResetGameData();
                GameManager.instance.ChangeScene(GameManager.GameScene.AdventureScene);
                break;
            case MenuState.Profile:
                ProfileUI.SetActive(true);
                ProfileUI?.GetComponent<Profile>().SetProfileUI();
                isProfileOptionUIActive = true;
                break;
            case MenuState.Web:
                Application.OpenURL("http://43.200.20.216");
                break;
        }
    }
    public void GyroInputDown()
    {
        switch (currentMenu)
        {
            case MenuState.Profile:
                ProfileUI.SetActive(false);
                isProfileOptionUIActive = false;
                break;
            case MenuState.Web:
                Application.Quit();
                break;
        }
    }
}
