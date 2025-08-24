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

    [Header("----- 마법진 연출 -----")]
    [SerializeField] MagicCircleCtrl _magicCircleCtrl;
    [SerializeField] SpriteRenderer _ringRenderer;
    [SerializeField] SpriteRenderer _coreRenderer;
    [SerializeField] Transform _runesParent;
    [SerializeField] SpriteRenderer[] _runeRenderers; // 룬들의 스프라이트 렌더러 배열

    MagicCirclrState _curState = MagicCirclrState.Idle;     //현재 상태
    public MagicCirclrState CurState => _curState;
    
    bool _hasStartedCharging = false;       //충전을 시작했는지 여부

    protected override void Awake()
    {
        base.Awake();

        _objectType = ObjectType.MagicCircle;
        _objectName = "마법진";
        _destroyAfterInteraction = false;

        if (_magicCircleCtrl == null)
        {
            _magicCircleCtrl = GetComponent<MagicCircleCtrl>();
        }

        if (_runesParent != null)
            _runeRenderers = _runesParent.GetComponentsInChildren<SpriteRenderer>();
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
    public void Initialize(MagicCircleSystem system)
    {
        _magicCircleSystem = system;

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

        switch (state)
        {
            case MagicCirclrState.Idle:
                // 연출 스크립트 비활성화
                if (_magicCircleCtrl != null)
                    _magicCircleCtrl.enabled = false;
                // 모든 스프라이트 색상을 회색으로 변경
                SetAllSpriteColors(Color.grey);
                break;

            case MagicCirclrState.Charging:
                // 모든 스프라이트 색상을 흰색으로 변경
                SetAllSpriteColors(Color.white);
                // 연출 스크립트 활성화
                if (_magicCircleCtrl != null)
                    _magicCircleCtrl.enabled = true;
                break;

            case MagicCirclrState.Charged:
                // 모든 스프라이트 색상을 노란색으로 변경
                SetAllSpriteColors(Color.yellow);
                // 연출 스크립트는 계속 활성화된 상태를 유지 (충전 완료 연출)
                if (_magicCircleCtrl != null)
                    _magicCircleCtrl.enabled = true;
                break;
        }
    }

    void SetAllSpriteColors(Color color)
    {
        if (_ringRenderer != null)
            _ringRenderer.color = color;

        if (_coreRenderer != null)
            _coreRenderer.color = color;

        if (_runeRenderers != null)
        {
            foreach (var runeRenderer in _runeRenderers)
            {
                // RuneLightUp의 FadeIn 효과와 충돌하지 않도록 알파값은 1로 고정
                Color runeColor = color;
                // Idle 상태일 때는 룬도 완전히 회색이어야 하므로 알파값 통일
                if (_curState == MagicCirclrState.Idle)
                {
                    runeRenderer.color = runeColor;
                }
                else // Charging, Charged 상태에서는 색상만 바꾸고 알파값은 RuneLightUp에 맡김
                {
                    Color originalRuneColor = runeRenderer.color;
                    runeRenderer.color = new Color(color.r, color.g, color.b, originalRuneColor.a);
                }
            }
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
        // 아직 충전을 시작하지 않은 경우
        if (!_hasStartedCharging)
        {
            return "[E] 마법진 충전 시작";
        }

        // 충전을 시작한 경우 - 현재 상태와 보스 처치 여부에 따라 분기
        switch (_curState)
        {
            case MagicCirclrState.Charging:
                // 충전 중인 경우
                return "[E] 충전 중... (보스를 처치하세요)";

            case MagicCirclrState.Charged:
                // 충전이 완료된 경우
                if (_magicCircleSystem.CanCompleteStage())
                {
                    // 충전 완료 + 보스 처치 완료
                    return "[E] 다음 스테이지로 이동";
                }
                else
                {
                    // 충전 완료했지만 보스를 아직 처치하지 않음
                    return "[E] 보스를 처치하세요";
                }

            default:
                // 기본값 (Idle 상태 등)
                return "[E] 마법진과 상호작용";
        }
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
