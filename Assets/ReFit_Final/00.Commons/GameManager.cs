using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Z_UIManager UIManager;
    [SerializeField] public Z_GyroManager GyroManager;
    [SerializeField] public ProfileManager ProfileManager;
    [SerializeField] public DataManager DataManager;

    [SerializeField] public GameManager instance;

    //-----------------------------------------------------------------------

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

    private void Update()
    {
        
    }
}
