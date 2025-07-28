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
        Debug.Log("Warrior Skill 1: �� �ֵθ���!");

        // ���� ���� �� ���� ã��
        Vector3 attackPosition = _caster.position;
        Vector3 forwardDirection = _caster.right; // 2D������ right�� forward ����

        // ��ä�� ������� �� Ž��
        List<IDamageable> targets = FindEnemiesInCone(attackPosition, forwardDirection, _data.range, 120f);

        // �ִ� 3������� ����
        int attackCount = Mathf.Min(targets.Count, MAX_TARGETS);

        for (int i = 0; i < attackCount; i++)
        {
            AttackTarget(targets[i]);
        }

        // ����Ʈ ����
        CreateSlashEffect(attackPosition, forwardDirection);

        Debug.Log($"�� �ֵθ���� {attackCount}���� ���� �����߽��ϴ�!");
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

        // �Ÿ������� ����
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
