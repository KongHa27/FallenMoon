using FunkyCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] Hero _hero;
    [SerializeField] Light2D _light2D;

    [Header("----- 광원 게이지 설정 -----")]
    [SerializeField] float _maxGauge;               //최대 게이지
    [SerializeField] float _curGauge;
    [SerializeField] float _baseGaugeTimeDecrease;  //게이지 초당 감소량
    [SerializeField] float _gaugeHitDecrease;       //게이지 피격 감소량

    [Header("----- 광원 크기 -----")]
    [SerializeField] float _maxSize;                //광원 최대 사이즈
    float _curSize;                                 //광원 현재 사이즈

    [Header("----- 상태 관리 -----")]
    [SerializeField] bool _isLightOn;               //광원 온오프 여부
    [SerializeField] float _hitSpan;                //피격 쿨타임
    float _hitTimer;                                //피격 타이머

    bool _isLightOffRoutineRunning = false;         //광원 0일때 실행되는 코루틴 변수

    float _curGaugeTimeDecrease;                    //난이도에 따라 적용되는 감소량 배수


    private void Awake()
    {
        _hero = GetComponentInParent<Hero>();
        _light2D = GetComponent<Light2D>();
    }

    private void Start()
    {
        //초기화
        InitializeLightSystem();
        
        //광원 게이지 감소 코루틴 시작
        StartCoroutine(DecreaseGaugeRoutine());

        //난이도 매니저 이벤트 구독 (선택 난이도)
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.OnSelectedDifficultyChanged += OnDifficultyChanged;
            UpdateDifficultyMultipliers();
        }
    }

    private void OnDestroy()
    {
        //이벤트 구독 해제
        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.OnSelectedDifficultyChanged -= OnDifficultyChanged;
    }

    private void Update()
    {
        //디버그용
        if (Input.GetKeyDown(KeyCode.Alpha2))
            AddGauge(20f);

        SetLightSize();

        if (_isLightOn == false && !_isLightOffRoutineRunning)
            StartCoroutine(LightOffRoutine());

        if (_hitTimer > 0)
            _hitTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 광원 게이지 시스템 초기화 함수
    /// </summary>
    void InitializeLightSystem()
    {
        _curGauge = _maxGauge / 2;
        _curSize = _maxSize / 2;
        _isLightOn = true;
        _hitTimer = 0;

        UpdateDifficultyMultipliers();
    }

    void OnDifficultyChanged(DifficultyManager.SelectDifficulty selectDifficulty)
    {
        UpdateDifficultyMultipliers();
        Debug.Log($"광원 시스템 난이도 적용 성공: {DifficultyManager.Instance.GetDifficultyName(selectDifficulty)}");
    }

    /// <summary>
    /// 선택 난이도에 따른 배수 업데이트
    /// </summary>
    void UpdateDifficultyMultipliers()
    {
        if (DifficultyManager.Instance != null)
        {
            float multiplier = DifficultyManager.Instance.GetLightDecreaseMultiplier();
            _curGaugeTimeDecrease = _baseGaugeTimeDecrease * multiplier;
        }
        else
        {
            _curGaugeTimeDecrease = _baseGaugeTimeDecrease;
        }
    }

    /// <summary>
    /// 시간에 따라 게이지가 감소하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator DecreaseGaugeRoutine()
    { 
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (_isLightOn)
            {
                _curGauge -= _curGaugeTimeDecrease;

                if (_curGauge <= 0)
                {
                    _curGauge = 0;
                    _isLightOn = false;
                }
            }

        }
    }

    /// <summary>
    /// 게이지가 0일 때 호출되는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator LightOffRoutine()
    {
        _isLightOffRoutineRunning = true;

        while (_isLightOn == false)
        {
            _hero.TakeHitByDarkness();

            yield return new WaitForSeconds(1f);

            if (_curGauge > 0)
            {
                _isLightOn = true;
            }
        }

        _isLightOffRoutineRunning = false;
    }

    /// <summary>
    /// 게이지를 증가시키는 함수
    /// </summary>
    /// <param name="amount">증가할 양</param>
    public void AddGauge(float amount)
    {
        _curGauge = Mathf.Min(_curGauge + amount, _maxGauge);

        if (_curGauge > 0 && !_isLightOn)
            _isLightOn = true;
    }

    /// <summary>
    /// 피격 시 게이지를 감소시키는 함수
    /// </summary>
    public void OnHit()
    {
        if (_hitTimer <= 0)
        {
            _curGauge -= _gaugeHitDecrease;

            if (_curGauge <= 0)
            {
                _curGauge = 0;
                _isLightOn = false;
            }

            _hitTimer = _hitSpan;
        }
    }

    /// <summary>
    /// 게이지에 따라 광원 사이즈를 설정하는 함수
    /// </summary>
    void SetLightSize()
    {
        float ratio = _curGauge / _maxGauge;

        _curSize = Mathf.Min(_maxSize * ratio, _maxSize);

        _light2D.size = _curSize;
    }

    /// <summary>
    /// 현재 게이지 반환(%) - UI용
    /// </summary>
    /// <returns></returns>
    public float GetGaugeRatio()
    {
        return _curGauge / _maxGauge;
    }
}
