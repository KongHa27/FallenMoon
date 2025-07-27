using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상자 타입 enum
/// </summary>
public enum ChestType
{
    Small,      //소형
    Large,      //대형
    Golden,     //황금
    Usable,    //장비
    Glass       //유리
}

public class Chest : InteractableObjects
{
    [Header("----- 상자 데이터 설정 -----")]
    [SerializeField] List<ChestData> _chestDatas;   // 상자 데이터들
    [SerializeField] ChestData _data;               // 생성할 상자 데이터
    [SerializeField] ChestDropData _dropData;       // 아이템 드롭 데이터

    [Header("----- 상자 설정 -----")]
    [SerializeField] Sprite _openedSprite;          // 열린 상자 스프라이트
    [SerializeField] int _baseCost = 15;            // 기본 비용
    [SerializeField] ChestType _chestType;          // 상자 타입

    [Header("----- 유리 상자 UI -----")]
    [SerializeField] GameObject _selectionPanel;    //유리 상자 선택 UI
    [SerializeField] GlassChestUI[] _selectionViews;    //선택 창 배열

    bool _isOpened = false;
    int _curStage;

    protected override void Awake()
    {
        base.Awake();

         _curStage = StageManager.Instance?.CurStageIndex ?? 1;

        _objectType = ObjectType.Chest;
        _destroyAfterInteraction = false;

        if (_chestDatas == null || _chestDatas.Count == 0)
        {
            Debug.LogError("ChestData가 존재하지 않습니다!!");
            return;
        }

        ChestData selected = GetDataByType(GetRanChestType());
        if (selected != null)
            Initialize(selected);
        else
            Debug.LogError("선택된 상자 타입에 대한 데이터가 없습니다!");

        if (_dropData == null)
            _dropData = GetDropData();
    }

    /// <summary>
    /// 랜덤 상자 타입을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    ChestType GetRanChestType()
    {
        float ranf = Random.Range(0f, 1f);

        //4스테이지 이후 5% 확률로 황금 상자
        if (_curStage >= 4 && ranf < 0.05f)
            return ChestType.Golden;

        //황금 상자를 제외한 나머지 상자들 스폰
        float spawnChance = _curStage >= 4 ? 0.95f : 1f;
        float ranf2 = Random.Range(0, spawnChance);

        //비율 계산 (소형 1 : 대형 0.85 : 장비 0.5 : 유리 0.5)
        //총합 2.85
        float totalRatio = 2.85f;

        float small = (1f / totalRatio) * spawnChance;
        float large = (0.85f / totalRatio) * spawnChance;
        float usable = (0.5f / totalRatio) * spawnChance;
        float glass = (0.5f / totalRatio) * spawnChance;

        float finalChance = 0f;

        //소형
        finalChance += small;
        if (ranf2 < finalChance)
            return ChestType.Small;
        //대형
        finalChance += large;
        if (ranf2 < finalChance)
            return ChestType.Large;
        //장비
        finalChance += usable;
        if (ranf2 < finalChance)
            return ChestType.Usable;
        //유리 (나머지)
        return ChestType.Glass;
    }

    /// <summary>
    /// 타입에 따라 상자 데이터를 가져오는 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    ChestData GetDataByType(ChestType type)
    {
        return _chestDatas.Find(_data => _data.Chesttype == type);
    }

    /// <summary>
    /// 상자 초기화 함수
    /// </summary>
    /// <param name="data"></param>
    void Initialize(ChestData data)
    {
        _data = data;

        _chestType = _data.Chesttype;
        _sprite = _data.ClosedSprite;
        _openedSprite = _data.OpenedSprite;

        _objectName = _data.GetName();

        // 디버깅: 스프라이트 정보 확인
        Debug.Log($"상자 초기화 - 타입: {_chestType}, 이름: {_objectName}");
        Debug.Log($"ClosedSprite null 여부: {_sprite == null}");
        Debug.Log($"OpenedSprite null 여부: {_openedSprite == null}");

        ApplySprite();
        SetChestScale();
    }

    /// <summary>
    /// 스프라이트를 실제로 적용하는 함수
    /// </summary>
    void ApplySprite()
    {
        if (_renderer == null)
        {
            Debug.LogError($"[{gameObject.name}] SpriteRenderer가 null입니다!");
            return;
        }

        if (_sprite == null)
        {
            Debug.LogError($"[{gameObject.name}] 적용할 스프라이트가 null입니다!");
            return;
        }

        _renderer.sprite = _sprite;

        // 디버깅: 렌더러 상태 확인
        Debug.Log($"[{gameObject.name}] 스프라이트 적용 완료");
        Debug.Log($"Renderer enabled: {_renderer.enabled}");
        Debug.Log($"GameObject active: {gameObject.activeInHierarchy}");
        Debug.Log($"Sprite 이름: {_sprite.name}");
        Debug.Log($"Renderer 정렬 레이어: {_renderer.sortingLayerName}");
        Debug.Log($"Renderer 정렬 순서: {_renderer.sortingOrder}");
    }

    /// <summary>
    /// 상자 타입에 따른 크기 설정
    /// </summary>
    void SetChestScale()
    {
        Vector3 scale = _chestType switch
        {
            ChestType.Small => new Vector3(0.5f, 0.5f, 1f),     // 소형 상자
            ChestType.Large => new Vector3(0.7f, 0.7f, 1f),     // 대형 상자  
            ChestType.Golden => new Vector3(0.8f, 0.8f, 1f),    // 황금 상자 (조금 더 크게)
            ChestType.Glass => new Vector3(0.6f, 0.6f, 1f),     // 유리 상자
            ChestType.Usable => new Vector3(0.6f, 0.6f, 1f),    // 장비 상자
            _ => new Vector3(0.6f, 0.6f, 1f)                     // 기본 크기
        };

        transform.localScale = scale;

        Debug.Log($"[{gameObject.name}] 상자 크기 설정: {_chestType} -> {scale}");
    }

    /// <summary>
    /// 상자 상호작용 처리
    /// </summary>
    /// <param name="interactor"></param>
    protected override void OnInteract(GameObject interactor)
    {
        Hero hero = interactor.GetComponent<Hero>();
        if (hero == null)
        {
            Debug.LogError("Hero 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        int cost = GetChestCost();

        // HeroModel 확인
        HeroModel heroModel = hero.GetComponentInChildren<HeroModel>();
        if (heroModel == null)
        {
            Debug.LogError("HeroModel 컴포넌트를 찾을 수 없습니다!");
            return;
        }
        // 골드 확인
        int curGold = heroModel.Gold;
        Debug.Log($"현재 골드: {curGold}, 필요 골드: {cost}");
        
        //골드 부족 체크
        if (curGold < cost)
        {
            Debug.Log($"골드가 부족합니다! 필요 골드: {cost}, 보유 골드: {curGold}");
            return;
        }

        // 골드 소모
        if (!heroModel.TryToSpendGold(cost))
        {
            Debug.Log("골드 소모에 실패했습니다!");
            return;
        }

        //유리 상자일 경우 선택 UI
        if (_chestType == ChestType.Glass)
            ShowGlassChestSelection();
        else
        {
            GiveItem(hero);
            CompleteChestOpening();
        }

        Debug.Log($"{_objectName}을(를) 열어 보상을 획득했습니다! (비용: {cost} 골드)");
    }

    /// <summary>
    /// 상자 타입과 침식도에 따른 상자 최종 비용을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    int GetChestCost()
    {
        // 침식도에 따른 비용 조정 (15% 증가)
        float erosionMultiplier = 1f + (DifficultyManager.Instance.CurrentErosionLevel * 0.15f);
        int cost = Mathf.RoundToInt(_baseCost * _data.GetMultiplier() * erosionMultiplier);

        return cost;
    }

    /// <summary>
    /// 유리 상자 선택 UI 표시
    /// </summary>
    void ShowGlassChestSelection()
    {
        //랜덤 옵션 아이템 생성
        List<ItemData> items = GenerateGlassChestOptions();

        //UI에 표시
        for (int i = 0; i < _selectionViews.Length; i++)
        {
            if (i < items.Count)
            {
                _selectionViews[i].Initialize(this, items[i]);
            }
        }

        _selectionPanel.SetActive(true);
    }

    /// <summary>
    /// 유리 상자에서 아이템을 선택했을 때 호출되는 함수
    /// </summary>
    /// <param name="selectedItem"></param>
    /// <param name="hero"></param>
    public void OnGlassChestItemSelected(ItemData selectedItem)
    {
        //선택한 아이템 드롭
        ItemManager.Instance.DropItem(selectedItem, transform.position);
        _selectionPanel.SetActive(false);
        CompleteChestOpening();
    }

    /// <summary>
    /// 유리 상자 옵션 생성 (랜덤 아이템 3개)
    /// </summary>
    /// <returns></returns>
    List<ItemData> GenerateGlassChestOptions()
    {
        List<ItemData> options = new();

        float ranf = Random.Range(0f, 1f);
        ItemRarity rarity;

        if (ranf < 0.8f)
            rarity = ItemRarity.Common;
        else
            rarity = ItemRarity.Uncommon;

        for (int i = 0; i < 3; i++)
        {
            ItemData ranItem = GetRanItemByRarity(rarity);
            if (ranItem != null)
                options.Add(ranItem);
        }

        return options;
    }

    /// <summary>
    /// 보상 지급 함수
    /// </summary>
    /// <param name="hero"></param>
    void GiveItem(Hero hero)
    {
        // 장비 상자는 사용 아이템만 드롭
        if (_chestType == ChestType.Usable)
        {
            RewardForUsableChest();
            return;
        }

        // 일반 상자들은 패시브 아이템 드롭
        ItemRarity dropRarity = DetermineDropRarity();
        ItemManager.Instance.DropItemFromObject(hero.transform.position, dropRarity);
    }

    /// <summary>
    /// 장비 상자 보상
    /// </summary>
    void RewardForUsableChest()
    {
        ItemData[] usableItems = ItemManager.Instance.GetUsableItems();
        if (usableItems != null && usableItems.Length > 0)
        {
            ItemData ranUsableItem = usableItems[Random.Range(0, usableItems.Length)];
            ItemManager.Instance.DropItem(ranUsableItem, transform.position);
        }
    }

    /// <summary>
    /// 드롭 아이템 등급 결정하는 함수
    /// </summary>
    /// <returns></returns>
    ItemRarity DetermineDropRarity()
    {
        float randomValue = Random.Range(0f, 1f);
        float cumulativeChance = 0f;

        // 전설 등급 체크
        cumulativeChance += _dropData.legendaryChance;
        if (randomValue <= cumulativeChance)
            return ItemRarity.Legendary;

        // 희귀 등급 체크
        cumulativeChance += _dropData.uncommonChance;
        if (randomValue <= cumulativeChance)
            return ItemRarity.Uncommon;

        // 기본적으로 일반 등급
        return ItemRarity.Common;
    }

    /// <summary>
    /// 등급 별 랜덤 아이템 가져오기
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    ItemData GetRanItemByRarity(ItemRarity rarity)
    {
        ItemData[] items = ItemManager.Instance.GetPassiveItemsByRarity(rarity);
        if (items != null && items.Length > 0)
            return items[Random.Range(0, items.Length)];
        
        return null;
    }

    ChestDropData GetDropData()
    {
        return _chestType switch
        {
            ChestType.Small => new ChestDropData(0.8f, 0.19f, 0.01f),      // 80% 일반, 19% 희귀, 1% 전설
            ChestType.Large => new ChestDropData(0.9f, 0.1f, 0f),          // 90% 희귀, 10% 전설
            ChestType.Golden => new ChestDropData(1f, 0f, 0f),             // 100% 전설 (실제로는 전설 드롭)
            ChestType.Glass => new ChestDropData(1f, 0f, 0f),              // 선택형이므로 기본값
            ChestType.Usable => new ChestDropData(1f, 0f, 0f),            // 사용 아이템만
            _ => new ChestDropData(0.8f, 0.19f, 0.01f)
        };
    }

    /// <summary>
    /// 상호작용 텍스트 반환
    /// </summary>
    /// <returns></returns>
    public override string GetInteractionText()
    {
        if (_isOpened)
        {
            return "";
        }

        if (!CanInteract)
        {
            return "";
        }

        int cost = GetChestCost();

        return $"[E] {_objectName} 열기 (골드 {cost})";
    }

    void CompleteChestOpening()
    {
        if (_openedSprite != null && _renderer != null)
            _renderer.sprite = _openedSprite;

        _isOpened = true;
        _canInteract = false;
    }

    public override bool CanInteract
    {
        get
        {
            return base.CanInteract && !_isOpened;
        }
    }
}
