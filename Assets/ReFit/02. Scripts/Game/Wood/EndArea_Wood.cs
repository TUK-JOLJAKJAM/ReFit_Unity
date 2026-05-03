using UnityEngine;

public class EndArea_Wood : MonoBehaviour
{
    // ==================== Inspector UI ====================
    [Header("--- Object ---")]
    [SerializeField] public PointManager_Wood PointManager;
    [SerializeField] public ReFit_G01_Wood GameUI;
    [SerializeField] private StartArea_Wood StartArea_Wood;

    // ==================== Hidden Datas ====================
    // ==================== Functions =======================
    private void OnTriggerEnter(Collider other)
    {
        if(PointManager.IsValidSwing())
        {
            PointManager.AddPoint();
            GameUI.SetPointText(PointManager.GetPoint());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PointManager.IsValidSwing())
        {
            PointManager.SetSwinging(false);
            PointManager.SetValidSwing(false);
            StartArea_Wood.ResetTimer();
            GameUI.SetSlider(0f);
        }
    }
}
