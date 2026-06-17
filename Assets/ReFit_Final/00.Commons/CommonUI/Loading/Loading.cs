using UnityEngine;

public class Loading : MonoBehaviour, IReFitUI
{
    void IReFitUI.Initialize()
    {
        //로딩 화면 초기화 작업이 필요한 경우 여기에 작성
    }
    
    void IReFitUI.UpdateUI()
    {
        //로딩 화면 업데이트 작업이 필요한 경우 여기에 작성
    }

    GameObject IReFitUI.GetGameObject()
    {
        return this.gameObject;
    }
}
