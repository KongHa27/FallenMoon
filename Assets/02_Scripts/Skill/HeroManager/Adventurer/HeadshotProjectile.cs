using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadshotProjectile : MonoBehaviour
{
    [Header("발사체 설정")]
    [SerializeField] float _speed = 20f;
    [SerializeField] LayerMask _enemyLayerMask;

    Vector2 _direction;
    float _maxRange;
    float _damage;
    float _traveledDistance;
    Transform _caster;
    AdventurerSkill2 _skill2;
    bool _hasHit = false;

    /// <summary>
    /// 발사체 초기화
    /// </summary>
    public void Initialize(Vector2 direction, float maxRange, float damage, Transform caster, AdventurerSkill2 skill2)
    {
        _enemyLayerMask = LayerMask.GetMask("Enemy");
        _direction = direction.normalized;
        _maxRange = maxRange;
        _damage = damage;
        _caster = caster;
        _skill2 = skill2;
        _traveledDistance = 0f;
    }

    void Update()
    {
        // 이동
        float moveDistance = _speed * Time.deltaTime;
        transform.Translate(_direction * moveDistance);
        _traveledDistance += moveDistance;

        // 사거리 체크
        if (_traveledDistance >= _maxRange)
        {
            DestroyProjectile();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 시전자는 무시하고, 이미 명중한 경우도 무시
        if (other.transform == _caster || _hasHit) return;

        // Enemy 레이어만 확인
        if (((1 << other.gameObject.layer) & _enemyLayerMask) == 0) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            _hasHit = true;

            // 적의 현재 체력 확인 (처치 여부 판단용)
            bool willKill = CheckIfWillKill(other.gameObject);

            // 데미지 적용
            damageable.TakeHit(_damage);

            Debug.Log($"헤드샷이 {other.name}에게 {_damage} 데미지를 입혔습니다!");

            // 적을 처치했다면 쿨다운 리셋
            if (willKill)
            {
                _skill2.OnEnemyKilledByHeadshot();
            }

            // 명중 이펙트
            CreateHeadshotEffect(other.transform.position);

            // 발사체 파괴
            DestroyProjectile();
        }
    }

    /// <summary>
    /// 이 공격으로 적이 죽을지 확인하는 함수
    /// </summary>
    bool CheckIfWillKill(GameObject target)
    {
        // BattlerModel에서 현재 체력 확인
        BattlerModel targetModel = target.GetComponent<BattlerModel>();
        if (targetModel != null)
        {
            float currentHp = GetCurrentHp(targetModel);
            return currentHp <= _damage;
        }

        // EnemyModel이 있는 경우도 확인
        EnemyModel enemyModel = target.GetComponent<EnemyModel>();
        if (enemyModel != null)
        {
            float currentHp = GetCurrentHp(enemyModel);
            return currentHp <= _damage;
        }

        return false;
    }

    /// <summary>
    /// BattlerModel에서 현재 체력을 가져오는 함수
    /// (리플렉션을 사용하여 private 필드에 접근)
    /// </summary>
    float GetCurrentHp(BattlerModel model)
    {
        var field = typeof(BattlerModel).GetField("_curHp",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return (float)field.GetValue(model);
        }
        return 999f; // 접근 실패 시 높은 값 반환 (처치 불가로 간주)
    }

    /// <summary>
    /// 명중 이펙트 생성
    /// </summary>
    void CreateHeadshotEffect(Vector3 position)
    {
        // 파티클 이펙트
        GameObject hitEffect = new GameObject("HeadshotHitEffect");
        hitEffect.transform.position = position;

        ParticleSystem particles = hitEffect.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = Color.red;
        main.maxParticles = 30;
        main.startLifetime = 0.8f;
        main.startSpeed = 6f;

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;

        var emission = particles.emission;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0.0f, 30)
        });

        // 이펙트 자동 삭제
        Destroy(hitEffect, 2f);
    }

    /// <summary>
    /// 발사체 파괴
    /// </summary>
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 기즈모로 사거리 표시 (디버그용)
    /// </summary>
    void OnDrawGizmos()
    {
        if (_caster != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_caster.position, _direction * _maxRange);
        }
    }
}
