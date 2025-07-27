using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : InputHandler
{
    public override event Action<Vector2> OnMoveInput;
    public override event Action OnJumpInput;

    public override event Action OnAttackInput;

    public override event Action OnSkill1Input;
    public override event Action OnSkill2Input;
    public override event Action OnMoveSkillInput;
    public override event Action OnPickupUseItemInput;
    public override event Action OnUseItemInput;
    public override event Action OnInteractionInput;

    Vector2 _moveInput;

    // Update is called once per frame
    void Update()
    {
        //입력 값 가져오기
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        //입력 이벤트 발행
        OnMoveInput?.Invoke(_moveInput);

        if (Input.GetButtonDown("Jump"))
            OnJumpInput?.Invoke();

        if (Input.GetButtonDown("Fire1"))
            OnAttackInput?.Invoke();

        if (Input.GetButtonDown("Skill1"))
            OnSkill1Input?.Invoke();

        if (Input.GetButtonDown("Skill2"))
            OnSkill2Input?.Invoke();

        if (Input.GetButtonDown("MoveSkill"))
            OnMoveSkillInput?.Invoke();

        if (Input.GetButtonDown("PickupItem"))
            OnPickupUseItemInput?.Invoke();

        if (Input.GetButtonDown("UseItem"))
            OnUseItemInput?.Invoke();

        if (Input.GetButtonDown("Interaction"))
            OnInteractionInput?.Invoke();
    }
}
