using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Z_UIManager : MonoBehaviour
{
    [SerializeField] public IReFitUI[] UIArray;
    [SerializeField] private Canvas _canvas;

    public enum UIType
    {
        MainMenu,
        Settings,
        UserInfo,
        Pause
    }

    //-----------------------------------------------------------------

    public void ResetUIManager()
    {
        foreach (var ui in UIArray)
        {
            ui.Initalize();
        }

        FindCanvas();
    }

    public void UpdateUIManager()
    {
        foreach (var ui in UIArray)
        {
            if(ui.GetGameObject().activeSelf == true) ui.UpdateUI();
        }
    }

    public void OpenUI(UIType uiType)
    {
        UIArray[(int)uiType].GetGameObject().SetActive(true);
    }

    public void CloseUI(UIType uiType)
    {
        UIArray[(int)uiType].GetGameObject().SetActive(false);
    }

    private void FindCanvas()
    {
        //캔버스 찾기
        if (_canvas == null)
        {
            _canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
            if (_canvas == null) ReFItLogger.Error("캔버스 태그가 없습니다. 혹은 캔버스가 존재하지 않습니다.");
        }

        //UI 오브젝트들을 캔버스의 자식으로 설정하고 비활성화
        for (int i = 0; i < UIArray.Length; i++)
        {
            UIArray[i].GetGameObject().transform.SetParent(_canvas.transform, false);
            UIArray[i].GetGameObject().SetActive(false);
        }
    }
}