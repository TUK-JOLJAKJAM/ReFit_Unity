using UnityEngine;

public class TestHandler : MonoBehaviour
{
    public bool isTestMode = false;

    private void Start()
    {
        if(isTestMode)ReFitLogger.Info("테스트 모드입니다.");
    }
}
