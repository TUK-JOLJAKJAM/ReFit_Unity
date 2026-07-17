using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.Networking;

#region 1. 데이터 모델 정의

// --- 로그인 관련 모델 ---
[System.Serializable]
public class LoginRequestData
{
    public string email;
    public string password;
    public string clientType = "UNITY"; // 명세서의 기본값 반영
    public string deviceId;
}

[System.Serializable]
public class LoginResponse
{
    public string userId;
    public string authId;
    public string accessToken;
    public long issuedAtMs;
    public long expiresAtMs;
    public string refreshToken;
    public long refreshExpiresAtMs;
}

// --- 토큰 재발급 관련 모델 ---
[System.Serializable]
public class RefreshRequestData
{
    public string refreshToken;
}

// --- 로그아웃 관련 모델 ---
[System.Serializable]
public class LogoutResponse
{
    public string authId;
    public long logoutAtMs;
}

// --- 프로필 관련 모델 ---
[System.Serializable]
public class ProfileRequestData
{
    public float heightCm;
    public float weightKg;
    public string dominantHand;
    public List<string> diagnosisTags;
    public int painBaseline0to10;
    public string notes;
}

[System.Serializable]
public class UserProfile
{
    public string userId;
    public float heightCm;
    public float weightKg;
    public string dominantHand;
    public List<string> diagnosisTags;
    public int painBaseline0to10;
    public string notes;
    public long updatedAtMs;
}
#endregion

#region 2. 프로필 매니저 클래스
public class ProfileManager : MonoBehaviour, IReFitManager
{
    [Header("[ Runtime Connection ]")]
    [SerializeField] private string defaultApiBaseUrl = "http://43.200.20.216";
    [SerializeField] private bool allowCompatibilityDemoLogin = true;
    [SerializeField] private string compatibilityDemoEmail = "testReFit@gmail.com";
    [SerializeField] private string compatibilityDemoPassword = "testReFit";

    private string baseURL => RefitRuntimeConfig.ResolveApiBaseUrl(defaultApiBaseUrl);
    public bool UsingCompatibilityDemoAccount { get; private set; }

    // 인증 토큰 보관 변수
    public string _authToken { get; private set; }
    public string _refreshToken { get; private set; }

    // 연속 재발급 차단을 위한 시간 기록 변수
    private float _lastRefreshTime = -999f;
    private const float REFRESH_COOLDOWN_SEC = 5f; // 제한 시간 (5초)

    [Header("[ Current User Profile ]")]
    [SerializeField] private UserProfile _currentProfile;

    // 외부 스크립트에서 프로필 데이터를 읽을 때 사용하는 프로퍼티
    public UserProfile CurrentProfile => _currentProfile;

    /// <summary>
    /// 운영에서는 실행 인자, 환경 변수 또는 refit-runtime.json의 토큰/계정을 사용합니다.
    /// 설정이 없을 때만 기존 데모 계정으로 호환 로그인합니다.
    /// </summary>
    public void Authenticate(string deviceId, Action onLoginSuccess = null)
    {
        string accessToken = RefitRuntimeConfig.ResolveAccessToken();
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _authToken = accessToken.Trim();
            _refreshToken = RefitRuntimeConfig.ResolveRefreshToken() ?? string.Empty;
            UsingCompatibilityDemoAccount = false;
            onLoginSuccess?.Invoke();
            return;
        }

        string configuredEmail = RefitRuntimeConfig.ResolveEmail();
        string configuredPassword = RefitRuntimeConfig.ResolvePassword();
        if (!string.IsNullOrWhiteSpace(configuredEmail) && !string.IsNullOrWhiteSpace(configuredPassword))
        {
            UsingCompatibilityDemoAccount = false;
            Login(configuredEmail, configuredPassword, deviceId, onLoginSuccess);
            return;
        }

        if (allowCompatibilityDemoLogin)
        {
            UsingCompatibilityDemoAccount = true;
            Login(compatibilityDemoEmail, compatibilityDemoPassword, deviceId, onLoginSuccess);
            return;
        }

        UsingCompatibilityDemoAccount = false;
        ReFitLogger.Warning("[ProfileManager] 인증 설정이 없어 오프라인 모드로 시작합니다.");
    }

    /// <summary>
    /// 앱이 종료될 때 자동으로 서버 로그아웃을 호출합니다.
    /// </summary>
    private void OnApplicationQuit()
    {
        if (!string.IsNullOrEmpty(_authToken))
        {
            ReFitLogger.Info($"[{gameObject.name} -> ProfileManager] 앱 종료로 인한 자동 로그아웃 시도...");
            Logout();
        }
    }

    /// <summary>
    /// 1. 명세서에 맞게 이메일과 패스워드로 로그인을 요청합니다.
    /// </summary>
    public void Login(string email, string password, string deviceId, Action onLoginSuccess = null)
    {
        StartCoroutine(LoginCoroutine(email, password, deviceId, onLoginSuccess));
    }

    private IEnumerator LoginCoroutine(string email, string password, string deviceId, Action onLoginSuccess)
    {
        string url = $"{baseURL}/api/v1/auth/login";

        LoginRequestData requestBody = new LoginRequestData
        {
            email = email,
            password = password,
            deviceId = deviceId
        };
        string jsonBody = JsonUtility.ToJson(requestBody);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(rawData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                LoginResponse res = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);
                _authToken = res.accessToken;
                _refreshToken = res.refreshToken; // 발급받은 리프레시 토큰 저장

                ReFitLogger.Info($"[{gameObject.name} -> ProfileManager] 로그인 성공!");
                onLoginSuccess?.Invoke();
            }
            else
            {
                string errorMsg = webRequest.responseCode == 401 ? "자격 증명이 유효하지 않음 (401)" : webRequest.error;
                ReFitLogger.Warning($"[{gameObject.name} -> ProfileManager] 로그인 실패: {webRequest.responseCode} - {errorMsg}");
            }
        }
    }

    /// <summary>
    /// 만료된 토큰을 Refresh Token을 사용해 재발급합니다.
    /// 5초 이내에 연속으로 호출되면 무한 루프로 판단하여 차단합니다.
    /// </summary>
    public void RefreshTokens(Action onSuccess = null)
    {
        // 현재 유니티가 시작된 이후 흐른 시간(초)을 구함
        float currentTime = Time.time;

        // 마지막으로 재발급에 성공/시도한 지 5초가 지나지 않았다면
        if (currentTime - _lastRefreshTime < REFRESH_COOLDOWN_SEC)
        {
            ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] ⚠️ 경고: 5초 이내에 토큰 재발급이 재호출되었습니다! 무한 루프 방지를 위해 재시도를 중단합니다. 서버 권한(403)이나 Security 설정을 확인하세요.");

            // 더 이상 서버에 요청을 보내지 않고 로컬 데이터를 정리한 뒤 종료
            ClearLocalData();
            return;
        }

        // 현재 시간을 마지막 재발급 시도 시간으로 기록
        _lastRefreshTime = currentTime;

        if (string.IsNullOrEmpty(_refreshToken))
        {
            ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] 리프레시 토큰이 없어 재발급이 불가능합니다. 다시 로그인해야 합니다.");
            ClearLocalData();
            return;
        }

        StartCoroutine(RefreshTokensCoroutine(onSuccess));
    }

    private IEnumerator RefreshTokensCoroutine(Action onSuccess)
    {
        string url = $"{baseURL}/api/v1/auth/refresh";

        RefreshRequestData requestData = new RefreshRequestData { refreshToken = _refreshToken };
        string jsonBody = JsonUtility.ToJson(requestData);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(rawData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                LoginResponse res = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);

                _authToken = res.accessToken;
                _refreshToken = res.refreshToken;

                ReFitLogger.Info($"[{gameObject.name} -> ProfileManager] 토큰 재발급(Refresh) 완료!");

                // 성공한 시점의 시간을 다시 정확히 기록
                _lastRefreshTime = Time.time;

                onSuccess?.Invoke();
            }
            else
            {
                string errorMsg = webRequest.responseCode == 401 ? "리프레시 토큰 만료 또는 유효하지 않음 (401)" : webRequest.error;
                ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] 토큰 재발급 실패: {webRequest.responseCode} - {errorMsg}");
                ClearLocalData();
            }
        }
    }

    /// <summary>
    /// 3. 내 프로필 데이터를 서버에서 가져옵니다.
    /// </summary>
    public void FetchMyProfile(Action<UserProfile> onSuccess = null, Action<string> onFailure = null)
    {
        if (string.IsNullOrEmpty(_authToken))
        {
            ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] 토큰이 없습니다. 로그인을 먼저 진행해주세요.");
            onFailure?.Invoke("토큰 없음");
            return;
        }
        StartCoroutine(FetchMyProfileCoroutine(onSuccess, onFailure));
    }

    private IEnumerator FetchMyProfileCoroutine(Action<UserProfile> onSuccess, Action<string> onFailure)
    {
        string url = $"{baseURL}/api/v1/users/me/profile";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                _currentProfile = JsonUtility.FromJson<UserProfile>(webRequest.downloadHandler.text);

                ReFitLogger.Info($"[{gameObject.name} -> ProfileManager] 프로필 로드 성공!");
                onSuccess?.Invoke(_currentProfile);
            }
            else
            {
                // 토큰 만료 또는 권한 오류(403/401) 발생 시 자동 재발급 시도 구조 적용 가능
                if (webRequest.responseCode == 403 || webRequest.responseCode == 401)
                {
                    ReFitLogger.Warning($"[{gameObject.name} -> ProfileManager] 프로필 로드 중 토큰 만료 감지. 재발급 시도...");
                    RefreshTokens(onSuccess: () => FetchMyProfile(onSuccess, onFailure));
                    yield break;
                }

                string errorMsg = webRequest.responseCode == 404 ? "프로필 없음 (404)" : webRequest.error;
                ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] 프로필 로드 실패: {errorMsg}");
                onFailure?.Invoke(errorMsg);
            }
        }
    }

    /// <summary>
    /// 4. [테스트용] 일회성으로 유저 프로필 데이터를 생성하거나 수정(Upsert)합니다.
    /// </summary>
    public void CreateTestProfile(Action onComplete = null)
    {
        if (string.IsNullOrEmpty(_authToken))
        {
            ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] 토큰이 없습니다. 로그인을 먼저 진행해주세요.");
            return;
        }
        StartCoroutine(CreateTestProfileCoroutine(onComplete));
    }

    private IEnumerator CreateTestProfileCoroutine(Action onComplete)
    {
        string url = $"{baseURL}/api/v1/users/me/profile";

        // 테스트용 더미 데이터 세팅
        ProfileRequestData testData = new ProfileRequestData
        {
            heightCm = 175.5f,
            weightKg = 72.3f,
            dominantHand = "r",
            diagnosisTags = new List<string> { "ROUND_SHOULDER" },
            painBaseline0to10 = 5,
            notes = "Unity test profile"
        };

        string jsonBody = JsonUtility.ToJson(testData);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "PUT"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(rawData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                _currentProfile = JsonUtility.FromJson<UserProfile>(webRequest.downloadHandler.text);
                ReFitLogger.Info($"[{gameObject.name} -> ProfileManager] 테스트 프로필 생성/수정 성공!");

                onComplete?.Invoke();
            }
            else
            {
                // 토큰 만료 또는 권한 오류(403/401) 발생 시 자동 재발급 후 재시도
                if (webRequest.responseCode == 403 || webRequest.responseCode == 401)
                {
                    ReFitLogger.Warning($"[{gameObject.name} -> ProfileManager] 프로필 생성 중 토큰 만료 감지. 재발급 시도...");
                    RefreshTokens(onSuccess: () => CreateTestProfile(onComplete));
                    yield break;
                }

                string serverMessage = webRequest.downloadHandler.text;
                ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] 테스트 프로필 생성 실패: {webRequest.responseCode} - {webRequest.error}\n서버 메시지: {serverMessage}");
            }
        }
    }

    /// <summary>
    /// 5. 서버에 로그아웃을 요청하고 로컬 데이터를 비웁니다.
    /// </summary>
    public void Logout(Action onSuccess = null, Action<string> onFailure = null)
    {
        if (string.IsNullOrEmpty(_authToken))
        {
            ClearLocalData();
            onSuccess?.Invoke();
            return;
        }
        StartCoroutine(LogoutCoroutine(onSuccess, onFailure));
    }

    private IEnumerator LogoutCoroutine(Action onSuccess, Action<string> onFailure)
    {
        string url = $"{baseURL}/api/v1/auth/logout";

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, ""))
        {
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ReFitLogger.Info($"[{gameObject.name} -> ProfileManager] 서버 로그아웃 성공!");
                ClearLocalData();
                onSuccess?.Invoke();
            }
            else
            {
                string errorMsg = webRequest.responseCode == 401 ? "인증되지 않은 토큰 (401)" : webRequest.error;
                ReFitLogger.Error($"[{gameObject.name} -> ProfileManager] 로그아웃 실패: {webRequest.responseCode} - {errorMsg}");

                ClearLocalData();
                onFailure?.Invoke(errorMsg);
            }
        }
    }

    /// <summary>
    /// 로컬 저장 데이터(토큰, 프로필) 초기화
    /// </summary>
    private void ClearLocalData()
    {
        _authToken = "";
        _refreshToken = "";
        _currentProfile = null;
        ReFitLogger.Info($"[{gameObject.name} -> ProfileManager] 로컬 유저 데이터 초기화 완료.");
    }

    public void UpdateReFitManager() { }
    public void ResetReFitManager() { }
}
#endregion
