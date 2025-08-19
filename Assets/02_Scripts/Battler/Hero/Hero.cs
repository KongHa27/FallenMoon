using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, ILadderUser
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Mover _mover;
    [SerializeField] Jumper _jumper;
    [SerializeField] LadderMover _ladderMover;
    [SerializeField] LightController _light;

    [Header("----- 프리팹 컴포넌트 참조 -----")]
    [SerializeField] HeroModel _model;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Animator _animator;
    [SerializeField] SkillManager _skillManager;
    [SerializeField] AttackSystem _attackSystem;

    [Header("----- 사다리 설정 -----")]
    [SerializeField] LadderUserState _ladderState = new LadderUserState();

    [Header("----- 아이템 시스템 -----")]
    [SerializeField] PlayerInventory _inventory;

    [Header("----- UI -----")]
    [SerializeField] HeroStatusView _statusView;
    [SerializeField] HeroSkillView _basicAttackView;
    [SerializeField] HeroSkillView _skill1View;
    [SerializeField] HeroSkillView _skill2View;
    [SerializeField] HeroSkillView _moveSkillView;

    Camera _camera;
    Vector2 _curInput;
    CharacterData _curCharData;

    //ILadderUser 인터페이스 구현
    public bool CanUseLadder => true;
    public bool IsOnLadder => _ladderState.isOnLadder;
    public bool IsClimbing => _ladderState.isClimbing;
    //

    /// <summary>
    /// 경험치 변화 이벤트
    /// </summary>
    public event Action<float, float> OnExpChanged
    {
        add => _model.OnExpChanged += value;
        remove => _model.OnExpChanged -= value;
    }

    /// <summary>
    /// 레벨 변화 이벤트
    /// </summary>
    public event Action<int, int> OnLevelChanged
    {
        add => _model.OnLevelChanged += value;
        remove => _model.OnLevelChanged -= value;
    }

    /// <summary>
    /// 스킬 쿨타임 변화 이벤트
    /// </summary>
    public event Action<SkillManager.SkillType, float, float> OnSkillCooldownChanged
    {
        add => _skillManager.OnSkillCooldownChanged += value;
        remove => _skillManager.OnSkillCooldownChanged -= value;
    }

    public CharacterData GetCurCharData()
    {
        return _curCharData;
    }

    public void InitializeWithPrefab()
    {
        if (GameManager.Instance != null)
            _curCharData = GameManager.Instance.GetSelectedCharacterData();

        _model = GetComponentInChildren<HeroModel>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _skillManager = GetComponentInChildren<SkillManager>();
        _attackSystem = GetComponentInChildren<AttackSystem>();

        if (!CheckComponents())
            return;

        ApplyCharacterData();

        Initialize();
    }

    bool CheckComponents()
    {
        if (_model == null)
        {
            Debug.LogError("HeroModel을 찾을 수 없습니다!");
            return false;
        }

        if (_renderer == null)
        {
            Debug.LogError("SpriteRenderer를 찾을 수 없습니다!");
            return false;
        }

        if (_skillManager == null)
        {
            Debug.LogError("SkillManager를 찾을 수 없습니다!");
            return false;
        }

        return true;
    }

    private void ApplyCharacterData()
    {
        if (_curCharData != null && _curCharData.HeroData != null && _model != null)
        {
            // HeroModel에 캐릭터별 HeroData 설정
            _model.SetHeroData(_curCharData.HeroData);

            Debug.Log($"캐릭터 데이터 적용 완료: {_curCharData.CharacterName}");
        }
        else
        {
            Debug.LogWarning("캐릭터 데이터가 없어 기본 설정을 사용합니다.");
        }
    }

    /// <summary>
    /// Hero 초기화
    /// </summary>
    void Initialize()
    {
        _camera = Camera.main;
        if (_camera == null)
            _camera = FindAnyObjectByType<Camera>();

        if (_inventory == null)
            _inventory = GetComponent<PlayerInventory>();

        _model.OnSpeedChanged += _mover.SetSpeed;
        _model.OnPowerChanged += _jumper.SetPower;
        _model.OnDead += OnDead;

        _jumper.OnJumpStateChanged += OnJumpStateChanged;

        //UI
        _model.OnHpChanged += _statusView.SetHpText;
        _model.OnGoldChanged += _statusView.SetGoldText;
        _model.OnExpChanged += _statusView.SetExpBar;
        _model.OnLevelChanged += (_, newLevel) => _statusView.SetLevelText(newLevel);

        if (_ladderMover != null)
            _ladderMover.OnLadderStateChanged += OnLadderStateChanged;

        _skillManager.Initialize(transform);

        if (_attackSystem is AdventurerAttack rangedAttack)
            _basicAttackView.InitializeAsBasicAttack(rangedAttack);

        _skill1View.Initialize(SkillManager.SkillType.Skill1, _skillManager);
        _skill2View.Initialize(SkillManager.SkillType.Skill2, _skillManager);
        _moveSkillView.Initialize(SkillManager.SkillType.MoveSkill, _skillManager);

        _model.Initialize();
    }

    private void Update()
    {
        UpdateFacingDirection();
        _statusView.SetLightGauge(_light.GetGaugeRatio());
    }

    #region --------- 이동 ----------
    /// <summary>
    /// 입력에 따라 사다리, 이동 함수를 실행하는 함수
    /// </summary>
    /// <param name="input"></param>
    public void HandleInput(Vector2 input)
    {
        _curInput = input;

        if (_ladderState.isClimbing)
            LadderMove(_curInput);
        else
            Move(_curInput);
    }

    /// <summary>
    /// 이동 함수
    /// </summary>
    /// <param name="dir"></param>
    public void Move(Vector3 dir)
    {
        //이동을 차단하는 스킬이 활성화 되어 있으면 리턴
        if (IsMovementBlocked()) return;

        _mover.Move(dir);
    }

    /// <summary>
    /// 마우스 위치에 따라 캐릭터가 바라보는 방향을 업데이트
    /// </summary>
    void UpdateFacingDirection()
    {
        if (_camera == null) return;

        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z; // Z축은 캐릭터와 같게 설정

        // 캐릭터와 마우스 위치의 X축 차이 계산
        float directionX = mouseWorldPos.x - transform.position.x;

        // 마우스가 캐릭터보다 오른쪽에 있으면 오른쪽을 향하고, 왼쪽에 있으면 왼쪽을 향함
        if (directionX > 0)
            _renderer.flipX = false; // 오른쪽을 향함
        else if (directionX < 0)
            _renderer.flipX = true;  // 왼쪽을 향함
        // directionX가 0에 가까우면 방향을 바꾸지 않음 (현재 방향 유지)
    }

    /// <summary>
    ///  사다리 이동
    /// </summary>
    /// <param name="input"></param>
    void LadderMove(Vector2 input)
    {
        //사다리에서 좌우 이동 입력 시 사다리 내리기
        if (Mathf.Abs(input.x) > 0.1f && _ladderState != null && _ladderState.CurLadder.CanExitAnyWhere)
        {
            ExitLadder();
            Move(input);
        }
        //사다리 위아래 이동
        else
        {
            if (_ladderMover != null)
                _ladderMover.ClimbLadder(input.y);
        }
    }

    /// <summary>
    /// 점프 함수
    /// </summary>
    public void Jump()
    {
        //사다리에서 점프 시 사다리 내리기
        if (_ladderState.isClimbing)
            _ladderMover.ExitLadder();

        _jumper.Jump();
    }

    /// <summary>
    /// 점프 상태 변경 함수
    /// </summary>
    /// <param name="state"></param>
    void OnJumpStateChanged(JumpState state)
    {
        if (state == JumpState.Climbing && _ladderState.isOnLadder)
            _ladderState.isClimbing = true;
        else if (state != JumpState.Climbing && _ladderState.isClimbing)
            _ladderState.isClimbing = false;

        //각 상태에 맞는 애니메이터 파라미터 연결
        switch (state)
        {
            case JumpState.Grounded:
                break;
            case JumpState.Jumping:
                break;
            case JumpState.Falling:
                break;
            case JumpState.Climbing:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 사다리 상태 변경 함수
    /// </summary>
    /// <param name="isOnLadder"></param>
    void OnLadderStateChanged(bool isOnLadder)
    {
        if (!isOnLadder)
            _ladderState.isClimbing = false;
    }

    //ILadderUser 인터페이스 구현
    public void EnterLadder(LadderSystem ladderSystem)
    {
        _ladderState.isOnLadder = true;
        _ladderState.CurLadder = ladderSystem;
        _ladderState.LastEnterTime = Time.time;

        if (_ladderMover != null)
            _ladderMover.SetLadderState(true);
    }

    public void ExitLadder()
    {
        if (_ladderState.CurLadder != null)
            _ladderState.CurLadder.ForceExitLadder(this);

        _ladderState.Reset();

        if (_ladderMover != null)
            _ladderMover.SetLadderState(false);
    }

    public void OnLadderStateChanged(bool isOnLadder, LadderSystem ladderSystem)
    {
        if (isOnLadder)
            EnterLadder(ladderSystem);
        else
            ExitLadder();
    }
    #endregion

    #region --------- 스킬 ----------
    /// <summary>
    /// 스킬1을 사용하는 함수
    /// </summary>
    public void UseSkill1()
    {
        _skillManager.UseSkill(SkillManager.SkillType.Skill1);
    }

    /// <summary>
    /// 스킬2를 사용하는 함수
    /// </summary>
    public void UseSkill2()
    {
        _skillManager.UseSkill(SkillManager.SkillType.Skill2);
    }

    /// <summary>
    /// 현재 이동이 차단되어 있는지 확인
    /// </summary>
    /// <returns></returns>
    private bool IsMovementBlocked()
    {
        if (_skillManager == null)
            return false;

        // 모든 스킬을 체크해서 이동을 차단하는 스킬이 있는지 확인
        foreach (SkillManager.SkillType skillType in System.Enum.GetValues(typeof(SkillManager.SkillType)))
        {
            var skill = _skillManager.GetSkill(skillType);
            if (skill != null && skill.IsBlockingMovement)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 이동 스킬을 사용하는 함수
    /// </summary>
    public void UseMoveSkill()
    {
        _skillManager.UseSkill(SkillManager.SkillType.MoveSkill);
    }

    public bool CanUseSkill(SkillManager.SkillType skillType)
    {
        return _skillManager.CanUseSkill(skillType);
    }

    public SkillBase GetSkill(SkillManager.SkillType skillType)
    {
        return _skillManager.GetSkill(skillType);
    }
    #endregion

    public void UseUsableItem()
    {
        _inventory.UseEquippedItem();
    }

    public PlayerInventory GetInventory()
    {
        return _inventory;
    }

    /// <summary>
    /// 공격할 때 실행되는 함수
    /// </summary>
    /// <param name="damageable"></param>
    public void Attack()
    {
        if (_attackSystem != null && _attackSystem.CanAttack())
        {
            _attackSystem.PerformAttack();
        }

    }

    #region --------- 피해 ----------
    /// <summary>
    /// 피해를 받으면 실행되는 함수
    /// </summary>
    /// <param name="damage"></param>
    public void TakeHit(float damage)
    {
        _model.TakeHit(damage);
        _light.OnHit();
    }

    /// <summary>
    /// 광원 게이지가 0에 수렴 시 실행되는 함수
    /// </summary>
    public void TakeHitByDarkness()
    {
        _model.TakeHitByDarkness();
    }

    /// <summary>
    /// 죽었을 떄 실행되는 함수
    /// </summary>
    void OnDead()
    {
        Debug.Log("죽음");
        //죽음 처리
    }
    #endregion

    /// <summary>
    /// 경험치를 추가하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddExp(float amount)
    {
        _model.AddExp(amount);
    }

    /// <summary>
    /// 골드를 추가하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddGold(int amount)
    {
        _model.AddGold(amount);
    }

    /// <summary>
    /// 광원 게이지를 추가하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddLightGauge(float amount)
    {
        _light.AddGauge(amount);
    }
}
