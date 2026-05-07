using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CastleGuard_PlayerMove : MonoBehaviour
{
    GyroManager gyroManager;

    public RectTransform AimUI;

    public float speed = 10f;
    Vector3 myposition;
    public RectTransform Canvas;

    private void Start()
    {
        gyroManager = GyroManager.Instance;
        myposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = myposition + new Vector3(Mathf.Sin(gyroManager.GetGyro().eulerAngles.x * Mathf.Deg2Rad) * speed, 0, 0);
        AimUI.localPosition = new Vector3(Mathf.Sin(gyroManager.GetGyro().eulerAngles.x * Mathf.Deg2Rad) * Canvas.sizeDelta.x * 0.5f, 0, 0);
    }
}
