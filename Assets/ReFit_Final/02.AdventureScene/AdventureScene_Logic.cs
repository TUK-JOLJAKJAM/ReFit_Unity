using UnityEngine;

public class AdventureScene_Logic : MonoBehaviour, IReFitGyro
{
    //위 입력 시 입장 구현하기
    public void GyroInputUp()
    {
        {
            AdventureManager adventureManager = GameManager.instance.MyAdventureManager;
            int level = adventureManager.currentStageLevel;

            switch (adventureManager.RandomNode[level-1].type)
            {
                case AdventureManager.NodeType.Fight:
                    //전투 세팅 함수 호출
                    GameManager.instance.ChangeScene(GameManager.GameScene.FightScene);
                    break;
                case AdventureManager.NodeType.Swim:
                    break;
                case AdventureManager.NodeType.Axe:
                    break;
                case AdventureManager.NodeType.Boss:
                    //보스전 세팅 함수 호출
                    GameManager.instance.ChangeScene(GameManager.GameScene.FightScene);
                    break;
            }
        }
    }

    public void GyroInputDown()
    {
        GameManager.instance.ChangeScene(GameManager.GameScene.TitleScene);
    }
}
