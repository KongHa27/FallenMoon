using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("----- 총알 설정 -----")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private LayerMask _targetLayers = -1; // 충돌할 레이어
    [SerializeField] private bool _destroyOnHit = true;
    [SerializeField] private bool _horizontalOnly = true; // 수평 방향으로만 이동

    [Header("----- 디버그 설정 -----")]
    [SerializeField] private bool _enableDebugLog = true;

    [Header("----- 이펙트 설정 -----")]
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private GameObject _destroyEffect;

    private BattlerModel _shooter; // 총알을 발사한 대상
    private Vector3 _direction;
    private float _damage;
    private Rigidbody2D _rigid;
    private bool _isInitialized = false;

    private void Awake()
    {
        _enableDebugLog = false;

        _rigid = GetComponent<Rigidbody2D>();
        if (_rigid == null)
        {
            _rigid = gameObject.AddComponent<Rigidbody2D>();
        }

        // Collider2D 확인 및 추가
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            // 기본 Circle Collider 추가
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.1f;
            //Debug.Log($"Bullet {gameObject.name}: Collider2D가 없어서 CircleCollider2D를 추가했습니다.");
        }
        else
        {
            // 기존 Collider가 있다면 Trigger로 설정
            collider.isTrigger = true;
            //Debug.Log($"Bullet {gameObject.name}: 기존 Collider2D를 Trigger로 설정했습니다.");
        }

        // 물리 설정
        _rigid.gravityScale = 0f;
        _rigid.drag = 0f;
        _rigid.angularDrag = 0f;
    }

    /// <summary>
    /// 총알 초기화
    /// </summary>
    /// <param name="shooter">총알을 발사한 대상</param>
    /// <param name="direction">발사 방향</param>
    /// <param name="speed">총알 속도</param>
    /// <param name="lifetime">총알 생존 시간</param>
    public void Initialize(BattlerModel shooter, Vector3 direction, float speed, float lifetime)
    {
        _shooter = shooter;
        _direction = direction.normalized;

        // 수평 방향으로만 이동하도록 설정
        if (_horizontalOnly)
        {
            _direction = new Vector3(_direction.x > 0 ? 1f : -1f, 0f, 0f);
        }

        _speed = speed;
        _lifetime = lifetime;
        _damage = shooter != null ? shooter.GetDamage() : 0f;

        if (_enableDebugLog)
        {
            Debug.Log($"Bullet {gameObject.name} 초기화: 발사자={shooter?.gameObject.name}, 방향={_direction}, 속도={_speed}, 데미지={_damage}");
        }

        // 총알 방향에 따라 회전
        if (_direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // 총알 이동 시작
        _rigid.velocity = _direction * _speed;

        _isInitialized = true;

        // 생존 시간 후 파괴
        StartCoroutine(DestroyAfterTime(_lifetime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isInitialized)
        {
            if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: 초기화되지 않아서 충돌 무시");
            return;
        }

        if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: {other.gameObject.name}과 충돌 감지");

        // 발사자와 충돌하지 않도록 체크
        BattlerModel otherBattler = other.GetComponent<BattlerModel>();
        if (otherBattler == _shooter)
        {
            if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: 발사자와 충돌하여 무시");
            return;
        }

        // 레이어 체크
        int otherLayer = other.gameObject.layer;
        if ((_targetLayers.value & (1 << otherLayer)) == 0)
        {
            if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: 레이어 {otherLayer}는 타겟 레이어가 아님. 타겟 레이어: {_targetLayers.value}");
            return;
        }

        if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: {other.gameObject.name}에 유효한 충돌!");

        // IDamageable 인터페이스를 구현한 대상에게 데미지 적용
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: {other.gameObject.name}에게 {_damage} 데미지 적용");
            damageable.TakeHit(_damage);

            // 히트 이펙트 생성
            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, transform.position, Quaternion.identity);
            }

            // 총알 파괴
            if (_destroyOnHit)
            {
                DestroyBullet();
            }
        }
        else
        {
            if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: {other.gameObject.name}은 IDamageable 인터페이스가 없음");

            // IDamageable이 없어도 충돌 시 파괴되도록 (벽 등)
            if (_destroyOnHit)
            {
                DestroyBullet();
            }
        }
    }

    // OnCollisionEnter2D도 추가 (Rigidbody2D 충돌용)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_enableDebugLog) Debug.Log($"Bullet {gameObject.name}: {collision.gameObject.name}과 물리 충돌");

        // Trigger가 아닌 충돌도 처리
        OnTriggerEnter2D(collision.collider);
    }

    /// <summary>
    /// 시간 후 총알 파괴 코루틴
    /// </summary>
    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyBullet();
    }

    /// <summary>
    /// 총알 파괴
    /// </summary>
    private void DestroyBullet()
    {
        if (this == null) return;

        // 파괴 이펙트 생성
        if (_destroyEffect != null)
        {
            Instantiate(_destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 총알 속도 변경
    /// </summary>
    public void SetSpeed(float speed)
    {
        _speed = speed;
        if (_rigid != null && _isInitialized)
        {
            _rigid.velocity = _direction * _speed;
        }
    }

    /// <summary>
    /// 총알 방향 변경
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        _direction = direction.normalized;

        // 수평 방향으로만 이동하도록 설정
        if (_horizontalOnly)
        {
            _direction = new Vector3(_direction.x > 0 ? 1f : -1f, 0f, 0f);
        }

        if (_rigid != null && _isInitialized)
        {
            _rigid.velocity = _direction * _speed;

            // 회전 업데이트
            if (_direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    /// <summary>
    /// 총알 데미지 변경
    /// </summary>
    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    /// <summary>
    /// 총알 생존 시간 변경
    /// </summary>
    public void SetLifetime(float lifetime)
    {
        _lifetime = lifetime;
    }

    // 프로퍼티들
    public float Speed => _speed;
    public Vector3 Direction => _direction;
    public float Damage => _damage;
    public float Lifetime => _lifetime;
    public BattlerModel Shooter => _shooter;
}
