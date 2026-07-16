using UnityEngine;

public class FightScene_MonsterAttack : MonoBehaviour
{
    public RectTransform rect;
    public int speed = 500;

    private void Update()
    {
        Vector2 thisPos = rect.anchoredPosition;

        rect.anchoredPosition = new Vector2(thisPos.x, thisPos.y - speed * Time.deltaTime);
    }
}
