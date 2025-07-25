using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 타입 enum
/// </summary>
public enum ItemType
{
    Passive = 0,        //패시브 아이템
    Usable = 1          //사용 아이템
}

/// <summary>
/// 아이템 등급 enum
/// </summary>
public enum ItemRarity
{
    Common = 0,         //일반 (회색)
    Uncommon = 1,       //희귀 (초록)
    Legendary = 2,      //전설 (주황)
    BossItem = 3        //보스 (빨강)
}

/// <summary>
/// 패시브 아이템 효과 타입
/// </summary>
public enum PassiveEffectType
{
    MaxHpBonus,         //최대 체력 증가
    DamageBonus,        //공격력 증가
    MoveSpeedBonus,     //이동속도 증가
    AttackSpeedBonus,   //공격속도 증가
    DefenseBonus,       //방어력 증가
    GoldBonus,          //골드 획득량 증가
    LuckBonus,          //운 (아이템 드롭률 증가)
    HpRegenBonus,       //체력 재생 증가
    SpecialEffect       //특수 효과
}

[CreateAssetMenu(menuName = "GameSettings/ItemData", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    [Header("----- 기본 정보 -----")]
    [SerializeField] ItemType _itemType;        //아이템 타입
    [SerializeField] ItemRarity _rarity;        //아이템 등급
    [SerializeField] string _itemName;          //아이템 이름
    [SerializeField] string _description;       //아이템 설명
    [SerializeField] Sprite _icon;              //아이템 아이콘

    [Header("----- 패시브 아이템 -----")]
    [SerializeField] PassiveEffectType _effectType;     //패시브 효과
    [SerializeField] float _effectValue;                //효과 수치
    [SerializeField] bool _isPercentage;                //퍼센트 증가인지 여부
    [SerializeField] string _specialEffectID;           //특수 효과 아이디

    [Header("----- 사용 아이템 -----")]
    [SerializeField] float _cooldown;           //사용 쿨타임
    [SerializeField] string _useEffectID;       //사용 효과 아이디

    //프로퍼티
    public ItemRarity Rarity => _rarity;
    public ItemType ItemType => _itemType;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public PassiveEffectType EffectType => _effectType;
    public float EffectValue => _effectValue;
    public bool IsPercentage => _isPercentage;
    public string SpecialEffectID => _specialEffectID;
    public float Cooldown => _cooldown;
    public string UseEffectID => _useEffectID;

    public Color GetRarityColor()
    {
        return _rarity switch
        {
            ItemRarity.Common => Color.white,
            ItemRarity.Uncommon => Color.green,
            ItemRarity.Legendary => Color.yellow,
            ItemRarity.BossItem => Color.red,
            _ => Color.white
        };
    }
}
