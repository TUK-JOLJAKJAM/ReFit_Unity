using TMPro;
using UnityEngine;

public class FightScene_Win : MonoBehaviour
{
    public RectTransform[] Graphs;
    public TextMeshProUGUI[] GraphText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI PlayerHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < Graphs.Length; i++)
        {
            Vector2 defaultPos = Graphs[i].anchoredPosition;
            Vector2 newPos = new Vector2(defaultPos.x, -500);
            Graphs[i].anchoredPosition = newPos;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values">공격 판정 카운트</param>
    /// <param name="Time">플레이타임</param>
    /// <param name="HP">플레이어 잔여 체력</param>
    public void SetWinUI(int[] values, float Time, float HP)
    {
        //최댓값찾기
        int maxValue = Mathf.Max(values);

        //길이 정하기, 텍스트 수정
        for (int i = 0; i < Graphs.Length; i++)
        {
            Graphs[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50 + 400 * values[i] / maxValue);
            GraphText[i].text = values[i].ToString();
        }
        TimeText.text = Time.ToString("0.#") + "s";
        PlayerHP.text = HP.ToString("0.#")+"/100 ";
    }
}
