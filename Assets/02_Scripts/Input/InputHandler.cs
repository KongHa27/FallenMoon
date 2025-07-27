using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputHandler : MonoBehaviour
{
    /// <summary>
    /// 이동 입력 이벤트
    /// </summary>
    public abstract event Action<Vector2> OnMoveInput;

    /// <summary>
    /// 점프 입력 이벤트
    /// </summary>
    public abstract event Action OnJumpInput;

    /// <summary>
    /// 기본 공격 입력 이벤트
    /// </summary>
    public abstract event Action OnAttackInput;

    /// <summary>
    /// 스킬 1 입력 이벤트
    /// </summary>
    public abstract event Action OnSkill1Input;

    /// <summary>
    /// 스킬 2 입력 이벤트
    /// </summary>
    public abstract event Action OnSkill2Input;

    /// <summary>
    /// 이동 스킬 입력 이벤트
    /// </summary>
    public abstract event Action OnMoveSkillInput;

    /// <summary>
    /// 장비 줍기 입력 이벤트
    /// </summary>
    public abstract event Action OnPickupUseItemInput;

    /// <summary>
    /// 장비 사용 입력 이벤트
    /// </summary>
    public abstract event Action OnUseItemInput;

    /// <summary>
    /// 상호작용 입력 이벤트
    /// </summary>
    public abstract event Action OnInteractionInput;
}

