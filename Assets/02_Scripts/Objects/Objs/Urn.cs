using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urn : InteractableObjects
{
    [Header("----- 항아리 설정 -----")]
    [SerializeField] Sprite _brokenSprite;              // 부서진 항아리 스프라이트
    [SerializeField] int _minGoldReward = 5;            // 최소 골드 보상
    [SerializeField] int _maxGoldReward = 15;           // 최대 골드 보상
    [SerializeField] float _expReward = 10f;            // 경험치 보상
    [SerializeField] float _minLightReward = 2f;        // 최소 광원 게이지 보상
    [SerializeField] float _maxLightReward = 3f;        // 최대 광원 게이지 보상

    protected override void Awake()
    {
        base.Awake();
        _objectType = ObjectType.Urn;
        _objectName = "항아리";
        _destroyAfterInteraction = true;    // 항아리는 상호작용 후 파괴됨
    }

    /// <summary>
    /// 항아리 상호작용 처리
    /// </summary>
    /// <param name="interactor"></param>
    protected override void OnInteract(GameObject interactor)
    {
        Hero hero = interactor.GetComponent<Hero>();
        if (hero == null) return;

        // 침식도에 따른 보상량 조정 (15% 증가)
        float erosionMultiplier = 1f + (DifficultyManager.Instance.CurrentErosionLevel * 0.15f);

        // 보상 계산
        int goldReward = Mathf.RoundToInt(UnityEngine.Random.Range(_minGoldReward, _maxGoldReward + 1) * erosionMultiplier);
        float expReward = _expReward * erosionMultiplier;
        float lightReward = UnityEngine.Random.Range(_minLightReward, _maxLightReward + 1);

        // 보상 지급
        hero.AddGold(goldReward);
        hero.AddExp(expReward);
        hero.AddLightGauge(lightReward);

        Debug.Log($"항아리에서 골드 {goldReward}개, 경험치 {expReward}, 광원 게이지 {lightReward}를 획득했습니다!");

        // 부서진 스프라이트로 변경
        if (_brokenSprite != null && _renderer != null)
        {
            _renderer.sprite = _brokenSprite;
        }

        // 상호작용 비활성화
        _canInteract = false;
    }

    /// <summary>
    /// 상호작용 텍스트 반환
    /// </summary>
    /// <returns></returns>
    public override string GetInteractionText()
    {
        if (CanInteract)
        {
            return "[E] 항아리 부수기";
        }
        else
        {
            return "";
        }
    }
}
