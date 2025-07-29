using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] Hero _hero;
    [SerializeField] ItemInteractionHandler _itemHandler;
    [SerializeField] InteractionManager _interactionManager;

    // Start is called before the first frame update
    void Start()
    {
        //이동 입력 이벤트 구독
        _inputHandler.OnMoveInput += OnMoveInput;
        //점프 입력 이벤트 구독
        _inputHandler.OnJumpInput += OnJumpInput;
        //공격 입력 이벤트 구독
        _inputHandler.OnAttackInput += OnAttackInput;
        //스킬 입력 이벤트 구독
        _inputHandler.OnSkill1Input += OnSkill1Input;
        _inputHandler.OnSkill2Input += OnSkill2Input;
        _inputHandler.OnMoveSkillInput += OnMoveSkillInput;
        //장비 아이템 줍기 입력 이벤트 구독
        _inputHandler.OnPickupUseItemInput += OnPickupUseItem;
        //장비 아이템 사용 입력 이벤트 구독
        _inputHandler.OnUseItemInput += OnUseItemInput;
        //상호작용 입력 이벤트 구독
        _inputHandler.OnInteractionInput += OnInteractionInput;

        StartCoroutine(DelayedInitializeHero());
        _interactionManager.Initialize(_hero);

    }

    IEnumerator DelayedInitializeHero()
    {
        yield return null;
        _hero.InitializeWithPrefab();
    }

    void OnMoveInput(Vector2 inputVector)
    {
        _hero.HandleInput(inputVector);
    }

    void OnJumpInput()
    {
        _hero.Jump();
    }

    void OnAttackInput()
    {
        _hero.Attack();
    }

    void OnSkill1Input()
    {
        _hero.UseSkill1();
    }

    void OnSkill2Input()
    {
        _hero.UseSkill2();
    }

    void OnMoveSkillInput()
    {
        _hero.UseMoveSkill();
    }

    void OnPickupUseItem()
    {
        _itemHandler.HandlePickupInput();
    }

    void OnUseItemInput()
    {
        _hero.UseUsableItem();
    }

    void OnInteractionInput()
    {
        _interactionManager.TryInteract();
    }
}
