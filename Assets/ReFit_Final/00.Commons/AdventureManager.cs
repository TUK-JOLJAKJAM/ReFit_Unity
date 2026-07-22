using UnityEngine;

public class AdventureManager : MonoBehaviour, IReFitManager
{
    #region 변수 선언
    [System.Serializable]
    public struct Node
    {
        public int id;
        public NodeType type;
        public FightScene_Logic.Skill attackType;
    }
    [System.Serializable]
    public enum NodeType
    {
        Fight,
        Axe,
        Swim,
        Boss
    }

    //--- 게임 정보 ---
    public Node[] RandomNode { get; private set; }

    [SerializeField] public int currentStageLevel;
    public int RedGauageLevel { get; private set; }
    public int BlueGauageLevel { get; private set; }
    public int GreenGauageLevel { get; private set; }
    #endregion

    #region IReFitManager 코드
    public void ResetReFitManager()
    {
        RandomNode = new Node[5];
        currentStageLevel = 1;
        RedGauageLevel = 0;
        BlueGauageLevel = 0;
        GreenGauageLevel = 0;
    }
    public void UpdateReFitManager()
    {

    }
    #endregion

    #region 게임 세팅
    public void SetRandomNode()
    {
        int bossTypeInt = Random.Range(1, 4);
        RandomNode[4].id = 4;
        RandomNode[4].type = NodeType.Boss;
        RandomNode[4].attackType = bossTypeInt == 1 ? FightScene_Logic.Skill.Red :
                bossTypeInt == 2 ? FightScene_Logic.Skill.Blue :
                FightScene_Logic.Skill.Green;
        for (int i = 0; i < 4; i++)
        {
            int typeInt = Random.Range(1, 4);

            RandomNode[i].type = NodeType.Fight;
            RandomNode[i].id = i;
            RandomNode[i].attackType = typeInt == 1 ? FightScene_Logic.Skill.Red :
                typeInt == 2 ? FightScene_Logic.Skill.Blue :
                FightScene_Logic.Skill.Green;

            //Axe, Swim 개발 후 밑에 주석 풀기
            /*RandomNode[i] = typeInt == 1 ? NodeType.Fight :
                typeInt == 2 ? NodeType.Axe :
                NodeType.Swim;*/
        }
    }
    public void ResetGameData()
    {
        SetRandomNode();
        currentStageLevel = 1;
        RedGauageLevel = 0;
        BlueGauageLevel = 0;
        GreenGauageLevel = 0;
    }

    public void NextLevel()
    {
        if(currentStageLevel < 5)currentStageLevel++;
    }
    #endregion
}
