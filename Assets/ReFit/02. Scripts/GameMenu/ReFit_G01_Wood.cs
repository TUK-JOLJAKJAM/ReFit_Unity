using UnityEngine;

public class ReFit_G01_Wood : MonoBehaviour
{
    [SerializeField] public UIManager Manager;

    void Start()
    {
        if (UIManager.Instance != null)
        {
            Manager = UIManager.Instance;
        }
    }
}
