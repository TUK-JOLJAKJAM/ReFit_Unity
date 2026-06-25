using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Z_GyroManager : MonoBehaviour, IReFitManager
{
    // 포트는 이전과 동일하게 유지 (방화벽에서 열었던 포트)
    private const int DataPort = 9001;

    private UdpClient dataReceiver;

    // [스레드 간 공유 변수] - 반드시 lock 블록 내부에서만 접근해야 합니다.
    private Quaternion receivedRotation = Quaternion.identity;
    private bool isDataReceived = false;
    private readonly object lockObject = new object();

    // [메인 스레드 전용 데이터 복사본] - lock 없이 안전하고 빠르게 접근하기 위함
    [SerializeField] private Quaternion localRotation = Quaternion.identity;

    // 기준이 되는 오프셋 회전값 (초기 정면 방향)
    [SerializeField] private Quaternion gyroOffset = Quaternion.identity;
    // 오프셋을 적용한 최종 연산 결과 캐시
    [SerializeField] private Quaternion offsetAppliedRotation = Quaternion.identity;

    //-----------------------------------------------------------------

    public void ResetReFitManager()
    {
        try
        {
            // 9001번 포트로 들어오는 모든 데이터를 수신 대기
            dataReceiver = new UdpClient(DataPort);
            dataReceiver.BeginReceive(OnDataReceived, null);
            ReFitLogger.Info($"[PC] {DataPort}번 포트에서 수신 대기 중...");

            // 시작할 때 기본 오프셋은 identity(회전 없음)로 초기화
            gyroOffset = Quaternion.identity;
            offsetAppliedRotation = Quaternion.identity;
        }
        catch (Exception e)
        {
            ReFitLogger.Error($"[PC] 소켓 오픈 실패: {e.Message}");
        }
    }

    public void UpdateReFitManager()
    {
        // 1. 비동기 스레드로부터 데이터가 새로 들어왔는지 확인
        if (isDataReceived)
        {
            // 2. lock을 걸고 안전하게 메인 스레드 전용 복사본(localRotation)으로 데이터 이전
            lock (lockObject)
            {
                localRotation = receivedRotation;
                isDataReceived = false;
            }

            // 데이터를 새로 받았을 때 오프셋 적용 연산도 메인 스레드에서 미리 처리해 둡니다.
            // 공식: 기준 회전의 역쿼터니언 * 현재 회전 = 기준점으로부터의 변화량
            offsetAppliedRotation = Quaternion.Inverse(gyroOffset) * localRotation;
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

                // 자물쇠를 잠그고 데이터 안전하게 기록
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
            ReFitLogger.Error($"[PC] 수신 에러: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        if (dataReceiver != null)
        {
            dataReceiver.Close();
            ReFitLogger.Info("[PC] UDP 수신 소켓이 안전하게 닫혔습니다.");
        }
    }

    // -----------------------------------------------------------------
    // [안전성 및 성능 최적화가 완료된 Getter 함수들]
    // -----------------------------------------------------------------

    /// <summary>
    /// 메인 스레드에서 최신 동기화된 자이로 값을 반환합니다.
    /// </summary>
    public Quaternion GetGyro()
    {
        return localRotation;
    }

    /// <summary>
    /// 현재 스마트폰의 회전 상태를 새로운 '정면(기준점)'으로 설정합니다.
    /// 주로 게임 시작 버튼을 누르거나, 유저가 수동으로 영점을 맞출 때 호출합니다.
    /// </summary>
    public void SetOffsetGyro()
    {
        // 현재 메인 스레드에 안전하게 복사되어 있는 최신 자이로 값을 기준점으로 삼음
        gyroOffset = localRotation;

        // 오프셋이 변경되었으므로 즉시 현재 프레임의 연산 결과도 업데이트
        offsetAppliedRotation = Quaternion.Inverse(gyroOffset) * localRotation;

        ReFitLogger.Info($"[Gyro] 자이로 영점(Offset)이 재설정되었습니다. 기준 값: {gyroOffset.eulerAngles}");
    }

    /// <summary>
    /// 설정된 기준점(Offset)을 기준으로 '얼마나 회전했는지' 변화량 쿼터니언을 반환합니다.
    /// 인게임 캐릭터 조작이나 조이스틱 판단 시 이 함수를 호출해야 합니다.
    /// </summary>
    public virtual Quaternion GetOffsetGyro()
    {
        return offsetAppliedRotation;
    }

    /// <summary>
    /// 오프셋 기준점으로부터 변경된 -1~1 사이의 정규화된 X축 자이로 값을 반환합니다.
    /// </summary>
    public float GetNormalizedOffsetGyroX()
    {
        return Mathf.Sin(offsetAppliedRotation.eulerAngles.x * Mathf.Deg2Rad);
    }

    /// <summary>
    /// 오프셋 기준점으로부터 변경된 -1~1 사이의 정규화된 Y축 자이로 값을 반환합니다.
    /// </summary>
    public float GetNormalizedOffsetGyroY()
    {
        return Mathf.Sin(offsetAppliedRotation.eulerAngles.y * Mathf.Deg2Rad);
    }

    /// <summary>
    /// 오프셋 기준점으로부터 변경된 -1~1 사이의 정규화된 Z축 자이로 값을 반환합니다.
    /// </summary>
    public float GetNormalizedOffsetGyroZ()
    {
        return Mathf.Sin(offsetAppliedRotation.eulerAngles.z * Mathf.Deg2Rad);
    }
}