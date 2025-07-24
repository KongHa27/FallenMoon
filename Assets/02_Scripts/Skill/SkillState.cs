using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 스킬 런타임 상태를 관리하는 클래스
/// </summary>
[System.Serializable]
public class SkillState
{
    public bool isOnCooldown;       //쿨타임이 돌고 있는지 여부
    public float cooldownTimer;     //쿨다운 타이머
    public bool isActive;           //스킬이 활성화 중인지 여부 (지속성 스킬)
    public float activeTimer;       //활성화 시간 타이머 (초)

    /// <summary>
    /// 쿨타임 업데이트 클래스
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateCooldown(float deltaTime)
    {
        if (isOnCooldown)
        {
            cooldownTimer -= deltaTime;
            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;
                cooldownTimer = 0;
            }
        }
    }

    /// <summary>
    /// 활성화 타이머 업데이트 클래스
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateActive(float deltaTime)
    {
        if (isActive)
        {
            activeTimer -= deltaTime;
            if (activeTimer <= 0)
            {
                isActive = false;
                activeTimer = 0;
            }
        }
    }

    /// <summary>
    /// 스킬 사용 시 쿨다운 시작하는 클래스
    /// </summary>
    /// <param name="cooldownTime"></param>
    public void StartCooldown(float cooldownTime)
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
    }

    /// <summary>
    /// (지속성)스킬 활성화 시작하는 클래스
    /// </summary>
    /// <param name="activeTime"></param>
    public void StartActive(float activeTime)
    {
        isActive = true;
        activeTimer = activeTime;
    }

    /// <summary>
    /// 쿨다운을 즉시 리셋하는 함수
    /// </summary>
    public void ResetCooldown()
    {
        isOnCooldown = false;
        cooldownTimer = 0f;
    }

    /// <summary>
    /// 활성화 상태를 즉시 종료하는 함수
    /// </summary>
    public void ResetActive()
    {
        isActive = false;
        activeTimer = 0f;
    }
}
