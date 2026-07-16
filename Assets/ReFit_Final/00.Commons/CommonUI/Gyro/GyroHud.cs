using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GyroHud : MonoBehaviour, IReFitUI
{
    GameManager gameManager;
    public RectTransform hudPointer;
    public HudDirections hudUp;
    public HudDirections hudDown;
    public HudDirections hudLeft;
    public HudDirections hudRight;

    public IReFitGyro gyroInput;
    public float inputTriggerTime = 0.0f;

    float pointerOffsetMax = 40.0f;

    public Vector2 TestGyro;

    public enum GyroDirection
    {
        Up,
        Down,
        Left,
        Right,
        Center
    }

    //테스트용 오브젝트
    public GameObject testCircle;

    //----------IReFitUI--------------
    public void Initialize()
    {
        gameManager = GameManager.instance;
        
        if(gameManager.MyTestHandler.isTestMode)
        {
            testCircle.SetActive(true);
        }
        else
        {
            testCircle.SetActive(false);
        }

        hudUp.image = hudUp.GetComponent<Image>();
        hudDown.image = hudDown.GetComponent<Image>();
        hudLeft.image = hudLeft.GetComponent<Image>();
        hudRight.image = hudRight.GetComponent<Image>();

        HudReset();
    }
    public void UpdateUI()
    {
        //테스트모드에서는 클릭하여 포인터 이동하도록 구현, 실제 게임에서는 자이로 센서 값에 따라 포인터 이동
        if (gameManager.MyTestHandler.isTestMode)
        {
            if (Input.GetMouseButton(0))
            {
                // 마우스의 스크린 좌표 * 1/4 을 포인터 부모(Canvas) 기준의 로컬 좌표로 변환
                RectTransform parentRect = hudPointer.parent as RectTransform;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, null, out Vector2 localPoint))
                {
                    // 포인터가 최대 반경(pointerOffsetMax)을 벗어나지 않도록 제한
                    localPoint = Vector2.ClampMagnitude(localPoint * 0.25f, pointerOffsetMax);

                    // 자이로와 비슷하게 부드럽게 마우스 위치로 이동
                    hudPointer.anchoredPosition = Vector2.Lerp(hudPointer.anchoredPosition, localPoint, Time.deltaTime * 10.0f);

                    // ---------------- [새로 추가된 코드] ----------------
                    // 현재 UI의 위치(anchoredPosition)를 최대 반경(pointerOffsetMax)으로 나누어 -1 ~ 1 범위로 변환합니다.
                    if (pointerOffsetMax > 0)
                    {
                        TestGyro = hudPointer.anchoredPosition / pointerOffsetMax;
                    }
                    // -------------------------------------------------
                }
            }
            else
            {
                // 마우스를 떼면 다시 중앙으로 부드럽게 복귀
                hudPointer.anchoredPosition = Vector2.Lerp(hudPointer.anchoredPosition, Vector2.zero, Time.deltaTime * 5.0f);
                // ---------------- [새로 추가된 코드] ----------------
                // UI가 중앙으로 복귀하는 흐름에 맞춰 TestGyro 값도 자연스럽게 변환됩니다.
                if (pointerOffsetMax > 0)
                {
                    TestGyro = new Vector2(0, 0);
                }
                // --------------------------------------------------
            }
        }
        else
        {
            hudPointer.anchoredPosition = Vector2.Lerp(hudPointer.anchoredPosition,
                new Vector2(gameManager.MyGyroManager.GetNormalizedOffsetGyroY() *
                pointerOffsetMax, -gameManager.MyGyroManager.GetNormalizedOffsetGyroX() * pointerOffsetMax),
                Time.deltaTime * 5.0f);
        }
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
    
    //---------------------------------------------------------------
    //인수로 들어온 enum에 따라 GyroInput 함수를 호출
    public void GyroInputEnter(GyroDirection gyroDirection)
    {
        switch (gyroDirection)
        {
            case GyroDirection.Up:
                gyroInput.GyroInputUp();
                break;
            case GyroDirection.Down:
                gyroInput.GyroInputDown();
                break;
            case GyroDirection.Left:
                gyroInput.GyroInputLeft();
                break;
            case GyroDirection.Right:
                gyroInput.GyroInputRight();
                break;
            case GyroDirection.Center:
                break;
        }
    }

    void HudReset()
    {
        hudUp.enabled = false;
        hudDown.enabled = false;
        hudLeft.enabled = false;
        hudRight.enabled = false;
    }
}