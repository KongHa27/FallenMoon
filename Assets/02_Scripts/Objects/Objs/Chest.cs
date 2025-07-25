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
    Useable,    //장비
    Glass       //유리
}

public class Chest : InteractableObjects
{
    [Header("----- 상자 설정 -----")]
    [SerializeField] Sprite _openedSprite;              // 열린 상자 스프라이트
    [SerializeField] int _baseCost = 34;                // 기본 비용
    [SerializeField] ChestType _chestType;              // 상자 타입

    [Header("----- 보상 설정 -----")]
    [SerializeField] int _minGoldReward = 20;           // 최소 골드 보상
    [SerializeField] int _maxGoldReward = 40;           // 최대 골드 보상
    [SerializeField] float _expReward = 25f;            // 경험치 보상
    [SerializeField] float _maxHpBonus = 10f;           // 최대 체력 보너스
    [SerializeField] float _healAmount = 20f;           // 힐량

    bool _isOpened = false;

    protected override void Awake()
    {
        base.Awake();
        _objectType = ObjectType.Chest;
        _objectName = GetChestName();
        _destroyAfterInteraction = false;
    }

    /// <summary>
    /// 상자 상호작용 처리
    /// </summary>
    /// <param name="interactor"></param>
    protected override void OnInteract(GameObject interactor)
    {
        Hero hero = interactor.GetComponent<Hero>();
        if (hero == null) return;

        // 침식도에 따른 비용 조정 (15% 증가)
        float erosionMultiplier = 1f + (DifficultyManager.Instance.CurrentErosionLevel * 0.15f);
        int cost = Mathf.RoundToInt(_baseCost * GetMultiplierByType() * erosionMultiplier);

        // 골드 확인
        HeroModel heroModel = hero.GetComponent<HeroModel>();
        if (heroModel == null || heroModel.Gold < cost)
        {
            Debug.Log($"골드가 부족합니다! 필요 골드: {cost}, 보유 골드: {heroModel?.Gold ?? 0}");
            return;
        }

        // 골드 소모
        if (!heroModel.TryToSpendGold(cost))
        {
            Debug.Log("골드 소모에 실패했습니다!");
            return;
        }

        // 보상 지급
        GiveRewards(hero);

        // 열린 상자 스프라이트로 변경
        if (_openedSprite != null && _renderer != null)
        {
            _renderer.sprite = _openedSprite;
        }

        _isOpened = true;
        _canInteract = false;   // 더 이상 상호작용 불가

        Debug.Log($"{_objectName}을(를) 열어 보상을 획득했습니다! (비용: {cost} 골드)");
    }

    /// <summary>
    /// 보상 지급 함수
    /// </summary>
    /// <param name="hero"></param>
    void GiveRewards(Hero hero)
    {

    }

    /// <summary>
    /// 상자 타입 별 배수 반환
    /// </summary>
    /// <returns></returns>
    float GetMultiplierByType()
    {
        return _chestType switch
        {
            ChestType.Small => 1f,
            ChestType.Large => 2f,
            ChestType.Golden => 6f,
            ChestType.Glass => 1.2f,
            _ => 1f
        };
    }

    /// <summary>
    /// 등급별 상자 이름 반환
    /// </summary>
    /// <returns></returns>
    string GetChestName()
    {
        return _chestType switch
        {
            ChestType.Small => "소형 상자",
            ChestType.Large => "대형 상자",
            ChestType.Golden => "황금 상자",
            ChestType.Glass => "유리 상자",
            _ => "상자"
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

        // 침식도에 따른 비용 계산
        float erosionMultiplier = 1f + (DifficultyManager.Instance.CurrentErosionLevel * 0.15f);
        int cost = Mathf.RoundToInt(_baseCost * GetMultiplierByType() * erosionMultiplier);

        return $"[E] {_objectName} 열기 (골드 {cost})";
    }

    public override bool CanInteract
    {
        get
        {
            return base.CanInteract && !_isOpened;
        }
    }
}
