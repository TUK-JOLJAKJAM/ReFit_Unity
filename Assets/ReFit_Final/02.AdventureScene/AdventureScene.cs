using UnityEngine;

public class AdventureScene : MonoBehaviour
{
    public RectTransform[] Pointer;

    public void SetPointer()
    {
        Pointer[GameManager.instance.MyAdventureManager.currentStageLevel-1].gameObject.SetActive(true);
    }
}
