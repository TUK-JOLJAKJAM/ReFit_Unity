using UnityEngine;

public class AdventureManager : MonoBehaviour, IReFitManager
{
    #region 변수 선언
    [System.Serializable]
    public enum NodeType
    {
        Fight,
        Axe,
        Swim,
        Boss
    }

    //--- 게임 정보 ---
    public NodeType[] RandomNode { get; private set; }
    public int currentStageLevel { get; private set; }
    public int RedGauageLevel { get; private set; }
    public int BlueGauageLevel { get; private set; }
    public int GreenGauageLevel { get; private set; }
    #endregion

    #region IReFitManager 코드
    public void ResetReFitManager()
    {
        RandomNode = new NodeType[5];
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
        RandomNode[4] = NodeType.Boss;

        for (int i = 0; i < 4; i++)
        {
            RandomNode[i] = NodeType.Fight;

            //Axe, Swim 개발 후 밑에 주석 풀기
            /*int typeInt = Random.Range(1, 4);

            RandomNode[i] = typeInt == 1 ? NodeType.Fight :
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
    #endregion
}
