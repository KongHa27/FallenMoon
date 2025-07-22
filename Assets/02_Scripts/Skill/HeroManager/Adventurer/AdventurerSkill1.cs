using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모험가 스킬1 "관통샷" 클래스
/// </summary>
public class AdventurerSkill1 : SkillBase
{
    private LayerMask _enemyLayerMask = LayerMask.GetMask("Enemy");

    public AdventurerSkill1(SkillData data, Transform caster) : base(data, caster)
    {
    }

    protected override void ExecuteSkill()
    {
        Debug.Log($"!!!스킬 1 \"{_data.skillName}\" 사용!!!");

        // 발사 방향 결정 (캐릭터가 바라보는 방향)
        SpriteRenderer renderer = _caster.GetComponentInChildren<SpriteRenderer>();
        Vector2 fireDirection = renderer.flipX ? Vector2.left : Vector2.right;

        // 관통 발사체 생성 및 발사
        FirePenetratingProjectile(fireDirection);

        // 이펙트 생성
        if (_data.effectPrefab != null)
        {
            GameObject effect = Object.Instantiate(_data.effectPrefab, _caster.position, Quaternion.identity);
            Object.Destroy(effect, 2f);
        }

        // 사운드 재생
        if (_data.soundClip != null)
        {
            AudioSource.PlayClipAtPoint(_data.soundClip, _caster.position);
        }
    }

    /// <summary>
    /// 관통 발사체를 발사하는 함수
    /// </summary>
    /// <param name="direction">발사 방향</param>
    private void FirePenetratingProjectile(Vector2 direction)
    {
        // 레이캐스트로 관통 공격 구현
        Vector2 startPos = _caster.position;
        startPos.y = 0.5f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, _data.range, _enemyLayerMask);

        // 거리순으로 정렬
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // 모든 적에게 관통 대미지 적용
        foreach (RaycastHit2D hit in hits)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // 230% 피해
                BattlerModel casterModel = _caster.GetComponent<BattlerModel>();
                if (casterModel != null)
                {
                    float finalDamage = casterModel.GetDamage() * _data.damage;
                    damageable.TakeHit(finalDamage);

                    Debug.Log($"관통상으로 {hit.collider.name}에게 {finalDamage} 피해!");
                }
            }
        }

        // 시각적 효과를 위한 라인 그리기 (디버그용)
        Debug.DrawRay(startPos, direction * _data.range, Color.red, 1f);
    }
}
