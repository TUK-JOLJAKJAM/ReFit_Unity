using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SimpleReceiver : MonoBehaviour
{
    UdpClient receiver;
    int port = 9005; // 복잡한 포트 말고 새로운 포트로 테스트

    void Start()
    {
        receiver = new UdpClient(port);
        receiver.BeginReceive(OnData, null);
        Debug.Log($"[PC] {port}번 포트에서 기다리는 중...");
    }

    void OnData(System.IAsyncResult res)
    {
        IPEndPoint remoteIp = new IPEndPoint(IPAddress.Any, port);
        byte[] data = receiver.EndReceive(res, ref remoteIp);
        string message = Encoding.UTF8.GetString(data);

        Debug.Log($"[PC] 수신 성공! 내용: {message} (보낸 기기 IP: {remoteIp.Address})");

        receiver.BeginReceive(OnData, null); // 다시 대기
    }

    void OnApplicationQuit() => receiver?.Close();
}