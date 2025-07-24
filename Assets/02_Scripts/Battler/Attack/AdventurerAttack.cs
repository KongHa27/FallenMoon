using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 모험가 기본 공격 클래스
/// 두 발을 빠르게 연사
/// </summary>
public class AdventurerAttack : AttackSystem
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] SpriteRenderer _spriteRenderer;

    [Header("----- 공격 설정 -----")]
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _firePoint;
    [SerializeField] float _attackRange = 10f;
    [SerializeField] float _attackCooldown = 1f;
    [SerializeField] float _burstDelay = 0.1f; // 연속 공격 간 딜레이
    [SerializeField] int _burstCount = 2; // 연속 공격 횟수

    [Header("----- 총알 설정 -----")]
    [SerializeField] float _bulletSpeed = 15f;
    [SerializeField] float _bulletLifetime = 3f;

    [Header("----- UI 설정 -----")]
    [SerializeField] Sprite _attackIcon;
    [SerializeField] string _attackName;
    [SerializeField] string _attackDesc;

    private float _lastAttackTime;
    private bool _isAttacking;

    public float AttackCooldown => _attackCooldown;
    public Sprite AttackIcon => _attackIcon;
    public string AttackName => _attackName;
    public string AttackDesc => _attackDesc;


    public override void Initialize(BattlerModel model, Transform transform)
    {
        base.Initialize(model, transform);

        // FirePoint가 설정되지 않았다면 자동으로 생성
        if (_firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(_transform);
            firePointObj.transform.localPosition = Vector3.right * 0.5f; // 캐릭터 앞쪽에 위치
            _firePoint = firePointObj.transform;
        }
    }

    public override void PerformAttack(IDamageable target = null)
    {
        if (!CanAttack()) return;

        _lastAttackTime = Time.time;
        _isAttacking = true;

        // 연속 공격 코루틴 시작
        StartCoroutine(BurstAttackCoroutine());
    }

    /// <summary>
    /// 연속 공격 코루틴
    /// </summary>
    private IEnumerator BurstAttackCoroutine()
    {
        for (int i = 0; i < _burstCount; i++)
        {
            FireBullet();

            // 마지막 공격이 아니라면 딜레이 적용
            if (i < _burstCount - 1)
            {
                yield return new WaitForSeconds(_burstDelay);
            }
        }

        _isAttacking = false;
    }

    /// <summary>
    /// 총알 발사
    /// </summary>
    private void FireBullet()
    {
        if (_bulletPrefab == null || _firePoint == null) return;

        // 총알 생성
        GameObject bulletObj = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            // 바라보는 방향 계산
            Vector3 direction = GetFacingDirection();

            // 총알 초기화
            bullet.Initialize(_model.GetComponent<BattlerModel>(), direction, _bulletSpeed, _bulletLifetime);
        }
    }

    /// <summary>
    /// 바라보는 방향 계산
    /// </summary>
    private Vector3 GetFacingDirection()
    {
        // SpriteRenderer의 flipX를 기준으로 수평 방향만 판단
        if (_spriteRenderer != null)
        {
            return _spriteRenderer.flipX ? Vector3.left : Vector3.right;
        }

        // SpriteRenderer가 없다면 transform의 scale을 기준으로 판단
        return _transform.localScale.x >= 0 ? Vector3.right : Vector3.left;
    }

    public override bool CanAttack()
    {
        return !_isAttacking && Time.time >= _lastAttackTime + _attackCooldown;
    }

    public override float GetAttackRange()
    {
        return _attackRange;
    }

    /// <summary>
    /// 공격 쿨다운 남은 시간 반환
    /// </summary>
    public float GetCooldownRemaining()
    {
        return Mathf.Max(0, (_lastAttackTime + _attackCooldown) - Time.time);
    }

    /// <summary>
    /// 현재 공격 중인지 확인
    /// </summary>
    public bool IsAttacking => _isAttacking;
}
