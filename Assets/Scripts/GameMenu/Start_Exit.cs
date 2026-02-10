using UnityEngine;

public class Start_Exit : MonoBehaviour
{
    public Menumanager Manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Manager = transform.parent.GetComponent<Menumanager>();
    }

    public void ButtonDown_Start()
    {
        Manager.ButtonDown_Start();
    }

    public void ButtonDown_Exit()
    {
        Manager.ButtonDown_Exit();
    }
}
