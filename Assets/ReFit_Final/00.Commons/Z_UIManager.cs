using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class Z_UIManager : MonoBehaviour, IReFitManager
{
    IReFitUI[] UIArray;
    [SerializeField] private Canvas _canvas;

    //UI 열거형의 순서와, 실제 UI매니저의 자식 오브젝트의 순서가 일치해야한다.
    //또한, 순서는 더 위에 보일 것이 더 아래 자식이도록 할 것.
    public enum UIType
    {
        Loading,
        TitleMenu,
        Settings,
        UserInfo,
        Pause,
        Gyro
    }

    //-----------------------------------------------------------------

    public void ResetReFitManager()
    {
        InitUIArray();

        if (System.Enum.GetValues(typeof(UIType)).Length != UIArray.Length)
        {
            ReFitLogger.Error("UIType 열거형의 개수와 UI매니저의 자식 오브젝트의 개수가 일치하지 않습니다.");
            ReFitLogger.Error($"UIType 열거형의 개수: {System.Enum.GetValues(typeof(UIType)).Length},  UI매니저의 자식 오브젝트의 개수: {UIArray.Length}");
            return;
        }

        FindCanvas();

        foreach (var ui in UIArray)
        {
            ui.Initialize();
        }
    }

    public void UpdateReFitManager()
    {
        foreach (var ui in UIArray)
        {
            if(ui.GetGameObject().activeSelf == true) ui.UpdateUI();
        }
    }

    public GameObject OpenUI(UIType uiType)
    {
        GameObject targetUI = UIArray[(int)uiType].GetGameObject();
        targetUI.SetActive(true);
        targetUI.transform.SetParent(_canvas.transform, false);

        return targetUI;
    }

    public void CloseUI(UIType uiType)
    {
        GameObject targetUI = UIArray[(int)uiType].GetGameObject();
        targetUI.SetActive(false);
        targetUI.transform.SetParent(this.transform, false);
    }

    public IReFitGyro GetGyroUI(GameObject gyroUI)
    {
        IReFitGyro gyroInput = gyroUI.GetComponent<IReFitGyro>();
        if (gyroInput == null)
        {
            ReFitLogger.Error("자이로 UI에 IReFitGyro 컴포넌트가 없습니다.");
            return null;
        }
        return gyroInput;
    }

    //상태 변경 이전에 모든 UI를 닫아준다.
    //씬 변경 시에도 UI매니저의 밑으로 모든 UI가 들어가도록 해준다. -> 그래야 사라지지 않는다.
    public void CloseAllUI()
    {
        foreach (var ui in UIArray)
        {
            ui.GetGameObject().SetActive(false);
            ui.GetGameObject().transform.SetParent(this.transform, false);
        }
    }

    //씬이 바뀔 때 캔버스를 새로 찾아줘야한다.
    public void FindCanvas()
    {
        //캔버스 찾기
        if (_canvas == null)
        {
            _canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
            if (_canvas == null) ReFitLogger.Error("캔버스 태그가 없습니다. 혹은 캔버스가 존재하지 않습니다.");
        }

        //UI 오브젝트들을 비활성화
        for (int i = 0; i < UIArray.Length; i++)
        {
            UIArray[i].GetGameObject().SetActive(false);
        }
    }

    void InitUIArray()
    {
        //UI매니저가 자식으로 가지고 있는 IReFitUI 컴포넌트들을 배열에 넣는다.
        UIArray = GetComponentsInChildren<IReFitUI>(true);

        //모두 false상태로 만든다.
        foreach (var ui in UIArray)
        {
            ui.GetGameObject().SetActive(false);
        }
    }
}