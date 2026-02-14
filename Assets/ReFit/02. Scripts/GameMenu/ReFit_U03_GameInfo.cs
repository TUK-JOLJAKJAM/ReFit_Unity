using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReFit_U03_GameInfo : MonoBehaviour
{
    [SerializeField] public UIManager Manager;

    [SerializeField] private TextMeshProUGUI _gameTitleText;
    [SerializeField] private TextMeshProUGUI _gameDescText;
    [SerializeField] private TextMeshProUGUI _gameBodyText;

    void Start()
    {
        if (UIManager.Instance != null)
        {
            Manager = UIManager.Instance;
        }
    }

    public void SetGameInfo(ReFit_U01_GameSelect.GameInfo gameInfo)
    {
        _gameTitleText.text = gameInfo.Name;
        _gameDescText.text = gameInfo.Description;
        _gameBodyText.text = gameInfo.Body;
    }

    public void ButtonDown_Back()
    {
        Manager.ButtonDown_MenuSelect(UIManager.MenuState.GameSelect);
    }

    public void ButtonDown_GameStart(int gameIndex)
    {
        SceneManager.LoadScene(gameIndex);
    }
}
