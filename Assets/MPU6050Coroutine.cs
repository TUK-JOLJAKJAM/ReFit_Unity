using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class MPU6050RotationFix : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM9", 9600);

    Quaternion baseRotation = Quaternion.identity; // 기준이 될 회전값
    bool isInitialized = false;

    void Start()
    {
        try
        {
            sp.Open();
            sp.ReadTimeout = 10;
            StartCoroutine(ReadSerialData());
        }
        catch (System.Exception e)
        {
            Debug.LogError("포트 연결 실패: " + e.Message);
        }
    }

    IEnumerator ReadSerialData()
    {
        while (sp.IsOpen)
        {
            string data = null;
            try
            {
                if (sp.BytesToRead > 0) data = sp.ReadLine();
            }
            catch (System.TimeoutException) { }

            if (!string.IsNullOrEmpty(data))
            {
                string[] v = data.Split(',');
                if (v.Length == 3)
                {
                    float x = float.Parse(v[0]);
                    float y = float.Parse(v[1]);
                    float z = float.Parse(v[2]);

                    // 1. 현재 센서의 날것 그대로의 회전값 생성
                    // (MPU6050 데이터는 보통 X-Pitch, Y-Roll, Z-Yaw 순서입니다)
                    Quaternion currentRawRotation = Quaternion.Euler(-x, -z, -y);

                    if (!isInitialized)
                    {
                        // 2. 초기 1회 실행: 현재 상태를 기준점(baseRotation)으로 잡음
                        // 역회전(Inverse)을 미리 계산해두면 나중에 편합니다.
                        baseRotation = Quaternion.Inverse(currentRawRotation);
                        isInitialized = true;
                        Debug.Log("영점 잡기 완료! 현재 위치가 (0,0,0)이 됩니다.");
                    }
                    else
                    {
                        // 3. 영점 보정 적용: (기준점의 역회전) * (현재 회전)
                        // 이렇게 하면 시작할 때 아무리 기울어져 있어도 0점에서 시작하는 것처럼 작동합니다.
                        transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * currentRawRotation, Time.deltaTime * 15f);
                    }
                }
            }
            yield return null;
        }
    }

    void OnApplicationQuit() { if (sp != null && sp.IsOpen) sp.Close(); }
}