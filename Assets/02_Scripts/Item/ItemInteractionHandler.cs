using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ��ȣ�ۿ��� ���� �ڵ鷯 Ŭ����
/// </summary>
public class ItemInteractionHandler : MonoBehaviour
{
    [Header("----- ���� -----")]
    [SerializeField] float _interactionRange = 1f;  // ��ȣ�ۿ� ����

    public static event Action OnPickupInputPressed;

    Hero _hero;

    void Start()
    {
        _hero = GetComponent<Hero>();
    }

    /// <summary>
    /// FŰ �Է� ó�� (PlayScene���� ȣ��)
    /// </summary>
    public void HandlePickupInput()
    {
        // ��� ItemPickup���� FŰ�� �������� �˸�
        OnPickupInputPressed?.Invoke();
    }

    /// <summary>
    /// �÷��̾� ��ġ ��ȯ (ItemPickup���� �Ÿ� ����)
    /// </summary>
    public static Vector3 GetPlayerPosition()
    {
        Hero hero = FindObjectOfType<Hero>();
        return hero != null ? hero.transform.position : Vector3.zero;
    }

    /// <summary>
    /// ��ȣ�ۿ� ���� ��ȯ
    /// </summary>
    public static float GetInteractionRange()
    {
        ItemInteractionHandler handler = FindObjectOfType<ItemInteractionHandler>();
        return handler != null ? handler._interactionRange : 2f;
    }

    /// <summary>
    /// ����׿� ��ȣ�ۿ� ���� ǥ��
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
