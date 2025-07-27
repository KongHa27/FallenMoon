using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 아이템들의 효과를 정의하고 관리하는 클래스
/// </summary>
public class UsableItemEffectFactory : MonoBehaviour
{
    private Dictionary<UsableItemEffectType, IUsableItemEffect> _effectMap;

    void Awake()
    {
        InitializeEffectMap();
    }

    /// <summary>
    /// 효과 맵 초기화
    /// </summary>
    void InitializeEffectMap()
    {
        _effectMap = new Dictionary<UsableItemEffectType, IUsableItemEffect>
        {
            {UsableItemEffectType.Heal, new HealEffect() },
            {UsableItemEffectType.Bomb, new BombEffect() },
        };
    }

    /// <summary>
    /// 효과 적용
    /// </summary>
    public void ApplyEffect(UsableItemEffectType effectType, Hero hero, ItemData itemData)
    {
        if (_effectMap.ContainsKey(effectType))
        {
            _effectMap[effectType].ApplyEffect(hero, itemData);
        }
        else
        {
            Debug.LogWarning($"정의되지 않은 사용 아이템 효과: {effectType}");
        }
    }
}

/// <summary>
/// 힐 효과
/// </summary>
public class HealEffect : IUsableItemEffect
{
    public void ApplyEffect(Hero hero, ItemData itemData)
    {
        HeroModel model = hero.GetComponentInChildren<HeroModel>();

        model.Heal(itemData.EffectValue);
    }
}

/// <summary>
/// 광역 대미지 효과
/// </summary>
public class BombEffect : IUsableItemEffect
{
    public void ApplyEffect(Hero hero, ItemData itemData)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            hero.transform.position, itemData.Range, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            damageable.TakeHit(itemData.EffectValue);
        }
    }
}

public class LightRecharge : IUsableItemEffect
{
    public void ApplyEffect(Hero hero, ItemData itemData)
    {
        hero.AddLightGauge(itemData.EffectValue);
    }
}
