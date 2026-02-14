using UnityEngine;

public class ReFit_U00_StartMenu : MonoBehaviour
{
    public UIManager Manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(UIManager.Instance != null)
        {
            Manager = UIManager.Instance;
        }
    }

    public void ButtonDown_Start()
    {
        Manager.ButtonDown_MenuSelect(UIManager.MenuState.GameSelect);
    }

    public void ButtonDown_Exit()
    {
        Manager.ButtonDown_Exit();
    }
}
