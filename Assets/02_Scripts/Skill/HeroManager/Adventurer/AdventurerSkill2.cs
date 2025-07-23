using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모험가 스킬2 "헤드샷" 클래스
/// </summary>
public class AdventurerSkill2 : SkillBase
{
    SkillManager _skillManager;

    public AdventurerSkill2(SkillData data, Transform caster) : base(data, caster)
    {
        _skillManager = caster.GetComponent<SkillManager>();
    }

    protected override void ExecuteSkill()
    {
        Debug.Log($"!!!스킬 2 \"{_data.skillName}\" 사용!!!");

        // 헤드샷 발사체 생성 및 발사
        FireHeadshotProjectile();

        // 사운드 재생
        if (_data.soundClip != null)
        {
            AudioSource.PlayClipAtPoint(_data.soundClip, _caster.position);
        }
    }

    /// <summary>
    /// 헤드샷 발사체를 발사하는 함수
    /// </summary>
    void FireHeadshotProjectile()
    {
        // 발사 방향 결정 (캐릭터가 바라보는 방향)
        SpriteRenderer renderer = _caster.GetComponentInChildren<SpriteRenderer>();
        Vector2 direction = renderer.flipX ? Vector2.left : Vector2.right;

        // 발사체 생성
        GameObject projectile = GameObject.Instantiate(_data.effectPrefab);
        if (projectile != null)
        {
            // 발사체 위치 설정 (약간 높은 위치에서 발사)
            projectile.transform.position = _caster.position + Vector3.up * 0.7f;

            // 발사체 컴포넌트 설정
            HeadshotProjectile projScript = projectile.GetComponent<HeadshotProjectile>();
            if (projScript == null)
                projScript = projectile.AddComponent<HeadshotProjectile>();

            projScript.Initialize(direction, _data.range, GetDamage(), _caster, this);
        }
    }

    /// <summary>
    /// 스킬 데미지 계산 (500%)
    /// </summary>
    float GetDamage()
    {
        BattlerModel casterModel = _caster.GetComponent<BattlerModel>();
        if (casterModel != null)
        {
            return casterModel.GetDamage() * _data.damage; // _data.damage = 5.0f (500%)
        }
        return _data.damage * 100f; // 기본값
    }

    /// <summary>
    /// 적 처치 시 모든 스킬 쿨다운 리셋
    /// </summary>
    public void OnEnemyKilledByHeadshot()
    {
        Debug.Log("헤드샷으로 적 처치! 모든 스킬 쿨다운 초기화!");

        if (_skillManager != null)
        {
            ResetAllSkillCooldowns();
        }
    }

    /// <summary>
    /// 모든 스킬의 쿨다운을 초기화하는 함수 (개선된 버전)
    /// </summary>
    void ResetAllSkillCooldowns()
    {
        // 스킬1 쿨다운 리셋
        SkillBase skill1 = _skillManager.GetSkill(SkillManager.SkillType.Skill1);
        if (skill1 != null)
        {
            skill1.State.ResetCooldown();
        }

        // 스킬2 쿨다운 리셋 (자기 자신)
        SkillBase skill2 = _skillManager.GetSkill(SkillManager.SkillType.Skill2);
        if (skill2 != null)
        {
            skill2.State.ResetCooldown();
        }

        // 이동스킬 쿨다운 리셋
        SkillBase moveSkill = _skillManager.GetSkill(SkillManager.SkillType.MoveSkill);
        if (moveSkill != null)
        {
            moveSkill.State.ResetCooldown();
        }
    }
}
