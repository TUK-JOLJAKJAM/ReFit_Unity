using UnityEngine;

public class TitleMenu : MonoBehaviour, IReFitUI
{
    GameManager gameManager;

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

    public void PressStartButton()
    {
        gameManager.ChangeState(GameManager.GameState.InGame);
    }
}