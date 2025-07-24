using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보스 몬스터 클래스
/// </summary>
public class Boss : MonoBehaviour
{
    [Header("----- 보스 설정 -----")]
    [SerializeField] int _hp = 100;

    int _curHp;

    public event Action OnBossDead;

    public int Hp => _hp;
    public int CurHp => _curHp;

    void Start()
    {
        _curHp = _hp;
    }

    public void TakeDamage(int damage)
    {
        _curHp -= damage;
        Debug.Log($"보스 체력: {_curHp}/{_hp}");

        if (_curHp <= 0)
        {
            DefeatBoss();
        }
    }

    void DefeatBoss()
    {
        Debug.Log("보스 처치됨!");
        OnBossDead?.Invoke();

        // 보스 제거 (이펙트 등 추가 가능)
        StartCoroutine(DestroyBossAfterDelay(1f));
    }

    System.Collections.IEnumerator DestroyBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 플레이어와 충돌 시 데미지 (예시)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어 공격으로 데미지 받는 로직
            TakeDamage(20); // 예시
        }
    }
}
