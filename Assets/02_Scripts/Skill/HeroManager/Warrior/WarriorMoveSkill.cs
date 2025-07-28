using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorMoveSkill : SkillBase
{
    private const float ARMOR_MULTIPLIER = 2f;
    private const float SPEED_MULTIPLIER = 1.5f;

    private HeroModel _heroModel;
    private float _originalArmor;
    private float _originalSpeed;
    private bool _isBuffApplied = false;

    public WarriorMoveSkill(SkillData data, Transform caster) : base(data, caster)
    {
        _heroModel = caster.GetComponent<HeroModel>();
    }

    protected override void ExecuteSkill()
    {
        Debug.Log("Warrior Move Skill: ��� �¼�!");

        if (_heroModel == null)
        {
            Debug.LogError("HeroModel�� ã�� �� �����ϴ�!");
            return;
        }

        // ���� ���� ����
        _originalArmor = _heroModel.Amor;
        _originalSpeed = _heroModel.MoveSpeed;

        // ���� ����
        ApplyBuff();

        Debug.Log($"���� {_originalArmor} �� {_originalArmor * ARMOR_MULTIPLIER}");
        Debug.Log($"�̵��ӵ� {_originalSpeed} �� {_originalSpeed * SPEED_MULTIPLIER}");
    }

    protected override void OnActiveUpdate(float deltaTime)
    {
        // ���� ȿ�� (�ʿ�� �߰� ����)
    }

    public override void OnSkillEnd()
    {
        // ���� ����
        RemoveBuff();

        Debug.Log("��� �¼� ����!");
        Debug.Log($"���� ����: {_originalArmor}");
        Debug.Log($"�̵��ӵ� ����: {_originalSpeed}");
    }

    private void ApplyBuff()
    {
        if (_isBuffApplied) return;

        // ���� ���� (���� ���� �����ŭ �߰�)
        float armorIncrease = _originalArmor * (ARMOR_MULTIPLIER - 1f);
        _heroModel.AddAmor(armorIncrease);

        // �̵��ӵ� ����
        float speedIncrease = _originalSpeed * (SPEED_MULTIPLIER - 1f);
        _heroModel.AddMoveSpeed(speedIncrease);

        _isBuffApplied = true;

        // �ð��� ȿ��
        if (_data.effectPrefab != null)
        {
            GameObject effect = Object.Instantiate(_data.effectPrefab, _caster.position, Quaternion.identity);
            effect.transform.SetParent(_caster);
            Object.Destroy(effect, _data.duration);
        }
    }

    private void RemoveBuff()
    {
        if (!_isBuffApplied) return;

        // ���� ���� (������Ų ��ŭ ����)
        float armorDecrease = _originalArmor * (ARMOR_MULTIPLIER - 1f);
        _heroModel.AddAmor(-armorDecrease);

        // �̵��ӵ� ����
        float speedDecrease = _originalSpeed * (SPEED_MULTIPLIER - 1f);
        _heroModel.AddMoveSpeed(-speedDecrease);

        _isBuffApplied = false;
    }

    // �̵� ��ų�� �̵��� �������� ����
    public override bool BlocksMovementInput => false;
}
