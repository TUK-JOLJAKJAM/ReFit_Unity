using UnityEngine;

public class MainScene_Logic : MonoBehaviour, IReFitGyro
{
    [SerializeField] public MainScene_Player Player;

    enum MenuState
    {
        None, // 초기 상태
        Fight, // 주요 컨텐츠 : 전투 씬
        Axe, // 도끼 휘두르기
        Castle // 성벽 지키기(회피 훈련 으로 변경 예정)
    }

    MenuState currentMenu = MenuState.None;

    //-----------------IReFitGyro----------------
    public void GyroInputLeft()
    {
        MainScene_Player.PathEnum targetPath = MainScene_Player.PathEnum.NoneToAxe;
        bool Movable = true;

        //메뉴 상태에 따라 좌측 입력 시 동작 다르게 구현
        //왼쪽 메뉴로 이동
        switch (currentMenu)
        {
            case MenuState.None:
                currentMenu = MenuState.Axe;
                targetPath = MainScene_Player.PathEnum.NoneToAxe;
                break;
            case MenuState.Fight:
                Movable = false;
                break;
            case MenuState.Axe:
                currentMenu = MenuState.Fight;
                targetPath = MainScene_Player.PathEnum.AxeToFight;
                break;
            case MenuState.Castle:
                currentMenu = MenuState.Axe;
                targetPath = MainScene_Player.PathEnum.CastleToAxe;
                break;
        }

        //플레이어 이동 코루틴
        if(Movable)Player.MoveToPosition(targetPath);
    }

    public void GyroInputRight()
    {
        MainScene_Player.PathEnum targetPath = MainScene_Player.PathEnum.NoneToAxe;
        bool Movable = true;

        //메뉴 상태에 따라 좌측 입력 시 동작 다르게 구현
        //오쪽 메뉴로 이동
        switch (currentMenu)
        {
            case MenuState.None:
                currentMenu = MenuState.Castle;
                targetPath = MainScene_Player.PathEnum.NoneToCastle;
                break;
            case MenuState.Fight:
                currentMenu = MenuState.Axe;
                targetPath = MainScene_Player.PathEnum.FightToAxe;
                break;
            case MenuState.Axe:
                currentMenu = MenuState.Castle;
                targetPath = MainScene_Player.PathEnum.AxeToCastle;
                break;
            case MenuState.Castle:
                Movable = false;
                break;
        }

        //플레이어 이동 코루틴
        if (Movable) Player.MoveToPosition(targetPath);
    }

    public void GyroInputUp()
    {
        switch (currentMenu)
        {
            case MenuState.None:
                break;
            case MenuState.Fight:
                GameManager.instance.ChangeScene(GameManager.GameScene.FightScene);
                break;
            case MenuState.Axe:
                GameManager.instance.ChangeScene(GameManager.GameScene.AxeScene);
                break;
            case MenuState.Castle:
                break;
        }
    }
}
