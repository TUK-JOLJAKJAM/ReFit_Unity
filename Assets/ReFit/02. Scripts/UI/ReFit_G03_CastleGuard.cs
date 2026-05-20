using TMPro;
using UnityEngine;

public class ReFit_G03_CastleGuard : MonoBehaviour
{
    //! ==================== Inspector UI =====================
    [SerializeField] public RectTransform AmmoUI = null;
    [SerializeField] private TextMeshProUGUI pointText = null;
    [SerializeField] private TextMeshProUGUI lifeText = null;

    //! ==================== Hidden Datas ====================
    WorldHandler worldHandler;
    UIManager uiManager;

    //! ==================== Functions =======================
    private void Awake()
    {
        worldHandler = WorldHandler.Instance;
        uiManager = UIManager.Instance;
    }

    /// <summary>
    /// 포인트 UI 텍스트를 갱신합니다.
    /// </summary>
    public void UpdatePoint(int point)
    {
        if (pointText != null)
            pointText.text = $"Point : {point}";
    }

    /// <summary>
    /// 라이프 UI 텍스트를 갱신합니다.
    /// </summary>
    public void UpdateLife(int life)
    {
        if (lifeText != null)
            lifeText.text = $"Life : {life}";
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
        for(int i = AmmoUI.childCount - 1; i >= 0; i--)
        {
            if(AmmoUI.GetChild(i).gameObject.activeSelf)
            {
                AmmoUI.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }
}
