using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] string _specialEffectID;           //특수 효과 아이디

    [Header("----- 사용 아이템 -----")]
    [SerializeField] UsableItemEffectType _usableEffectType;
    [SerializeField] float _cooldown;           //사용 쿨타임
    [SerializeField] float _duration = 0f;      //지속 시간
    [SerializeField] float _range = 0f;         //범위

    [Header("----- 적용 효과 -----")]
    [SerializeField] float _effectValue;                //효과 수치
    [SerializeField] bool _isPercentage;                //퍼센트 증가인지 여부

    //프로퍼티
    public ItemRarity Rarity => _rarity;
    public ItemType ItemType => _itemType;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public PassiveEffectType EffectType => _effectType;
    public UsableItemEffectType UsableEffectType => _usableEffectType;
    public float EffectValue => _effectValue;
    public bool IsPercentage => _isPercentage;
    public string SpecialEffectID => _specialEffectID;
    public float Cooldown => _cooldown;
    public float Duration => _duration;
    public float Range => _range;

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
