using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 시스템을 총괄하는 매니저 클래스
/// </summary>
public class ItemManager : MonoBehaviour
{
    [Header("----- 드롭 설정 -----")]
    [SerializeField] GameObject _itemPickupPrefab;
    [SerializeField] ItemData[] _commonItems;
    [SerializeField] ItemData[] _uncommonItems;
    [SerializeField] ItemData[] _legendaryItems;
    [SerializeField] ItemData[] _bossItems;
    [SerializeField] ItemData[] _usableItems;

    [Header("----- 드롭 확률 설정 -----")]
    [SerializeField] float _bossItemDropChance = 0.1f;      // 보스 아이템 드롭 확률
    [SerializeField] float _eliteItemDropChance = 0.05f;    // 엘리트 아이템 드롭 확률

    // 싱글톤
    public static ItemManager Instance { get; private set; }

    // 인벤토리 시스템
    PlayerInventory _playerInventory;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _playerInventory = FindObjectOfType<PlayerInventory>();

        // 보스 사망 이벤트 구독
        Boss[] bosses = FindObjectsOfType<Boss>();
        foreach (Boss boss in bosses)
        {
            boss.OnBossDead += () => HandleBossDeath(boss.transform.position);
        }

        // 적 사망 이벤트 구독
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.OnEnemyDeath += HandleEnemyDeath;
        }
    }

    /// <summary>
    /// 아이템 획득 처리
    /// </summary>
    public void PickupItem(ItemData itemData, Hero hero)
    {
        if (_playerInventory != null)
        {
            _playerInventory.AddItem(itemData);
        }
    }

    /// <summary>
    /// 보스 사망 시 아이템 드롭 처리
    /// </summary>
    void HandleBossDeath(Vector3 position)
    {
        if (UnityEngine.Random.Range(0f, 1f) < _bossItemDropChance)
        {
            DropRandomItem(position, ItemRarity.BossItem);
        }
    }

    /// <summary>
    /// 적 사망 시 아이템 드롭 처리
    /// </summary>
    void HandleEnemyDeath(Enemy enemy)
    {
        // 네잎클로버 소지 시에만 엘리트가 아이템 드롭
        if (enemy.IsElite && _playerInventory.HasSpecialEffect("FourLeafClover"))
        { 
            if (Random.Range(0f, 1f) < _eliteItemDropChance)
            {
                // 등급별 드롭 확률 (일반 70%, 희귀 25%, 전설 5%)
                float rarity = Random.Range(0f, 1f);
                ItemRarity dropRarity;

                if (rarity < 0.7f)
                    dropRarity = ItemRarity.Common;
                else if (rarity < 0.95f)
                    dropRarity = ItemRarity.Uncommon;
                else
                    dropRarity = ItemRarity.Legendary;

                DropRandomItem(enemy.transform.position, dropRarity);
            }
        }
    }

    /// <summary>
    /// 상자나 오브젝트에서 아이템 드롭
    /// </summary>
    public void DropItemFromObject(Vector3 position, ItemRarity rarity = ItemRarity.Common)
    {
        DropRandomItem(position, rarity);
    }

    /// <summary>
    /// 랜덤 아이템 드롭
    /// </summary>
    void DropRandomItem(Vector3 position, ItemRarity rarity)
    {
        ItemData[] itemArray = GetPassiveItemsByRarity(rarity);

        if (itemArray != null && itemArray.Length > 0)
        {
            ItemData randomItem = itemArray[Random.Range(0, itemArray.Length)];
            DropItem(randomItem, position);
        }
    }

    /// <summary>
    /// 특정 아이템 드롭
    /// </summary>
    public void DropItem(ItemData itemData, Vector3 position)
    {
        if (_itemPickupPrefab != null && itemData != null)
        {
            GameObject itemObj = Instantiate(_itemPickupPrefab, position, Quaternion.identity);
            ItemPickup pickup = itemObj.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.Initialize(itemData);
            }
        }
    }

    /// <summary>
    /// 등급에 따른 아이템 배열 반환
    /// </summary>
    public ItemData[] GetPassiveItemsByRarity(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return _commonItems;
            case ItemRarity.Uncommon:
                return _uncommonItems;
            case ItemRarity.Legendary:
                return _legendaryItems;
            case ItemRarity.BossItem:
                return _bossItems;
            default:
                return _commonItems;
        }
    }

    public ItemData[] GetUsableItems()
    {
        return _usableItems;
    }
}
