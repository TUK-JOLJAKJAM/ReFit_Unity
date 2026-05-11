using System;
using UnityEngine;

public class ReFit_G00_GameSelect : MonoBehaviour
{
    [SerializeField] public UIManager Manager;
    [SerializeField] public GameInfo[] Games;

    public enum GameIndex
    {
        Wood, CastleGuard
    }

    //게임 설명
    [System.Serializable]
    public class GameInfo
    {
        public string Name;
        public string Description;
        public string Body;
        public GameIndex GameIndex;
    }


    void Start()
    {
        if (UIManager.Instance != null)
        {
            Manager = UIManager.Instance;
        }
    }

    public void ButtonDown_GameSelect(int gameIndex)
    {
        Manager.ButtonDown_MenuSelect(UIManager.MenuState.GameInfo);
        Manager.SetGameInfo(Games[gameIndex]);
    }
}
