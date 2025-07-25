using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 마법진 상태 enum
/// </summary>
public enum MagicCirclrState
{
    Idle = 0,       //대기
    Charging = 1,   //충전 중
    Charged = 2     //충전 완료
}

/// <summary>
/// 마법진 클래스
/// </summary>
public class MagicCircle : InteractableObjects
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] MagicCircleSystem _magicCircleSystem;

    [Header("----- 마법진 스프라이트 -----")]
    [SerializeField] Sprite[] _stateSprites = new Sprite[3];

    MagicCirclrState _curState = MagicCirclrState.Idle;     //현재 상태
    public MagicCirclrState CurState => _curState;
    
    bool _hasStartedCharging = false;       //충전을 시작했는지 여부
    bool _isAnimating = false;              //애니메이션이 재생되고 있는지 여부
    float _chargingAnimationSpeed;          // 충전 중 애니메이션 속도

    protected override void Awake()
    {
        base.Awake();

        _objectType = ObjectType.MagicCircle;
        _objectName = "마법진";
        _destroyAfterInteraction = false;
    }

    protected override void Start()
    {
        base.Start();

        // 마법진 시스템 참조
        if (_magicCircleSystem == null)
            _magicCircleSystem = FindObjectOfType<MagicCircleSystem>();
    }

    /// <summary>
    /// 마법진을 초기화하는 함수
    /// </summary>
    /// <param name="system"></param>
    /// <param name="animationSpeed"></param>
    public void Initialize(MagicCircleSystem system, float animationSpeed)
    {
        _magicCircleSystem = system;
        _chargingAnimationSpeed = animationSpeed;

        //초기(Idle) 스프라이트 설정
        SetState(MagicCirclrState.Idle);
    }

    /// <summary>
    /// 마법진 상태 설정
    /// </summary>
    /// <param name="state"></param>
    public void SetState(MagicCirclrState state)
    {
        //현재 상태와 변경하려는 상태가 같으면 리턴
        if (_curState == state) return;

        _curState = state;

        //상태에 맞는 스프라이트로 스프라이트 변경
        if (_stateSprites[(int)state] != null)
            _renderer.sprite = _stateSprites[(int)state];
        else
            Debug.LogWarning($"마법진 상태 {state}에 대한 스프라이트가 존재하지 않습니다.");

        //상태에 따른 특별한 처리들 (애니메이션 처리)
        switch (state)
        {
            case MagicCirclrState.Charging:
                StartChargingAnim();
                break;

            case MagicCirclrState.Charged:
                StopChargingAnim();
                break;

            default:
                StopChargingAnim();
                break;
        }
    }

    /// <summary>
    /// 마법진 충전 애니메이션을 시작하는 함수
    /// </summary>
    void StartChargingAnim()
    {
        if (!_isAnimating)
        {
            _isAnimating = true;
            StartCoroutine(ChargingAnimRoutine());
        }
    }

    /// <summary>
    /// 마법진 충전 애니메이션을 종료하는 함수
    /// </summary>
    void StopChargingAnim()
    {
        _isAnimating = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// 마법진 충전 애니메이션 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator ChargingAnimRoutine()
    {
        float originalAlpha = _renderer.color.a;

        while (_isAnimating)
        {
            float alpha = Mathf.PingPong(Time.time * _chargingAnimationSpeed, 1f);
            Color color = _renderer.color;
            color.a = Mathf.Lerp(0.3f, originalAlpha, alpha);
            _renderer.color = color;

            yield return null;
        }
    }

    /// <summary>
    /// 마법진 상호작용 처리
    /// </summary>
    /// <param name="interactor"></param>
    protected override void OnInteract(GameObject interactor)
    {
        Hero hero = interactor.GetComponent<Hero>();
        if (hero == null) return;

        //마법진 충전이 시작하지 않았을 때
        //첫번째 상호작용 : 충전 시작
        if (!_hasStartedCharging)
        {
            _magicCircleSystem.StartCharging();
            _hasStartedCharging = true;
            SetState(MagicCirclrState.Charging);
            //디버깅
            Debug.Log("마법진 충전 시작");
        }
        //마법진 충전을 시작했고, 다시 상호작용했을 때
        //스테이지 클리어 조건을 다 달성했다면
        else if (_magicCircleSystem.CanCompleteStage())
        {
            //스테이지 클리어
            _magicCircleSystem.CompleteStage();
        }
        else
        {
            Debug.Log("마법진이 아직 완전히 충전되지 않았거나, 보스를 처치하지 않았습니다!");
        }
    }

    public override string GetInteractionText()
    {
        if (!_hasStartedCharging)
            return "[E] 마법진 충전 시작";
        else if (_magicCircleSystem.CanCompleteStage())
            return "[E] 다음 스테이지로 이동";
        else
            return "[E] 충전 중... (보스를 처치하세요)";
    }

    public override bool CanInteract
    {
        get
        {
            // 마법진은 항상 상호작용 가능
            return _canInteract;
        }
    }
}
