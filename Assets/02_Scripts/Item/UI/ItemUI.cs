using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [Header("----- 패시브 아이템 -----")]
    [SerializeField] Transform[] _passiveItemContainers;      // 패시브 아이템들이 들어갈 컨테이너 배열
    [SerializeField] GameObject _passiveItemSlotPrefab;       // 패시브 아이템 슬롯 프리팹
    [SerializeField] int _slotsPerContainer = 6;              // 컨테이너 당 최대 슬롯 개수

    [Header("----- 사용 아이템 -----")]
    [SerializeField] ItemSlotView _usableItemSlot;      // 사용 아이템 슬롯 (통합 ItemSlotView 사용)

    PlayerInventory _inventory;
    List<ItemSlotView> _passiveSlotUIs = new List<ItemSlotView>();

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
        ItemSlotView existingSlot = _passiveSlotUIs.Find(slot =>
            slot.ItemData == itemData && slot.SlotType == ItemSlotType.Passive);

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
        if (_passiveItemSlotPrefab == null) return;

        Transform targetContainer = GetAvailableContainer();

        if (targetContainer != null)
        {
            GameObject slotObj = Instantiate(_passiveItemSlotPrefab, targetContainer);
            ItemSlotView slotUI = slotObj.GetComponent<ItemSlotView>();

            if (slotUI != null)
            {
                slotUI.InitializeAsPassive(itemData, count);
                _passiveSlotUIs.Add(slotUI);
            }
        }
        else
            Debug.LogWarning("모든 아이템 슬롯이 가득 찼습니다!");
    }

    /// <summary>
    /// 사용 가능한 컨테이너 반환 (순서대로 채우기)
    /// </summary>
    Transform GetAvailableContainer()
    {
        if (_passiveItemContainers == null) return null;

        // 배열 순서대로 확인하여 빈 자리가 있는 첫 번째 컨테이너 반환
        foreach (Transform container in _passiveItemContainers)
        {
            if (container != null && container.childCount < _slotsPerContainer)
            {
                return container;
            }
        }

        return null; // 모든 컨테이너가 가득 참
    }

    /// <summary>
    /// 사용 아이템 장착 시 UI 업데이트
    /// </summary>
    void OnUsableItemEquipped(ItemData itemData)
    {
        if (_usableItemSlot != null)
        {
            _usableItemSlot.InitializeAsUsable(itemData, _inventory);
        }
    }

    /// <summary>
    /// 사용 아이템 쿨타임 UI 업데이트
    /// </summary>
    void OnUsableItemCooldownChanged(float curCooldown, float maxCooldown)
    {
        if (_usableItemSlot != null)
        {
            _usableItemSlot.UpdateCooldown(curCooldown, maxCooldown);
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (_inventory != null)
        {
            _inventory.OnPassiveItemAdded -= OnPassiveItemAdded;
            _inventory.OnUsableItemEquipped -= OnUsableItemEquipped;
            _inventory.OnUsableItemCooldownChanged -= OnUsableItemCooldownChanged;
        }
    }
}
