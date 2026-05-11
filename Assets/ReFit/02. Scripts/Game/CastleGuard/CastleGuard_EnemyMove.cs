using NUnit.Framework.Constraints;
using UnityEngine;

public class CastleGuard_EnemyMove : MonoBehaviour
{
    bool _direction = true; // true : right, false : left
    public float speed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetDirection();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToDirection();
        DeleteEnemy();
    }

    void SetDirection()
    {
        if (transform.position.x > 0)
        {
            _direction = false;
        }
        else
        {
            _direction = true;
        }
    }

    private void MoveToDirection()
    {
        if (_direction)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }

    void DeleteEnemy()
    {
        if(transform.position.x > 15 || transform.position.x < -15)
        {
            Destroy(gameObject);
        }
    }
}
