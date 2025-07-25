using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("----- UI 참조 -----")]
    [SerializeField] Transform _passiveItemParent;      // 패시브 아이템들이 들어갈 부모
    [SerializeField] GameObject _passiveItemSlotPrefab; // 패시브 아이템 슬롯 프리팹
    [SerializeField] Image _usableItemIcon;             // 사용 아이템 아이콘
    [SerializeField] TextMeshProUGUI _usableItemCooldownText; // 사용 아이템 쿨타임 텍스트

    PlayerInventory _inventory;
    List<PassiveItemSlotUI> _passiveSlotUIs = new List<PassiveItemSlotUI>();

    /// <summary>
    /// UI 초기화
    /// </summary>
    public void Initialize(PlayerInventory inventory)
    {
        _inventory = inventory;

        // 이벤트 구독
        _inventory.OnPassiveItemAdded += OnPassiveItemAdded;
        _inventory.OnUsableItemEquipped += OnUsableItemEquipped;
        _inventory.OnUsableItemCooldownChanged += OnUsableItemCooldownChanged;
    }

    /// <summary>
    /// 패시브 아이템 추가 시 UI 업데이트
    /// </summary>
    void OnPassiveItemAdded(ItemData itemData, int count)
    {
        // 기존 슬롯 찾기
        PassiveItemSlotUI existingSlot = _passiveSlotUIs.Find(slot => slot.ItemData == itemData);

        if (existingSlot != null)
        {
            // 기존 슬롯 개수 업데이트
            existingSlot.UpdateCount(count);
        }
        else
        {
            // 새로운 슬롯 생성
            CreateNewPassiveSlot(itemData, count);
        }
    }

    /// <summary>
    /// 새로운 패시브 아이템 슬롯 생성
    /// </summary>
    void CreateNewPassiveSlot(ItemData itemData, int count)
    {
        if (_passiveItemSlotPrefab != null && _passiveItemParent != null)
        {
            GameObject slotObj = Instantiate(_passiveItemSlotPrefab, _passiveItemParent);
            PassiveItemSlotUI slotUI = slotObj.GetComponent<PassiveItemSlotUI>();

            if (slotUI != null)
            {
                slotUI.Initialize(itemData, count);
                _passiveSlotUIs.Add(slotUI);
            }
        }
    }

    /// <summary>
    /// 사용 아이템 장착 시 UI 업데이트
    /// </summary>
    void OnUsableItemEquipped(ItemData itemData)
    {
        if (_usableItemIcon != null)
        {
            _usableItemIcon.sprite = itemData.Icon;
            _usableItemIcon.color = itemData.GetRarityColor();
        }
    }

    /// <summary>
    /// 사용 아이템 쿨타임 UI 업데이트
    /// </summary>
    void OnUsableItemCooldownChanged(float remainingCooldown, float maxCooldown)
    {
        if (_usableItemCooldownText != null)
        {
            if (remainingCooldown > 0)
            {
                _usableItemCooldownText.text = Mathf.Ceil(remainingCooldown).ToString();
            }
            else
            {
                _usableItemCooldownText.text = "";
            }
        }
    }
}
