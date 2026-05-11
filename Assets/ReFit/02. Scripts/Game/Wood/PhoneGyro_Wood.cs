using Unity.VisualScripting;
using UnityEngine;

public class PhoneGyro_Wood : MonoBehaviour
{
    public GyroManager gyroManager;
    public Transform targetObject;

    public Vector3 defaultRotation = new Vector3(0, 0, -15); // 초기 회전값
    public float rotationSensitivity = 1.0f; // 회전 민감도


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gyroManager = GyroManager.Instance;
        targetObject.rotation = Quaternion.Euler(defaultRotation);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion receivedRotation = gyroManager.GetGyro();

        Vector3 receivedRotationVector3 = new Vector3(0, 0, gyroManager.GetNormalizedGyroY() * rotationSensitivity);

        targetObject.rotation = Quaternion.Euler(defaultRotation + receivedRotationVector3);
    }
}
