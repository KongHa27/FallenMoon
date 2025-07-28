using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorSkill1 : SkillBase
{
    private const int MAX_TARGETS = 3;
    private const float DAMAGE_MULTIPLIER = 1.3f;

    [SerializeField] private LayerMask _enemyLayerMask = -1;

    public WarriorSkill1(SkillData data, Transform caster) : base(data, caster)
    {
    }

    protected override void ExecuteSkill()
    {
        Debug.Log("Warrior Skill 1: 검 휘두르기!");

        // 전방 범위 내 적들 찾기
        Vector3 attackPosition = _caster.position;
        Vector3 forwardDirection = _caster.right; // 2D에서는 right가 forward 방향

        // 부채꼴 모양으로 적 탐지
        List<IDamageable> targets = FindEnemiesInCone(attackPosition, forwardDirection, _data.range, 120f);

        // 최대 3명까지만 공격
        int attackCount = Mathf.Min(targets.Count, MAX_TARGETS);

        for (int i = 0; i < attackCount; i++)
        {
            AttackTarget(targets[i]);
        }

        // 이펙트 생성
        CreateSlashEffect(attackPosition, forwardDirection);

        Debug.Log($"검 휘두르기로 {attackCount}명의 적을 공격했습니다!");
    }

    private List<IDamageable> FindEnemiesInCone(Vector3 origin, Vector3 direction, float range, float angle)
    {
        List<IDamageable> enemies = new List<IDamageable>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, range, _enemyLayerMask);

        foreach (var collider in colliders)
        {
            if (collider.transform == _caster) continue;

            Vector3 dirToTarget = (collider.transform.position - origin).normalized;
            float angleToTarget = Vector3.Angle(direction, dirToTarget);

            if (angleToTarget <= angle / 2f)
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    enemies.Add(damageable);
                }
            }
        }

        // 거리순으로 정렬
        enemies.Sort((a, b) =>
        {
            float distA = Vector3.Distance(origin, ((MonoBehaviour)a).transform.position);
            float distB = Vector3.Distance(origin, ((MonoBehaviour)b).transform.position);
            return distA.CompareTo(distB);
        });

        return enemies;
    }

    private void AttackTarget(IDamageable target)
    {
        BattlerModel casterModel = _caster.GetComponent<BattlerModel>();
        if (casterModel != null)
        {
            float damage = casterModel.GetDamage() * DAMAGE_MULTIPLIER;
            target.TakeHit(damage);
        }
    }

    private void CreateSlashEffect(Vector3 position, Vector3 direction)
    {
        if (_data.effectPrefab != null)
        {
            GameObject effect = Object.Instantiate(_data.effectPrefab, position, Quaternion.LookRotation(Vector3.forward, direction));
            Object.Destroy(effect, 1f);
        }
    }
}
