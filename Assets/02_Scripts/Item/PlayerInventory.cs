using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveItemSlot
{
    public ItemData itemData;
    public int count;

    public PassiveItemSlot(ItemData data)
    {
        itemData = data;
        count = 1;
    }
}

public class PlayerInventory : MonoBehaviour
{
    [Header("----- 인벤토리 설정 -----")]
    [SerializeField] List<PassiveItemSlot> _passiveItems = new List<PassiveItemSlot>();
    [SerializeField] ItemData _equippedUsableItem;
    [SerializeField] float _usableItemCooldown;

    [Header("----- UI 참조 -----")]
    [SerializeField] InventoryUI _inventoryUI;

    // 이벤트
    public event Action<ItemData, int> OnPassiveItemAdded;
    public event Action<ItemData> OnUsableItemEquipped;
    public event Action<float, float> OnUsableItemCooldownChanged;

    Hero _hero;
    float _lastUseTime;

    public ItemData EquippedUsableItem => _equippedUsableItem;
    public bool CanUseItem => Time.time - _lastUseTime >= _usableItemCooldown;

    void Start()
    {
        _hero = GetComponent<Hero>();
        if (_inventoryUI != null)
        {
            _inventoryUI.Initialize(this);
        }
    }

    void Update()
    {
        // 사용 아이템 쿨타임 UI 업데이트
        if (_equippedUsableItem != null)
        {
            float remainingCooldown = Mathf.Max(0, _usableItemCooldown - (Time.time - _lastUseTime));
            OnUsableItemCooldownChanged?.Invoke(remainingCooldown, _usableItemCooldown);
        }
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    public void AddItem(ItemData itemData)
    {
        if (itemData.ItemType == ItemType.Passive)
        {
            AddPassiveItem(itemData);
        }
        else if (itemData.ItemType == ItemType.Usable)
        {
            EquipUsableItem(itemData);
        }
    }

    /// <summary>
    /// 패시브 아이템 추가
    /// </summary>
    void AddPassiveItem(ItemData itemData)
    {
        // 기존에 같은 아이템이 있는지 확인
        PassiveItemSlot existingSlot = _passiveItems.Find(slot => slot.itemData == itemData);

        if (existingSlot != null)
        {
            // 중첩 가능 - 개수 증가
            existingSlot.count++;
        }
        else
        {
            // 새로운 아이템 추가
            _passiveItems.Add(new PassiveItemSlot(itemData));
        }

        // 효과 적용
        ApplyPassiveItemEffect(itemData);

        // 이벤트 발행
        int currentCount = existingSlot?.count ?? 1;
        OnPassiveItemAdded?.Invoke(itemData, currentCount);

        Debug.Log($"패시브 아이템 획득: {itemData.ItemName} (총 {currentCount}개)");
    }

    /// <summary>
    /// 사용 아이템 장착
    /// </summary>
    void EquipUsableItem(ItemData itemData)
    {
        _equippedUsableItem = itemData;
        _usableItemCooldown = itemData.Cooldown;
        _lastUseTime = -_usableItemCooldown; // 즉시 사용 가능하도록

        OnUsableItemEquipped?.Invoke(itemData);
        Debug.Log($"사용 아이템 장착: {itemData.ItemName}");
    }

    /// <summary>
    /// 사용 아이템 사용
    /// </summary>
    public void UseEquippedItem()
    {
        if (_equippedUsableItem != null && CanUseItem)
        {
            ApplyUsableItemEffect(_equippedUsableItem);
            _lastUseTime = Time.time;
        }
    }

    /// <summary>
    /// 패시브 아이템 효과 적용
    /// </summary>
    void ApplyPassiveItemEffect(ItemData itemData)
    {
        if (_hero == null) return;

        float effectValue = itemData.EffectValue;

        switch (itemData.EffectType)
        {
            case PassiveEffectType.MaxHpBonus:
                _hero.GetComponent<HeroModel>().AddMaxHp(effectValue);
                break;
            case PassiveEffectType.MoveSpeedBonus:
                _hero.GetComponent<HeroModel>().AddMoveSpeed(effectValue);
                break;
            case PassiveEffectType.DamageBonus:
                // 공격력 증가 로직 (BattlerModel에 추가 필요)
                break;
            case PassiveEffectType.SpecialEffect:
                // 특수 효과는 별도 처리
                break;
        }
    }

    /// <summary>
    /// 사용 아이템 효과 적용
    /// </summary>
    void ApplyUsableItemEffect(ItemData itemData)
    {
        if (_hero == null) return;

        switch (itemData.UseEffectID)
        {
            case "Heal":
                _hero.GetComponent<HeroModel>().Heal(itemData.EffectValue);
                break;
            case "SpeedBoost":
                StartCoroutine(TemporarySpeedBoost(itemData.EffectValue, 5f));
                break;
                // 추가 사용 아이템 효과들...
        }
    }

    /// <summary>
    /// 임시 속도 증가 코루틴
    /// </summary>
    IEnumerator TemporarySpeedBoost(float speedBonus, float duration)
    {
        _hero.GetComponent<HeroModel>().AddMoveSpeed(speedBonus);
        yield return new WaitForSeconds(duration);
        _hero.GetComponent<HeroModel>().AddMoveSpeed(-speedBonus);
    }

    /// <summary>
    /// 특수 효과 보유 여부 확인
    /// </summary>
    public bool HasSpecialEffect(string effectID)
    {
        return _passiveItems.Exists(slot =>
            slot.itemData.EffectType == PassiveEffectType.SpecialEffect &&
            slot.itemData.SpecialEffectID == effectID);
    }

    /// <summary>
    /// 패시브 아이템 개수 반환
    /// </summary>
    public int GetPassiveItemCount(ItemData itemData)
    {
        PassiveItemSlot slot = _passiveItems.Find(s => s.itemData == itemData);
        return slot?.count ?? 0;
    }

    /// <summary>
    /// 모든 패시브 아이템 반환
    /// </summary>
    public List<PassiveItemSlot> GetAllPassiveItems()
    {
        return new List<PassiveItemSlot>(_passiveItems);
    }
}
