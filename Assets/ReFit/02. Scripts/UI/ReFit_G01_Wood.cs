using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReFit_G01_Wood : MonoBehaviour
{
    [SerializeField] public UIManager Manager;
    [SerializeField] public PointManager_Wood PointManager;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _pointText;

    private float _time = 0f;

    void Start()
    {
        if (UIManager.Instance != null)
        {
            Manager = UIManager.Instance;
        }

        PointManager = GameObject.FindWithTag("PointManager").GetComponent<PointManager_Wood>();
    }

    public void SetSlider(float time)
    {
        _slider.value = time;
    }

    public void SetPointText(int point)
    {
        _pointText.text = point.ToString();
    }
}
