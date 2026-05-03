using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReFit_U02_GameInfo : MonoBehaviour
{
    [SerializeField] public UIManager UIManager;
    [SerializeField] public WorldHandler WorldHandler;

    [SerializeField] private TextMeshProUGUI _gameTitleText;
    [SerializeField] private TextMeshProUGUI _gameDescText;
    [SerializeField] private TextMeshProUGUI _gameBodyText;         

    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager = UIManager.Instance;
            WorldHandler = WorldHandler.Instance;
        }
    }

    public void SetGameInfo(ReFit_G00_GameSelect.GameInfo gameInfo)
    {
        _gameTitleText.text = gameInfo.Name;
        _gameDescText.text = gameInfo.Description;
        _gameBodyText.text = gameInfo.Body;
    }

    public void ButtonDown_Back()
    {
        UIManager.ButtonDown_MenuSelect(UIManager.MenuState.None);
        UIManager.ExitMenu(gameObject);
    }

    public void ButtonDown_GameStart(int gameIndex)
    {
        Debug.Log(gameIndex);
        WorldHandler.ChangeWorldInt(gameIndex + 1);
        UIManager.ExitMenu(gameObject);
    }
}
