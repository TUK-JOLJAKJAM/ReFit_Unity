using System.IO.Ports;
using System;
using UnityEngine;
using System.Collections;

public class DeviceManager_Wood : MonoBehaviour
{
    [Header("회전시킬 객체")]
    [SerializeField] Transform TargetObject;

    [Header("디바이스 회전 정보")]
    [SerializeField] Vector3 DeviceRotation;

    SerialPort sp;

    [Header("Settings")]
    public string portName = "COM8"; // 아두이노 포트 번호
    public int baudRate = 115200;
    public float lerpSpeed = 10f;    // 회전을 부드럽게 만드는 속도

    [Header("필터 설정")]
    [Range(0.01f, 0.2f)] public float filterSmoothing = 0.1f; // 낮을수록 노이즈가 사라지지만 반응이 약간 느려짐
    private Vector3 smoothedRotation;

    public float primaryZ = 0f;

    void Start()
    {
        // 시리얼 포트 설정 및 열기
        sp = new SerialPort(portName, baudRate);
        try
        {
            sp.Open();
            sp.ReadTimeout = 1; // 읽기 타임아웃 설정
            Debug.Log("Serial Port Opened!");
        }
        catch (Exception e)
        {
            Debug.LogError("Could not open serial port: " + e.Message);
        }
    }

    void Update()
    {
        /*if (sp != null && sp.IsOpen)
        {
            try
            {
                // 1. 현재 시리얼 버퍼에 쌓인 모든 줄(Line)을 다 읽어버립니다.
                // 이렇게 하면 옛날 데이터는 순식간에 지나가고 마지막 최신 데이터가 targetRotation에 담깁니다.
                while (sp.BytesToRead > 0)
                {
                    string data = sp.ReadLine();
                    string[] values = data.Split(',');
                    Debug.Log("Device Rotation: " + data);
                    if (values.Length == 3)
                    {
                        float x = float.Parse(values[0]);
                        float y = float.Parse(values[1]);
                        float z = float.Parse(values[2]);

                        Vector3 rawTarget = new Vector3(x, y, -z);

                        smoothedRotation = Vector3.Lerp(smoothedRotation, rawTarget, filterSmoothing);
                    }
                }
            }
            catch (System.TimeoutException) { }
            catch (System.Exception e) { Debug.LogWarning(e.Message); }
        }

        // 2. 루프 밖에서 부드럽게 회전 적용 (이게 있어야 물체가 실제로 움직입니다)
        *//*Quaternion targetQuat = Quaternion.Euler(smoothedRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuat, Time.deltaTime * lerpSpeed);*//*

        TargetObject.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -smoothedRotation.z + primaryZ));
        Debug.Log("Device Rotation: " + smoothedRotation);
        // 인스펙터 확인용 변수 업데이트
        DeviceRotation = smoothedRotation;*/
    }

    void OnApplicationQuit()
    {
        // 게임 종료 시 포트 닫기 (매우 중요!)
        if (sp != null && sp.IsOpen) sp.Close();
    }

    public void setPrimaryZ()
    {
        TargetObject.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -18.79f));
        primaryZ = smoothedRotation.z;
    }

    public IEnumerator RotationAxe()
    {
        yield return null;

        while (true)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    // 1. 현재 시리얼 버퍼에 쌓인 모든 줄(Line)을 다 읽어버립니다.
                    // 이렇게 하면 옛날 데이터는 순식간에 지나가고 마지막 최신 데이터가 targetRotation에 담깁니다.
                    while (sp.BytesToRead > 0)
                    {
                        string data = sp.ReadLine();
                        string[] values = data.Split(',');
                        Debug.Log("Device Rotation: " + data);
                        if (values.Length == 3)
                        {
                            float x = float.Parse(values[0]);
                            float y = float.Parse(values[1]);
                            float z = float.Parse(values[2]);

                            Vector3 rawTarget = new Vector3(x, y, -z);

                            smoothedRotation = Vector3.Lerp(smoothedRotation, rawTarget, filterSmoothing);
                        }
                    }
                }
                catch (System.TimeoutException) { }
                catch (System.Exception e) { Debug.LogWarning(e.Message); }
            }

            // 2. 루프 밖에서 부드럽게 회전 적용 (이게 있어야 물체가 실제로 움직입니다)
            /*Quaternion targetQuat = Quaternion.Euler(smoothedRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuat, Time.deltaTime * lerpSpeed);*/

            TargetObject.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -smoothedRotation.z + primaryZ));
            Debug.Log("Device Rotation: " + smoothedRotation);
            // 인스펙터 확인용 변수 업데이트
            DeviceRotation = smoothedRotation;

            yield return null;
        }
    }
}
