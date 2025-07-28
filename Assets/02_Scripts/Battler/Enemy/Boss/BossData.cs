using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossPattern
{
    [Header("----- 패턴 기본 정보 -----")]
    public string patternName;
    public float cooldown;
    public float damage;
    public float range;
    public float castTime;

    [Header("----- 패턴별 설정 -----")]
    public BossPatternType patternType;
    public GameObject projectilePrefab;     //투사체 프리팹
    public GameObject effectPrefab;         //이펙트 프리팹
}

public enum BossPatternType
{
    RockThrow,
    GroundSlam,
}

[CreateAssetMenu(menuName = "GameSettings/BossData", fileName = "BossData")]
public class BossData : EnemyData
{
    [Header("----- 보스 설정 -----")]
    [SerializeField] bool _isBoss = true;
    [SerializeField] BossPattern[] _patterns;
    [SerializeField] float _patternStartDelay = 2f;     //패턴 시작 딜레이 (기본 2초)

    public bool IsBoss => _isBoss;
    public BossPattern[] Patterns => _patterns;
    public float PatternStartDelay => _patternStartDelay;
}


