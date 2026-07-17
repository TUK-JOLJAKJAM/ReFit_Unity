using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 빌드를 다시 만들지 않고 API 주소와 인증 정보를 교체하기 위한 런타임 설정입니다.
/// 우선순위: 실행 인자 > 환경 변수 > persistentDataPath/refit-runtime.json > 호환 기본값.
/// </summary>
public static class RefitRuntimeConfig
{
    [Serializable]
    private class RuntimeOptions
    {
        public string apiBaseUrl;
        public string email;
        public string password;
        public string accessToken;
        public string refreshToken;
        public bool createDemoProfile;
    }

    private static RuntimeOptions _cached;

    public static string ResolveApiBaseUrl(string fallback)
    {
        string value = FirstNonEmpty(
            CommandLineValue("--refit-api-base"),
            Environment.GetEnvironmentVariable("REFIT_API_BASE_URL"),
            Load().apiBaseUrl,
            fallback);
        return (value ?? string.Empty).Trim().TrimEnd('/');
    }

    public static string ResolveEmail(string compatibilityFallback = null) => FirstNonEmpty(
        CommandLineValue("--refit-email"),
        Environment.GetEnvironmentVariable("REFIT_EMAIL"),
        Load().email,
        compatibilityFallback);

    public static string ResolvePassword(string compatibilityFallback = null) => FirstNonEmpty(
        CommandLineValue("--refit-password"),
        Environment.GetEnvironmentVariable("REFIT_PASSWORD"),
        Load().password,
        compatibilityFallback);

    public static string ResolveAccessToken() => FirstNonEmpty(
        CommandLineValue("--refit-access-token"),
        Environment.GetEnvironmentVariable("REFIT_ACCESS_TOKEN"),
        Load().accessToken);

    public static string ResolveRefreshToken() => FirstNonEmpty(
        CommandLineValue("--refit-refresh-token"),
        Environment.GetEnvironmentVariable("REFIT_REFRESH_TOKEN"),
        Load().refreshToken);

    public static bool ShouldCreateDemoProfile(bool compatibilityFallback) =>
        Load().createDemoProfile || compatibilityFallback;

    private static RuntimeOptions Load()
    {
        if (_cached != null) return _cached;
        _cached = new RuntimeOptions();
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "refit-runtime.json");
            if (File.Exists(path))
            {
                _cached = JsonConvert.DeserializeObject<RuntimeOptions>(File.ReadAllText(path))
                          ?? new RuntimeOptions();
            }
        }
        catch (Exception ex)
        {
            ReFitLogger.Warning($"[RuntimeConfig] 설정 파일을 읽지 못했습니다: {ex.Message}");
        }
        return _cached;
    }

    private static string CommandLineValue(string key)
    {
        string prefix = key + "=";
        foreach (string arg in Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return arg.Substring(prefix.Length).Trim('"');
        }
        return null;
    }

    private static string FirstNonEmpty(params string[] values)
    {
        foreach (string value in values)
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        return null;
    }
}
