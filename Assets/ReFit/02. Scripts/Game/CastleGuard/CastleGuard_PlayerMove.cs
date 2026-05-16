using UnityEngine;

public class CastleGuard_PlayerMove : MonoBehaviour
{
    GyroManager gyroManager;
    public CastleGuard_GameManager gameManager;

    public RectTransform AimUI;

    public float speed = 10f;
    Vector3 myposition;
    public RectTransform Canvas;

    [SerializeField] Vector3 offset;

    private void Start()
    {
        gyroManager = GyroManager.Instance;
        myposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager._currentState == CastleGuard_GameManager.GameState.Playing)
        {
            Quaternion gyroOffset = gyroManager.GetGyro();

            //debug
            offset = new Vector3(gyroOffset.eulerAngles.x, gyroOffset.eulerAngles.y, gyroOffset.eulerAngles.z);
            //

            transform.position = myposition + new Vector3(Mathf.Sin(gyroOffset.eulerAngles.x * Mathf.Deg2Rad) * speed, 0, 0);
            AimUI.localPosition = new Vector3(Mathf.Sin(gyroOffset.eulerAngles.x * Mathf.Deg2Rad) * Canvas.sizeDelta.x * 0.5f, Mathf.Sin(gyroOffset.eulerAngles.y * Mathf.Deg2Rad) * Canvas.sizeDelta.y * 0.5f, 0);
        }
    }
}
