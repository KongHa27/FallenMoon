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
        Debug.Log("Warrior Move Skill: 방어 태세!");

        if (_heroModel == null)
        {
            Debug.LogError("HeroModel을 찾을 수 없습니다!");
            return;
        }

        // 원본 스탯 저장
        _originalArmor = _heroModel.Amor;
        _originalSpeed = _heroModel.MoveSpeed;

        // 버프 적용
        ApplyBuff();

        Debug.Log($"방어력 {_originalArmor} → {_originalArmor * ARMOR_MULTIPLIER}");
        Debug.Log($"이동속도 {_originalSpeed} → {_originalSpeed * SPEED_MULTIPLIER}");
    }

    protected override void OnActiveUpdate(float deltaTime)
    {
        // 지속 효과 (필요시 추가 구현)
    }

    public override void OnSkillEnd()
    {
        // 버프 해제
        RemoveBuff();

        Debug.Log("방어 태세 해제!");
        Debug.Log($"방어력 복구: {_originalArmor}");
        Debug.Log($"이동속도 복구: {_originalSpeed}");
    }

    private void ApplyBuff()
    {
        if (_isBuffApplied) return;

        // 방어력 증가 (원본 값의 배수만큼 추가)
        float armorIncrease = _originalArmor * (ARMOR_MULTIPLIER - 1f);
        _heroModel.AddAmor(armorIncrease);

        // 이동속도 증가
        float speedIncrease = _originalSpeed * (SPEED_MULTIPLIER - 1f);
        _heroModel.AddMoveSpeed(speedIncrease);

        _isBuffApplied = true;

        // 시각적 효과
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

        // 방어력 복구 (증가시킨 만큼 빼기)
        float armorDecrease = _originalArmor * (ARMOR_MULTIPLIER - 1f);
        _heroModel.AddAmor(-armorDecrease);

        // 이동속도 복구
        float speedDecrease = _originalSpeed * (SPEED_MULTIPLIER - 1f);
        _heroModel.AddMoveSpeed(-speedDecrease);

        _isBuffApplied = false;
    }

    // 이동 스킬은 이동을 차단하지 않음
    public override bool BlocksMovementInput => false;
}
