using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 상호작용을 위한 핸들러 클래스
/// </summary>
public class ItemInteractionHandler : MonoBehaviour
{
    [Header("----- 설정 -----")]
    [SerializeField] float _interactionRange = 1f;  // 상호작용 범위

    public static event Action OnPickupInputPressed;

    Hero _hero;

    void Start()
    {
        _hero = GetComponent<Hero>();
    }

    /// <summary>
    /// F키 입력 처리 (PlayScene에서 호출)
    /// </summary>
    public void HandlePickupInput()
    {
        // 모든 ItemPickup에게 F키가 눌렸음을 알림
        OnPickupInputPressed?.Invoke();
    }

    /// <summary>
    /// 플레이어 위치 반환 (ItemPickup에서 거리 계산용)
    /// </summary>
    public static Vector3 GetPlayerPosition()
    {
        Hero hero = FindObjectOfType<Hero>();
        return hero != null ? hero.transform.position : Vector3.zero;
    }

    /// <summary>
    /// 상호작용 범위 반환
    /// </summary>
    public static float GetInteractionRange()
    {
        ItemInteractionHandler handler = FindObjectOfType<ItemInteractionHandler>();
        return handler != null ? handler._interactionRange : 2f;
    }

    /// <summary>
    /// 디버그용 상호작용 범위 표시
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (_hero != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_hero.transform.position, _interactionRange);
        }
    }
}
