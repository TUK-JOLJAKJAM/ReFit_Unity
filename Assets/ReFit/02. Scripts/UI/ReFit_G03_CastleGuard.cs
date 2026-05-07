using UnityEngine;

public class ReFit_G03_CastleGuard : MonoBehaviour
{
    //! ==================== Inspector UI =====================
    [SerializeField] public RectTransform AmmoUI = null;

    //! ==================== Hidden Datas ====================
    WorldHandler worldHandler;
    UIManager uiManager;

    //! ==================== Functions =======================
    private void Awake()
    {
        worldHandler = WorldHandler.Instance;
        uiManager = UIManager.Instance;
    }

    public void ReLoad()
    {
        for (int i = 0; i < AmmoUI.childCount; i++)
        {
            AmmoUI.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void UseAmmo()
    {
        for(int i = AmmoUI.childCount - 1; i > 0; i--)
        {
            if(AmmoUI.GetChild(i).gameObject.activeSelf)
            {
                AmmoUI.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }
}
