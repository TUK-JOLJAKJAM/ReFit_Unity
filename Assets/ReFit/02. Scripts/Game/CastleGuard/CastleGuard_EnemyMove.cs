using NUnit.Framework.Constraints;
using UnityEngine;

public class CastleGuard_EnemyMove : MonoBehaviour
{
    bool _direction = true; // true : right, false : left
    public float speed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        MoveToDirection();
        DeleteEnemy();
    }


    private void MoveToDirection()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }

    void DeleteEnemy()
    {
        if(transform.position.z < -3.0f)
        {
            Destroy(gameObject);
        }
    }
}
