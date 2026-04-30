using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class PCReceiver : MonoBehaviour
{
    // 포트는 이전과 동일하게 유지 (방화벽에서 열었던 포트)
    private const int DataPort = 9001;

    private UdpClient dataReceiver;
    public Transform targetObject;

    // 수신된 데이터를 안전하게 전달하기 위한 변수
    private Quaternion receivedRotation = Quaternion.identity;
    private bool isDataReceived = false;
    private readonly object lockObject = new object();

    void Start()
    {
        try
        {
            // 9001번 포트로 들어오는 모든 데이터를 수신 대기
            dataReceiver = new UdpClient(DataPort);
            dataReceiver.BeginReceive(OnDataReceived, null);
            Debug.Log($"[PC] {DataPort}번 포트에서 수신 대기 중... (PC IP: 10.124.192.9)");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PC] 소켓 오픈 실패: {e.Message}");
        }
    }

    void Update()
    {
        // 메인 스레드에서 오브젝트 회전 적용
        if (isDataReceived)
        {
            lock (lockObject)
            {
                if (targetObject != null)
                {
                    // 부드러운 움직임을 원하시면 Slerp를 사용하세요
                    //targetObject.rotation = Quaternion.Slerp(targetObject.rotation, receivedRotation, Time.deltaTime * 15f);
                    targetObject.rotation = receivedRotation;
                }
                isDataReceived = false;
            }
        }
    }

    private void OnDataReceived(IAsyncResult res)
    {
        try
        {
            IPEndPoint remoteIp = new IPEndPoint(IPAddress.Any, DataPort);
            byte[] data = dataReceiver.EndReceive(res, ref remoteIp);

            // 16바이트(float 4개) 데이터인지 확인
            if (data.Length == 16)
            {
                float x = BitConverter.ToSingle(data, 0);
                float y = BitConverter.ToSingle(data, 4);
                float z = BitConverter.ToSingle(data, 8);
                float w = BitConverter.ToSingle(data, 12);

                lock (lockObject)
                {
                    receivedRotation = new Quaternion(x, y, z, w);
                    isDataReceived = true;
                }
            }

            // 다음 데이터 수신을 위해 다시 시작
            dataReceiver.BeginReceive(OnDataReceived, null);
        }
        catch (ObjectDisposedException) { }
        catch (Exception e)
        {
            Debug.LogError($"[PC] 수신 에러: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        dataReceiver?.Close();
    }
}