using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour, IReFitManager
{
    // --- IReFitManager 구현 ---
    public void ResetReFitManager() { }
    public void UpdateReFitManager() { }

    // 기본 주소는 기존 데모와 호환하고, 실행 인자/환경 변수/로컬 설정으로 교체할 수 있습니다.
    [SerializeField] private string defaultApiBaseUrl = "http://43.200.20.216";

    private string GameHistoryUrl =>
        $"{RefitRuntimeConfig.ResolveApiBaseUrl(defaultApiBaseUrl)}/api/v1/game-histories";

    /// <summary>
    /// 매개변수로 개별 값을 받아 게임 히스토리를 서버에 저장합니다.
    /// </summary>
    public void SaveGameHistory(
        string gameId,
        string gameName,
        string gameVersion,
        string primaryPart,
        string deviceId,
        long startedAtMs,
        long endedAtMs,
        int score,
        int actionCount,
        int successCount,
        int failCount,
        Dictionary<string, object> sessionSummary,
        List<BodyPartSummary> bodyPartSummaries,
        List<GameActionRecord> gameData,
        Action<bool, string> onComplete = null)
    {
        // 1. 받은 매개변수들로 DTO 객체 생성 및 데이터 매핑
        GameHistoryRequest requestData = new GameHistoryRequest
        {
            schemaVersion = "2.0",
            gameId = gameId,
            gameName = gameName,
            gameVersion = gameVersion,
            primaryPart = primaryPart,
            clientType = "UNITY", // 고정값
            deviceId = deviceId,
            startedAtMs = startedAtMs,
            endedAtMs = endedAtMs,
            score = score,
            actionCount = actionCount,
            successCount = successCount,
            failCount = failCount,
            sessionSummary = sessionSummary,
            bodyPartSummaries = bodyPartSummaries,
            gameData = gameData
        };

        // 2. 코루틴을 통해 전송 시작
        StartCoroutine(PostGameHistoryCoroutine(requestData, onComplete));
    }

    private IEnumerator PostGameHistoryCoroutine(GameHistoryRequest data, Action<bool, string> onComplete)
    {
        string jsonPayload = JsonConvert.SerializeObject(data);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(GameHistoryUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            string token = GetAuthToken();

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            // 1. 실제 응답 코드 및 에러 메시지 확인
            long responseCode = request.responseCode;
            string errorText = request.error ?? "";

            // 일부 플랫폼에서 responseCode가 0으로 오고 에러 메시지에 코드가 포함되는 현상 방어
            bool isUnauthorized = (responseCode == 401 || responseCode == 403) ||
                                  (responseCode == 0 && (errorText.Contains("401") || errorText.Contains("403")));

            if (isUnauthorized)
            {
                // 플랫폼 이슈로 responseCode가 0일 경우, 로그 상에는 실제 검출 결과 기준(401)으로 보완하여 표시합니다.
                long displayCode = (responseCode == 0) ? (errorText.Contains("403") ? 403 : 401) : responseCode;

                ReFitLogger.Warning($"[DataManager] 오류코드: {displayCode} - 토큰 만료 감지. 토큰 재발급을 시도합니다. (상세에러: {errorText})");

                if (GameManager.instance != null && GameManager.instance.MyProfileManager != null)
                {
                    // 토큰 재발급 요청 후 성공 시 재전송 시도
                    GameManager.instance.MyProfileManager.RefreshTokens(onSuccess: () =>
                    {
                        ReFitLogger.Info("[DataManager] 토큰 재발급 완료! 게임 히스토리 전송을 재시도합니다.");
                        StartCoroutine(PostGameHistoryCoroutine(data, onComplete));
                    });
                }
                else
                {
                    ReFitLogger.Error("[DataManager] GameManager 또는 MyProfileManager가 null 상태여서 토큰 재발급이 불가능합니다.");
                    onComplete?.Invoke(false, "Token expired and MyProfileManager is null.");
                }

                yield break; // 현재의 실패한 요청 루프 중단
            }

            // 2. 응답이 성공인 경우 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                try
                {
                    GameHistoryResponse responseData = JsonConvert.DeserializeObject<GameHistoryResponse>(responseText);
                    if (responseData != null && !string.IsNullOrEmpty(responseData.historyId))
                    {
                        ReFitLogger.Info($"[DataManager] 게임 히스토리 저장 성공! 생성된 히스토리 ID: {responseData.historyId}");
                    }
                    else
                    {
                        ReFitLogger.Warning("[DataManager] 저장 성공했으나 응답에서 historyId를 찾을 수 없습니다.");
                    }
                }
                catch (Exception ex)
                {
                    ReFitLogger.Warning($"[DataManager] 응답 JSON 해석 실패: {ex.Message}\n서버 응답 원본: {responseText}");
                }

                onComplete?.Invoke(true, responseText);
            }
            else
            {
                // 401/403 외의 일반 오류 처리
                ReFitLogger.Error($"[DataManager] 게임 히스토리 저장 실패! 오류코드: {responseCode} (에러명: {request.error})\n응답 내용: {request.downloadHandler.text}");
                onComplete?.Invoke(false, request.downloadHandler.text);
            }
        }
    }

    private string GetAuthToken()
    {
        try
        {
            if (GameManager.instance != null && GameManager.instance.MyProfileManager != null)
            {
                string token = GameManager.instance.MyProfileManager._authToken;
                if (!string.IsNullOrEmpty(token))
                {
                    // 토큰 양 끝에 혹시 모를 쌍따옴표(")가 있다면 안전하게 제거합니다.
                    return token.Trim('"').Trim();
                }
            }
        }
        catch (Exception ex)
        {
            ReFitLogger.Warning($"[DataManager] 인증 토큰을 불러오는 중 실패: {ex.Message}");
        }
        return null;
    }
}

#region Data Transfer Objects (DTO)

[Serializable]
public class GameHistoryRequest
{
    public string schemaVersion;
    public string gameId;
    public string gameName;
    public string gameVersion;
    public string primaryPart;
    public string clientType;
    public string deviceId;
    public long startedAtMs;
    public long endedAtMs;
    public int score;
    public int actionCount;
    public int successCount;
    public int failCount;
    public Dictionary<string, object> sessionSummary;
    public List<BodyPartSummary> bodyPartSummaries;
    public List<GameActionRecord> gameData;
}

[Serializable]
public class BodyPartSummary
{
    public string bodyPart;
    public string side;
    public int? pain0to10;
    public int? stiffness0to10;
    public int? fatigue0to10;
    public bool? swelling;
    public string notes;
    public Dictionary<string, object> metrics;
}

[Serializable]
public class GameActionRecord
{
    public string actionId;
    public string actionType;
    public string exerciseCode;
    public string direction;
    public long startedAtMs;
    public long endedAtMs;
    public long durationMs;
    public bool success;
    public string attackGrade;
    public float reactionTimeMs;
    public List<SensorSampleRecord> samples = new List<SensorSampleRecord>();
}

[Serializable]
public class SensorSampleRecord
{
    public long timestampMs;
    public float qx;
    public float qy;
    public float qz;
    public float qw;
}

[Serializable]
public class GameHistoryResponse
{
    public string historyId;
}

#endregion
