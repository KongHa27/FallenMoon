using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorSkill2 : SkillBase
{
    private const int STRIKE_COUNT = 6;
    private const float DAMAGE_MULTIPLIER = 1.1f;
    private const float STRIKE_INTERVAL = 0.2f;

    private IDamageable _target;
    private int _currentStrike;
    private float _strikeTimer;
    private int _originalLayer;
    private LayerMask _enemyLayerMask = -1;

    public WarriorSkill2(SkillData data, Transform caster) : base(data, caster)
    {
    }

    protected override void ExecuteSkill()
    {
        Debug.Log("Warrior Skill 2: ���� ����!");

        // ���� ����� �� ã��
        _target = FindNearestEnemy();

        if (_target == null)
        {
            Debug.Log("������ ���� �����ϴ�!");
            return;
        }

        // ���� ���·� ����
        GameObject casterObject = _caster.gameObject;
        _originalLayer = casterObject.layer;
        casterObject.layer = LayerMask.NameToLayer("Invincibility");

        // ���� ���� ����
        _currentStrike = 0;
        _strikeTimer = 0f;

        Debug.Log("���� ���� Ȱ��ȭ!");
    }

    protected override void OnActiveUpdate(float deltaTime)
    {
        if (_target == null) return;

        _strikeTimer += deltaTime;

        // ���� ���ݸ��� Ÿ��
        if (_strikeTimer >= STRIKE_INTERVAL && _currentStrike < STRIKE_COUNT)
        {
            PerformStrike();
            _strikeTimer = 0f;
            _currentStrike++;
        }
    }

    public override void OnSkillEnd()
    {
        // ���� ���� ����
        if (_caster != null)
        {
            _caster.gameObject.layer = _originalLayer;
            Debug.Log("���� ���� ����!");
        }

        _target = null;
        _currentStrike = 0;
    }

    private IDamageable FindNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_caster.position, _data.range, _enemyLayerMask);

        IDamageable nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.transform == _caster) continue;

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float distance = Vector2.Distance(_caster.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = damageable;
                }
            }
        }

        return nearestEnemy;
    }

    private void PerformStrike()
    {
        BattlerModel casterModel = _caster.GetComponent<BattlerModel>();
        if (casterModel != null && _target != null)
        {
            float damage = casterModel.GetDamage() * DAMAGE_MULTIPLIER;
            _target.TakeHit(damage);

            // Ÿ�� ����Ʈ
            if (_data.effectPrefab != null)
            {
                MonoBehaviour targetMono = _target as MonoBehaviour;
                if (targetMono != null)
                {
                    GameObject effect = Object.Instantiate(_data.effectPrefab, targetMono.transform.position, Quaternion.identity);
                    Object.Destroy(effect, 0.5f);
                }
            }

            Debug.Log($"���� ���� {_currentStrike + 1}/{STRIKE_COUNT}: {damage} ������!");
        }
    }
}
