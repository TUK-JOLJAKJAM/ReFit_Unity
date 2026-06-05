using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject UIManager;
    [SerializeField] public GameObject GyroManager;
    [SerializeField] public GameObject ProfileManager;
    [SerializeField] public GameObject DataManager;

    GameManager instance;

    private void Awake()
    {
        //½Ì±ÛÅæ ±¸Çö
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
