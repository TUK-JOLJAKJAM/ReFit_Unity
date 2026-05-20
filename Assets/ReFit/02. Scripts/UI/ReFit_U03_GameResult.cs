using UnityEngine;

public class ReFit_U03_GameResult : MonoBehaviour
{
    //! ==================== Inspector UI =====================
    [Header("-------오브젝트-------")]
    [Space(30)]
    [Header("********** Read Only **********")]
    [Header("매니저")]
    [SerializeField] public UIManager Manager;
    //! ==================== Hidden Datas ====================

    //! ==================== Functions =======================
    private void Start()
    {
        if(UIManager.Instance != null)
        {
            Manager = UIManager.Instance;
        }
    }

    public void ButtonDown_MainMenu()
    {
        WorldHandler.Instance.ChangeWorldInt(0);
        Destroy(gameObject);
    }
}
