using System.Collections;
using UnityEngine;

public class CastleGuard_EnemyFactory : MonoBehaviour
{
    public CastleGuard_GameManager GameManager;
    public GameObject EnemyPrefab;
    public Transform EnemyParent;

    private int _maxEnemyCount = 5;


    private Vector3[] _spawnPoints = new Vector3[]
    {
        new Vector3(-13, 4, 5),
        new Vector3(-13, 2, 5),
        new Vector3(-13, 0, 5),
        new Vector3(13, 3, 5),
        new Vector3(13, 1, 5)
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        if (GameManager._currentState == CastleGuard_GameManager.GameState.Playing)
        {
            if (EnemyParent.childCount < _maxEnemyCount)
            {
                int spawnIndex = Random.Range(0, _spawnPoints.Length);
                Instantiate(EnemyPrefab, _spawnPoints[spawnIndex], Quaternion.identity, EnemyParent);
            }
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(3f); // Adjust the spawn interval as needed
        }
    }
}
