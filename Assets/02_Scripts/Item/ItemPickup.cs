using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드에 드롭된 아이템을 제어하는 클래스
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("----- 아이템 설정 -----")]
    [SerializeField] ItemData _itemData;
    [SerializeField] SpriteRenderer _bgRenderer;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Collider2D _collider;

    [Header("----- 시각 효과 -----")]
    [SerializeField] float _floatHeight = 0.3f;
    [SerializeField] float _floatSpeed = 2f;

    Vector3 _originalPos;

    public ItemData ItemData => _itemData;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 아이템 초기화
    /// </summary>
    public void Initialize(ItemData itemData = null)
    {
        if (itemData != null)
            _itemData = itemData;

        if (_bgRenderer != null && _renderer != null)
        {
            //배경 아이콘 및 색 설정
            _bgRenderer.sprite = _itemData.Icon;
            _bgRenderer.color = _itemData.GetRarityColor();

            //아이콘 설정
            _renderer.sprite = _itemData.Icon;
        }

        _originalPos = transform.position;
        StartCoroutine(FloatAnimation());
    }

    /// <summary>
    /// 둥둥 떠다니는 애니메이션
    /// </summary>
    IEnumerator FloatAnimation()
    {
        while (true)
        {
            float newY = _originalPos.y + Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;
            transform.position = new Vector3(_originalPos.x, newY, _originalPos.z);
            yield return null;
        }
    }

    /// <summary>
    /// 플레이어와 접촉 시 아이템 획득
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Hero hero = other.GetComponent<Hero>();
            if (hero != null)
            {
                // 아이템 매니저를 통해 아이템 획득 처리
                ItemManager itemManager = FindObjectOfType<ItemManager>();
                if (itemManager != null)
                {
                    itemManager.PickupItem(_itemData, hero);
                    Destroy(gameObject);
                }
            }
        }
    }
}

