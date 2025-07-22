using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] EnemyModel _model;
    [SerializeField] Mover _mover;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Animator _animator;

    [Header("----- AI 설정 -----")]
    [SerializeField] float _detectionRange = 5f;        //감지 거리
    [SerializeField] float _attackRange = 1.5f;         //공격 사거리
    [SerializeField] float _attackCooldown = 2f;        //공격 쿨타임
    [SerializeField] LayerMask _targetLayer;            //공격 타겟의 레이어

    //[Header("----- 시각적 효과 -----")]
    //[SerializeField] GameObject _eliteEffectPrefab;     //엘리트 이펙트
    //[SerializeField] Color _eliteColor = Color.yellow;  //엘리트 이미지 색

    float _moveSpeed;
    Transform _target;          //공격 대상(타겟)
    float _lastAttackTime;
    bool _isDead = false;

    // 적 죽음 이벤트
    public event Action<Enemy> OnEnemyDeath;

    /// <summary>
    /// 체력 변화 이벤트
    /// </summary>
    public event Action<float, float> OnHpChanged
    {
        add => _model.OnHpChanged += value;
        remove => _model.OnHpChanged -= value;
    }

    /// <summary>
    /// 엘리트 상태 변화 이벤트 (생성 시에만 발생)
    /// </summary>
    public event Action<bool> OnEliteStatusChanged
    {
        add => _model.OnEliteStatusChanged += value;
        remove => _model.OnEliteStatusChanged -= value;
    }

    public bool IsElite => _model.IsElite;
    public EnemyModel Model => _model;


    public void Initialize(int level, bool forceElite = false)
    {
        _model.Initialize(level, forceElite);
        _model.OnDead += OnDead;

        // 이동 속도 설정
        if (_mover != null)
            _mover.SetSpeed(_model.MoveSpeed);

        // 엘리트라면 시각 효과 적용
        if (_model.IsElite)
        {
            Debug.LogWarning("엘리트 몹 출현!!");
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //ApplyEliteVisualEffects();
        }
    }

    private void Update()
    {
        if (_isDead) return;

        FindTarget();

        if (_target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, _target.position);

            if (distanceToTarget <= _attackRange)
            {
                // 공격 범위 내에 있으면 공격
                TryAttack();
            }
            else if (distanceToTarget <= _detectionRange)
            {
                // 감지 범위 내에 있으면 추적
                ChaseTarget();
            }
        }
    }

    /// <summary>
    /// 타겟 찾기
    /// </summary>
    void FindTarget()
    {
        if (_target == null)
        {
            Collider2D targetCollider = Physics2D.OverlapCircle(transform.position, _detectionRange, _targetLayer);
            if (targetCollider != null)
            {
                _target = targetCollider.transform;
            }
        }
    }

    /// <summary>
    /// 타겟 추적
    /// </summary>
    void ChaseTarget()
    {
        if (_target == null || _mover == null) return;

        Vector2 direction = (_target.position - transform.position).normalized;
        _mover.Move(direction);

        // 스프라이트 방향 설정
        if (_renderer != null)
        {
            _renderer.flipX = direction.x > 0;
        }
    }

    /// <summary>
    /// 공격 시도
    /// </summary>
    void TryAttack()
    {
        if (Time.time - _lastAttackTime >= _attackCooldown)
        {
            Attack();
            _lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// 공격 실행
    /// </summary>
    void Attack()
    {
        if (_target == null) return;

        IDamageable damageable = _target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            _model.Attack(damageable);

            // 공격 애니메이션 실행
            if (_animator != null)
                _animator.SetTrigger("Attack");
        }
    }

    /// <summary>
    /// 피격 처리
    /// </summary>
    /// <param name="damage">받을 데미지</param>
    public void TakeHit(float damage)
    {
        if (_isDead) return;

        _model.TakeHit(damage);

        // 피격 애니메이션 실행
        if (_animator != null)
            _animator.SetTrigger("Hit");
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    void OnDead()
    {
        _isDead = true;

        //경험치, 골드 보상 지급
        Hero hero = FindObjectOfType<Hero>();
        if (hero != null)
        {
            hero.AddExp(_model.GetExpRewardOnDeath());
            hero.AddGold(_model.GetGoldRewardOnDeath());
        }

        // 사망 이벤트 발행
        OnEnemyDeath?.Invoke(this);

        // 사망 애니메이션 실행 또는 즉시 제거
        if (_animator != null)
        {
            _animator.SetTrigger("Death");
            // 애니메이션 완료 후 제거하도록 코루틴 실행
            StartCoroutine(DestroyAfterDelay(2f));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 딜레이 후 오브젝트 제거하는 코루틴
    /// </summary>
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 디버그용 기즈모 그리기
    private void OnDrawGizmosSelected()
    {
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
