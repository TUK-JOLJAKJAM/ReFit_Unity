using System.Collections;
using UnityEngine;

public class FightScene_PlayerUI : MonoBehaviour
{
    public RectTransform HPBar;
    public RectTransform ShieldBar;

    float BarMaxSize;

    Coroutine hpBarCoroutine;

    private void Awake()
    {
        gameObject.SetActive(true);
        BarMaxSize = HPBar.sizeDelta.x;
        HPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BarMaxSize);
        ShieldBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal , 246);
    }

    public void UpdateHpBar(float damage, float MaxHP)
    {
        if (hpBarCoroutine != null) StopCoroutine(hpBarCoroutine);

        hpBarCoroutine = StartCoroutine(HPCoroutine(damage, MaxHP));
    }

    private float duration = 1.5f;
    IEnumerator HPCoroutine(float damage, float MaxHP)
    {
        if (HPBar == null) yield break;

        // 1. 현재 HP바의 가로 길이 구하기
        Vector2 currentSize = HPBar.sizeDelta;
        float startWidth = currentSize.x;

        // 2. 데미지 비율(damage/100)만큼 감소한 목표 가로 길이 계산 (0 이하로 떨어지지 않도록 제한)
        float damageWidth = BarMaxSize * (damage / MaxHP);
        float targetWidth = Mathf.Max(0f, startWidth - damageWidth);

        // 3. 시간(duration) 동안 부드럽게 크기 줄이기 (Lerp 활용)
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentPercent = elapsedTime / duration;

            // 현재 크기에서 목표 크기로 보간
            currentSize.x = Mathf.Lerp(startWidth, targetWidth, currentPercent);
            HPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentSize.x);

            yield return null; // 다음 프레임까지 대기
        }

        // 4. 오차 방지를 위해 마지막에 최종 목표 크기로 확실히 설정
        currentSize.x = targetWidth;
        HPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentSize.x);
    }

    public void UpdateShieldBar(int count)
    {
        ShieldBar.SetSizeWithCurrentAnchors(RectTransform.Axis .Horizontal, 246 + 64 * count);
    }
}
