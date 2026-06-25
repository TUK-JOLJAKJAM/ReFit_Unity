
using System.Diagnostics; // [Conditional]을 사용하기 위해 필요합니다.
using Debug = UnityEngine.Debug; // 유니티의 Debug 클래스와 이름 충돌을 방지합니다.

public static class ReFitLogger
{
    // 유니티 에디터 안에서나, 개발용 빌드(Development Build)에서만 이 함수가 컴파일에 포함됩니다.
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Info(object message)
    {
        Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Warning(object message)
    {
        Debug.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Error(object message)
    {
        Debug.LogError(message);
    }
}
