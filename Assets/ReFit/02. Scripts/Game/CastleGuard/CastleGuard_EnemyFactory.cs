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
        new Vector3(-3.25f, 0, 15),
        new Vector3(-1.75f, 0, 15),
        new Vector3(-0.25f, 0, 15),
        new Vector3(1.25f, 0, 15),
        new Vector3(2.75f, 0, 15)
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
