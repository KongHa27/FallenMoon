using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보스 투사체 클래스
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

        // 일정 시간 후 자동 제거
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // 투사체 이동
        transform.position += _direction * _speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable?.TakeHit(_damage);

            Debug.Log($"암석 투척으로 {other.name}에게 {_damage} 데미지!");

            // 투사체 제거
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            // 땅에 닿으면 제거
            Destroy(gameObject);
        }
    }
}
