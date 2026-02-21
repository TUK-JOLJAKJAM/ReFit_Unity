using System.IO;
using UnityEngine;

[System.Serializable]
public class WoodGameResult
{
    [Header("----Json 데이터----")]
    [SerializeField] public string body_part = "WAIST";
    [SerializeField] public string side = "NONE";
    [SerializeField] public int duration_sec = 420;
    [SerializeField] public int score = 12800;
}

public class WoodPoint : MonoBehaviour
{
    public WoodGameResult gameData = new WoodGameResult();

    public void SaveData()
    {
        string json = JsonUtility.ToJson(gameData, true);
        string path = Path.Combine(Application.persistentDataPath, "WoodGameData.json");
        File.WriteAllText(path, json);

        Debug.Log($"JSON 파일 저장 완료: {path}");
        Debug.Log($"내용:\n{json}");
    }
}
