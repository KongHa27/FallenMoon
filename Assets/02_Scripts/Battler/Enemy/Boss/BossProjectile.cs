using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����ü Ŭ����
/// </summary>
public class BossProjectile : MonoBehaviour
{
    float _damage;
    float _speed;
    Vector3 _targetPosition;
    Vector3 _direction;

    public void Initialize(Vector3 targetPos, float damage, float speed)
    {
        _targetPosition = targetPos;
        _damage = damage;
        _speed = speed;
        _direction = (targetPos - transform.position).normalized;

        // ���� �ð� �� �ڵ� ����
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // ����ü �̵�
        transform.position += _direction * _speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable?.TakeHit(_damage);

            Debug.Log($"�ϼ� ��ô���� {other.name}���� {_damage} ������!");

            // ����ü ����
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            // ���� ������ ����
            Destroy(gameObject);
        }
    }
}
