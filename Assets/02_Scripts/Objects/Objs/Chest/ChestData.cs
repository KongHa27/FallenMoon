using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/ChestData", fileName = "ChestData")]
public class ChestData : ScriptableObject
{
    [Header("----- 타입 -----")]
    [SerializeField] ChestType _chestType;          //상자 타입

    [Header("----- 스프라이트 -----")]
    [SerializeField] Sprite _closedSprite;          //기본 스프라이트 (닫힌 이미지)
    [SerializeField] Sprite _openedSprite;          //열린 상자 스프라이트

    //프로퍼티
    public ChestType Chesttype => _chestType;
    public Sprite ClosedSprite => _closedSprite;
    public Sprite OpenedSprite => _openedSprite;

    /// <summary>
    /// 상자 타입에 따른 비용 배수 반환
    /// </summary>
    /// <returns></returns>
    public float GetMultiplier()
    {
        return _chestType switch
        {
            ChestType.Small => 1f,
            ChestType.Large => 2f,
            ChestType.Golden => 6f,
            ChestType.Glass => 1.2f,
            ChestType.Usable => 1.5f,
            _ => 1f
        };
    }

    public string GetName()
    {
        return _chestType switch
        {
            ChestType.Small => "소형 상자",
            ChestType.Large => "대형 상자",
            ChestType.Golden => "황금 상자",
            ChestType.Glass => "유리 상자",
            ChestType.Usable => "장비 상자",
            _ => "상자"
        };
    }
}

/// <summary>
/// 상자 드롭 확률 데이터
/// </summary>
[System.Serializable]
public class ChestDropData
{
    public float commonChance;      // 일반 등급 확률
    public float uncommonChance;    // 희귀 등급 확률  
    public float legendaryChance;   // 전설 등급 확률

    public ChestDropData(float common, float uncommon, float legendary)
    {
        commonChance = common;
        uncommonChance = uncommon;
        legendaryChance = legendary;
    }
}
