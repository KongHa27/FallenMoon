using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 타입 enum
/// </summary>
public enum ItemType
{
    Passive = 0,        //패시브 아이템
    Usable = 1          //사용 아이템
}

/// <summary>
/// 아이템 등급 enum
/// </summary>
public enum ItemRarity
{
    Common = 0,         //일반 (회색)
    Uncommon = 1,       //희귀 (초록)
    Legendary = 2,      //전설 (주황)
    BossItem = 3        //보스 (빨강)
}

/// <summary>
/// 패시브 아이템 효과 타입
/// </summary>
public enum PassiveEffectType
{
    MaxHpBonus,         //최대 체력 증가
    DamageBonus,        //공격력 증가
    MoveSpeedBonus,     //이동속도 증가
    AttackSpeedBonus,   //공격속도 증가
    DefenseBonus,       //방어력 증가
    GoldBonus,          //골드 획득량 증가
    LuckBonus,          //운 (아이템 드롭률 증가)
    HpRegenBonus,       //체력 재생 증가
    SpecialEffect       //특수 효과
}

public enum UsableItemEffectType
{
    Heal,               // 체력 회복
    SpeedBoost,         // 이동속도 일시 증가
    DamageBoost,        // 공격력 일시 증가
    Invincibility,      // 무적 상태
    Teleport,           // 순간이동
    AreaHeal,           // 광역 힐
    Bomb,               // 폭탄 (광역 공격)
    Shield,             // 방어막 생성
    TimeSlowdown,       // 시간 둔화
    LightRecharge,      // 광원 게이지 충전
    ExpBoost,           // 경험치 획득량 일시 증가
    GoldMagnet,         // 일정 시간 골드 자동 수집
}
