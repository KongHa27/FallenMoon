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
public class MagicCircle : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] MagicCircleSystem _magicCircleSystem;
    [SerializeField] SpriteRenderer _renderer;

    [Header("----- 마법진 스프라이트 -----")]
    [SerializeField] Sprite[] _stateSprites = new Sprite[3];

    MagicCirclrState _curState = MagicCirclrState.Idle;     //현재 상태
    public MagicCirclrState CurState => _curState;

    float _chargingAnimationSpeed;          // 충전 중 애니메이션 속도
    bool _hasStartedCharging = false;       //충전을 시작했는지 여부
    bool _isAnimating = false;              //애니메이션이 재생되고 있는지 여부

    /// <summary>
    /// 마법진을 초기화하는 함수
    /// </summary>
    /// <param name="system"></param>
    /// <param name="animationSpeed"></param>
    public void Initialize(MagicCircleSystem system, float animationSpeed)
    {
        _magicCircleSystem = system;
        _chargingAnimationSpeed = animationSpeed;

        _renderer = GetComponentInChildren<SpriteRenderer>();

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

        //상태에 따른 특별한 처리들
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
    /// 마법진 상태를 충전 중(Charging)으로 설정하는 함수
    /// </summary>
    public void SetChargingState()
    {
        SetState(MagicCirclrState.Charging);
    }

    /// <summary>
    /// 마법진 상태를 충전 완료(Charged)로 설정하는 함수
    /// </summary>
    public void SetChargedState()
    {
        SetState(MagicCirclrState.Charged);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //마법진 충전이 시작하지 않았을 때
            //첫번째 상호작용 : 충전 시작
            if (!_hasStartedCharging)
            {
                _magicCircleSystem.StartCharging();
                _hasStartedCharging = true;
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
    }
}
