using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GameResult
{
    [Header("----Json µ•¿Ã≈Õ----")]
    [SerializeField] public int Perfect = 0;
    [SerializeField] public int Good = 0;
    [SerializeField] public int Bad = 0;
    [SerializeField] public int Miss = 0;
    [SerializeField] public int input_right = 0;
    [SerializeField] public int input_left = 0;
}

public class Point : MonoBehaviour
{
    int mypoint = 0;
    float speed = 2.0f;
    public TextMeshProUGUI result = null;

    public GameResult gameData = new GameResult();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        result.text = "Result : " + mypoint.ToString();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Red"))
        {
            mypoint += 3;
            gameData.Perfect++;
        }
        else if (collision.gameObject.CompareTag("Green"))
        {
            mypoint += 2;
            gameData.Good++;
        }
        else if (collision.gameObject.CompareTag("Blue"))
        {
            mypoint += 1;
            gameData.Bad++;
        }
        else
        {
            mypoint--;
            gameData.Miss++;
        }


        Debug.Log(mypoint);
        transform.position = new Vector3(0, 5, 0);
    }

    void Move()
    {
        transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gameData.input_right++;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gameData.input_left++;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(gameData, true);
        string path = Path.Combine(Application.persistentDataPath, "SaveData.json");
        File.WriteAllText(path, json);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
