using UnityEngine;

[System.Serializable]
public class TitleMenu : MonoBehaviour, IReFitUI, IReFitGyro
{
    GameManager gameManager;

    //----------IReFitUI--------------
    public void Initialize()
    {
        gameManager = GameManager.instance;
    }

    public void UpdateUI()
    {

    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    //--------------IReFitGyro----------------
    public void GyroInputUp()
    {
        PressStartButton();
    }

    //--------------UI Button Event-----------
    public void PressStartButton()
    {
        gameManager.ChangeState(GameManager.GameState.InGame);
    }
}