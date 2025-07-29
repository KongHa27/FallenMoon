using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] float _floatHeight = 0.3f;     //드롭 됐을 때 둥둥 떠다닐 거리(상하)
    [SerializeField] float _floatSpeed = 2f;

    [Header("----- UI -----")]
    [SerializeField] GameObject _interactionUI;
    [SerializeField] TextMeshProUGUI _interactionText;

    Vector3 _originalPos;
    bool _heroInRange = false;
    Hero _hero;

    public ItemData ItemData => _itemData;

    void Start()
    {
        Initialize();

        //사용 아이템 줍기 입력 이벤트 구독
        ItemInteractionHandler.OnPickupInputPressed += OnPickupInputPressed;
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        ItemInteractionHandler.OnPickupInputPressed -= OnPickupInputPressed;
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

        if (_interactionUI != null)
            _interactionUI.SetActive(false);
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
            _hero = other.GetComponent<Hero>();
            if (_hero != null)
            {
                _heroInRange = true;

                // 패시브 아이템은 즉시 획득, 사용 아이템은 UI 표시
                if (_itemData.ItemType == ItemType.Passive)
                {
                    StartCoroutine(PickupPassiveItemRoutine());
                }
                else if (_itemData.ItemType == ItemType.Usable)
                {
                    ShowInteractionUI();
                }
            }
        }
    }

    /// <summary>
    /// 1초 후 패시브 아이템을 줍는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator PickupPassiveItemRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        PickupItem();
    }

    /// <summary>
    /// 플레이어와 접촉이 끝났을 때 UI 숨기기
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _heroInRange = false;
            _hero = null;
            HideInteractionUI();
        }
    }

    void OnPickupInputPressed()
    {
        if (_heroInRange && _itemData.ItemType == ItemType.Usable)
        {
            PickupItem();
        }
    }

    /// <summary>
    /// 상호작용 UI 표시
    /// </summary>
    void ShowInteractionUI()
    {
        if (_interactionUI != null)
        {
            _interactionUI.SetActive(true);
            UpdateInteractionText();
        }
    }

    /// 상호작용 UI 숨기기
    /// </summary>
    void HideInteractionUI()
    {
        if (_interactionUI != null)
        {
            _interactionUI.SetActive(false);
        }
    }

    /// <summary>
    /// 상호작용 텍스트 업데이트
    /// </summary>
    void UpdateInteractionText()
    {
        if (_interactionText == null || _hero == null) return;

        if (_itemData.ItemType == ItemType.Usable)
        {
            PlayerInventory inventory = _hero.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.EquippedUsableItem != null)
            {
                _interactionText.text = "F - 교체";
            }
            else
            {
                _interactionText.text = "F - 줍기";
            }
        }
    }

    /// <summary>
    /// 아이템 획득 처리
    /// </summary>
    public void PickupItem()
    {
        if (_hero != null)
        {
            // 사용 아이템이고 이미 장착된 아이템이 있다면 기존 아이템을 드롭
            if (_itemData.ItemType == ItemType.Usable)
            {
                PlayerInventory inventory = _hero.GetComponent<PlayerInventory>();
                if (inventory != null && inventory.EquippedUsableItem != null)
                {
                    // 기존 아이템을 현재 위치에 드롭
                    ItemManager itemmanager = ItemManager.Instance;
                    if (itemmanager != null)
                    {
                        itemmanager.DropItem(inventory.EquippedUsableItem, transform.position);
                    }
                }
            }

            // 아이템 매니저를 통해 아이템 획득 처리
            ItemManager itemManager = ItemManager.Instance;
            if (itemManager != null)
            {
                itemManager.PickupItem(_itemData, _hero);
                Destroy(gameObject);
            }
        }
    }
}

