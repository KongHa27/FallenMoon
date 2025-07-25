using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveItemSlotUI : MonoBehaviour
{
    [Header("----- UI 컴포넌트 -----")]
    [SerializeField] Image _iconImage;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] Image _backgroundImage;

    ItemData _itemData;

    public ItemData ItemData => _itemData;

    /// <summary>
    /// 슬롯 초기화
    /// </summary>
    public void Initialize(ItemData itemData, int count)
    {
        _itemData = itemData;

        if (_iconImage != null)
        {
            _iconImage.sprite = itemData.Icon;
        }

        if (_backgroundImage != null)
        {
            _backgroundImage.color = itemData.GetRarityColor();
        }

        UpdateCount(count);
    }

    /// <summary>
    /// 개수 업데이트
    /// </summary>
    public void UpdateCount(int count)
    {
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
}
