using UnityEngine;

public class FightScene_ShieldMove : MonoBehaviour
{

    // --- 이동 ---
    public int speed = 10;
    public RectTransform rect;

    private void Update()
    {
        Vector2 thisPos = rect.anchoredPosition;

        float newX;
        // 1. 새로운 X 위치를 먼저 계산합니다.
        if(GameManager.instance.MyTestHandler.isTestMode)
            newX = thisPos.x + GameManager.instance.GyroHud.TestGyro.x * speed * Time.deltaTime;
        else
            newX = thisPos.x + GameManager.instance.MyGyroManager.GetNormalizedOffsetGyroY() * speed * Time.deltaTime;

            // 2. Mathf.Clamp를 사용하여 X 값을 -304에서 304 사이로 제한합니다.
            newX = Mathf.Clamp(newX, -304f, 304f);

        // 3. 제한된 X 값을 적용합니다.
        rect.anchoredPosition = new Vector2(newX, thisPos.y);
    }
}
