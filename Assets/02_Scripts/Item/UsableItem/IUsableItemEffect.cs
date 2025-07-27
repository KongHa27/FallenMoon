using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사용 아이템 효과 인터페이스
/// </summary>
public interface IUsableItemEffect
{
    void ApplyEffect(Hero hero, ItemData itemData);
}
