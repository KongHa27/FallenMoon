using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlassChestUI : MonoBehaviour
{
    [Header("----- UI -----")]
    [SerializeField] Image _iconBG;              //아이템 아이콘 배경
    [SerializeField] Image _icon;                //아이템 아이콘

    Chest _chest;
    ItemData _data;

    public void Initialize(Chest chest, ItemData data)
    {
        _chest = chest;
        _data = data;

        _iconBG.sprite = _data.Icon;
        _iconBG.color = _data.GetRarityColor();

        _icon.sprite = _data.Icon;
    }

    public void OnClicked()
    {
        if (_chest != null)
            _chest.OnGlassChestItemSelected(_data);
    }
}
