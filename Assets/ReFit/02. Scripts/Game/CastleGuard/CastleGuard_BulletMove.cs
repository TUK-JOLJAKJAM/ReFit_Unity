using System.Security.Cryptography;
using UnityEngine;

public class CastleGuard_BulletMove : MonoBehaviour
{
    float speed = 10f;
    GyroManager gyroManager;
    Vector3 moveDirection;

    void Start()
    {
        gyroManager = GyroManager.Instance;
        //Vector3 aimAngle = new Vector3(-gyroManager.GetNormalizedGyroY() * 45f, gyroManager.GetNormalizedGyroX() * 45f * Mathf.Abs(gyroManager.GetNormalizedGyroX()), 0);
        //moveDirection = GetVectorFromAngle(aimAngle);
        moveDirection = transform.forward; // 총알이 발사되는 방향으로 이동
        Destroy(gameObject, 3f); // 5초 후에 총알 삭제 (필요에 따라 조정)
    }

    void Update()
    {
        
        transform.Translate(moveDirection * Time.deltaTime * speed, Space.World);
    }

    Vector3 GetVectorFromAngle(Vector3 eulerAngles)
    {
        // 1. 오일러 각도를 쿼터니언으로 변환
        Quaternion rotation = Quaternion.Euler(eulerAngles);

        // 2. 앞방향 벡터(0,0,1)를 해당 회전값만큼 회전시킴
        Vector3 direction = rotation * Vector3.forward;

        return direction;
    }
}
