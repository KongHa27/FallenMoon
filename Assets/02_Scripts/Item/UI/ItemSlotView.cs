using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemSlotType
{
    Passive,    // 패시브 아이템 슬롯
    Usable      // 사용 아이템 슬롯
}

public class ItemSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("----- 기본 UI 컴포넌트 -----")]
    [SerializeField] Image _iconImage;                              // 아이템 아이콘
    [SerializeField] Image _backgroundImage;                        // 배경 이미지 (등급 색상)
    [SerializeField] TextMeshProUGUI _countText;                    // 개수 텍스트 (패시브용)

    [Header("----- 쿨다운 UI (사용 아이템용) -----")]
    [SerializeField] Image _cooldownFill;                           // 쿨다운 fillAmount 이미지
    [SerializeField] TextMeshProUGUI _cooldownText;                 // 쿨다운 시간 텍스트

    [Header("----- 툴팁 UI -----")]
    [SerializeField] GameObject _tooltipPanel;                      // 툴팁 패널
    [SerializeField] TextMeshProUGUI _itemNameText;                 // 아이템 이름
    [SerializeField] TextMeshProUGUI _descriptionText;              // 아이템 설명
    [SerializeField] TextMeshProUGUI _cooldownInfoText;             // 사용 아이템 쿨타임 정보

    [Header("----- 설정 -----")]
    [SerializeField] ItemSlotType _slotType = ItemSlotType.Passive; // 슬롯 타입

    ItemData _itemData;
    int _itemCount = 1;
    PlayerInventory _inventory; // 사용 아이템의 쿨다운 정보 확인용

    public ItemData ItemData => _itemData;
    public ItemSlotType SlotType => _slotType;

    /// <summary>
    /// 패시브 아이템 슬롯으로 초기화
    /// </summary>
    public void InitializeAsPassive(ItemData itemData, int count)
    {
        _slotType = ItemSlotType.Passive;
        _itemData = itemData;
        _itemCount = count;

        SetupBasicUI();
        UpdateCount(count);

        // 쿨다운 UI 비활성화
        SetCooldownUIActive(false);

        // 툴팁 초기에는 비활성화
        SetTooltipActive(false);
    }

    /// <summary>
    /// 사용 아이템 슬롯으로 초기화
    /// </summary>
    public void InitializeAsUsable(ItemData itemData, PlayerInventory inventory)
    {
        _slotType = ItemSlotType.Usable;
        _itemData = itemData;
        _itemCount = 1;
        _inventory = inventory;
        _cooldownFill.sprite = _itemData.Icon;

        SetupBasicUI();

        // 개수 텍스트 비활성화
        if (_countText != null)
            _countText.gameObject.SetActive(false);

        // 쿨다운 UI 활성화
        SetCooldownUIActive(true);

        // 툴팁 초기에는 비활성화
        SetTooltipActive(false);
    }

    /// <summary>
    /// 기본 UI 설정 (공통)
    /// </summary>
    void SetupBasicUI()
    {
        if (_itemData == null) return;

        // 아이콘 설정
        if (_iconImage != null)
            _iconImage.sprite = _itemData.Icon;

        // 배경 설정
        if (_backgroundImage != null)
        {
            _backgroundImage.sprite = _itemData.Icon;
            _backgroundImage.color = _itemData.GetRarityColor();
        }
    }

    /// <summary>
    /// 패시브 아이템 개수 업데이트
    /// </summary>
    public void UpdateCount(int count)
    {
        if (_slotType != ItemSlotType.Passive) return;

        _itemCount = count;

        if (_countText != null)
        {
            if (count > 1)
            {
                _countText.text = $"×{count}";
                _countText.gameObject.SetActive(true);
            }
            else
            {
                _countText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 사용 아이템 쿨다운 업데이트
    /// </summary>
    public void UpdateCooldown(float curCooldown, float maxCooldown)
    {
        if (_slotType != ItemSlotType.Usable) return;

        // 쿨다운 fillAmount 업데이트
        if (_cooldownFill != null && maxCooldown > 0)
        {
            _cooldownFill.fillAmount = curCooldown / maxCooldown;
        }

        // 쿨다운 텍스트 업데이트
        if (_cooldownText != null)
        {
            if (curCooldown > 0)
            {
                _cooldownText.text = ((int)curCooldown).ToString();
                _cooldownText.gameObject.SetActive(true);
            }
            else
            {
                _cooldownText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 쿨다운 UI 활성화/비활성화
    /// </summary>
    void SetCooldownUIActive(bool active)
    {
        if (_cooldownFill != null)
            _cooldownFill.gameObject.SetActive(active);
        if (_cooldownText != null)
            _cooldownText.gameObject.SetActive(false); // 초기에는 비활성화
    }

    /// <summary>
    /// 툴팁 활성화/비활성화
    /// </summary>
    void SetTooltipActive(bool active)
    {
        if (_tooltipPanel != null)
            _tooltipPanel.SetActive(active);
    }

    /// <summary>
    /// 마우스가 아이템 슬롯에 올라왔을 때
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    /// <summary>
    /// 마우스가 아이템 슬롯에서 나갔을 때
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    /// <summary>
    /// 툴팁 표시
    /// </summary>
    void ShowTooltip()
    {
        if (_tooltipPanel == null || _itemData == null) return;

        if (_slotType == ItemSlotType.Passive)
        {
            ShowPassiveTooltip();
        }
        else
        {
            ShowUsableTooltip();
        }

        // 툴팁 위치 설정
        Vector3 tooltipPosition = transform.position + Vector3.right * 150f + Vector3.up * 100f;
        _tooltipPanel.transform.position = tooltipPosition;

        SetTooltipActive(true);
    }

    /// <summary>
    /// 패시브 아이템 툴팁 표시
    /// </summary>
    void ShowPassiveTooltip()
    {
        // 아이템 이름 설정
        if (_itemNameText != null)
        {
            _itemNameText.text = _itemData.ItemName;
        }

        // 아이템 설명 설정
        if (_descriptionText != null)
        {
            _descriptionText.text = _itemData.Description;
        }
    }

    /// <summary>
    /// 사용 아이템 툴팁 표시
    /// </summary>
    void ShowUsableTooltip()
    {
        // 아이템 이름 설정
        if (_itemNameText != null)
            _itemNameText.text = _itemData.ItemName;

        // 아이템 쿨타임
        if (_cooldownInfoText != null)
            _cooldownInfoText.text = $"{_itemData.Cooldown}초";

        // 아이템 설명
        if (_descriptionText != null)
            _descriptionText.text = _itemData.Description;
    }

    /// <summary>
    /// 툴팁 숨기기
    /// </summary>
    void HideTooltip()
    {
        SetTooltipActive(false);
    }
}
