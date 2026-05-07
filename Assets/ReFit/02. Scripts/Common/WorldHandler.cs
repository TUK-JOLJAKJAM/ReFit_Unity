using UnityEngine;

public class WorldHandler : MonoBehaviour
{
    //! ==================== Inspector UI ====================
    [SerializeField] public GameObject[] worldObjects; //각 월드에 해당하는 오브젝트들을 담는 배열 - GameSelect, Wood 등
    public bool TestMode = false;
    public WorldIndex TestWorld = WorldIndex.GameSelect;

    [Header("*** Read Only ***")]
    [SerializeField] public WorldIndex _currentWorld;

    //! ==================== Hidden Datas ====================
    public enum WorldIndex { GameSelect, Wood, CastleGuard }
    public static WorldHandler Instance;

    //! ==================== Functions =======================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //매니저는 삭제되지 않게
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _currentWorld = WorldIndex.GameSelect;
        SetWorld(_currentWorld);
        if (TestMode)
        {
            ChangeWorld(TestWorld);
        }
    }

    //실제 버튼에 할당하는 함수
    public void ChangeWorldInt(int index)
    {
        ChangeWorld((WorldIndex)index);
    }

    //월드 변경 함수 - GameSelect 월드에서 게임을 선택할 때 호출
    private void ChangeWorld(WorldIndex worldIndex)
    {
        _currentWorld = worldIndex;
        SetWorld(_currentWorld);

    }

    //게임 월드 생성 및 GameSelect 비활성화
    private void SetWorld(WorldIndex worldIndex)
    {
        worldObjects[(int)worldIndex].SetActive(true);
        DestroyOtherWorld();
    }

    //현재 월드 외 나머지 월드 비활성화
    private void DestroyOtherWorld()
    {
        for (int i = 0; i < worldObjects.Length; i++)
        {
            if (i != (int)_currentWorld)
            {
                worldObjects[i].SetActive(false);
            }
        }
    }
}
