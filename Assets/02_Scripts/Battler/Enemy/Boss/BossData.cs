using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossPattern
{
    [Header("----- ���� �⺻ ���� -----")]
    public string patternName;
    public float cooldown;
    public float damage;
    public float range;
    public float castTime;

    [Header("----- ���Ϻ� ���� -----")]
    public BossPatternType patternType;
    public GameObject projectilePrefab;     //����ü ������
    public GameObject effectPrefab;         //����Ʈ ������
}

public enum BossPatternType
{
    RockThrow,
    GroundSlam,
}

[CreateAssetMenu(menuName = "GameSettings/BossData", fileName = "BossData")]
public class BossData : EnemyData
{
    [Header("----- ���� ���� -----")]
    [SerializeField] bool _isBoss = true;
    [SerializeField] BossPattern[] _patterns;
    [SerializeField] float _patternStartDelay = 2f;     //���� ���� ������ (�⺻ 2��)

    public bool IsBoss => _isBoss;
    public BossPattern[] Patterns => _patterns;
    public float PatternStartDelay => _patternStartDelay;
}


