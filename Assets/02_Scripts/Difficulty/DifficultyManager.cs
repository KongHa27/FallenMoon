using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

/// <summary>
/// 선택,가변 난이도와 그에 따른 데이터들을 관리하는 클래스
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    /// <summary>
    /// 선택 난이도 종류
    /// </summary>
    public enum SelectDifficulty
    {
        Easy = 0,       // 쉬움 (만월)
        Normal = 1,     // 보통 (하현)
        Hard = 2,       // 어려움 (그믐)
        Nightmare = 3   // 악몽 (월식)
    }

    [Header("----- 침식도 설정 -----")]
    [SerializeField] float _erosionIncreaseInterval = 75f;  // 침식도 증가 간격 (초)
    [SerializeField] int _maxErosionLevel = 99;             // 최대 침식도 레벨

    [Header("----- 침식도별 어둠 설정 -----")]
    [SerializeField] Color _baseDarknessColor = new Color(0.1f, 0.1f, 0.15f, 0.3f);  // 기본 어둠 색상
    [SerializeField] float _darknessIncreaseRate = 0.05f;  // 침식도 단계별 어둠 증가율
    [SerializeField] float _maxDarknessAlpha = 0.8f;       // 최대 어둠 투명도

    [Header("----- 디버그용 -----")]
    [SerializeField] bool _debugMode = false;
    [SerializeField] KeyCode _increaseErosionKey = KeyCode.F1;
    [SerializeField] KeyCode _decreaseErosionKey = KeyCode.F2;

    // 현재 난이도 상태
    SelectDifficulty _selectedDifficulty;   //선택 난이도
    int _curErosionLevel = 1;               //가변 난이도
    float _erosionTimer = 0f;               //가변 난이도 증가 타이머

    //Smart Lighting2D 에셋 참조
    [SerializeField] LightingManager2D _lightingManager;

    /// <summary>
    /// 선택 난이도 변화 이벤트
    /// </summary>
    public event Action<SelectDifficulty> OnSelectedDifficultyChanged;

    /// <summary>
    /// 가변 난이도(침식도) 레벨 변화 이벤트
    /// (이전 레벨, 새 레벨)
    /// </summary>
    public event Action<int, int> OnErosionLevelChanged;

    /// <summary>
    /// 난이도 매니저 싱글톤
    /// </summary>
    public static DifficultyManager Instance { get; private set; }

    // 프로퍼티
    public SelectDifficulty SelectedDifficulty => _selectedDifficulty;
    public int CurrentErosionLevel => _curErosionLevel;
    public float ErosionProgress => _erosionTimer / _erosionIncreaseInterval;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSelectedDifficulty();

            _lightingManager = FindObjectOfType<LightingManager2D>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(ErosionTimerCoroutine());

        SetLightByErosionLevel(_curErosionLevel);
    }

    private void Update()
    {
        // 디버그 용
        if (_debugMode)
        {
            if (Input.GetKeyDown(_increaseErosionKey))
            {
                IncreaseErosionLevel();
            }
            else if (Input.GetKeyDown(_decreaseErosionKey))
            {
                DecreaseErosionLevel();
            }
        }
    }

    #region 선택 난이도
    /// <summary>
    /// 선택 난이도 설정 (게임 시작 전 선택)
    /// </summary>
    public void SetSelectedDifficulty(SelectDifficulty difficulty)
    {
        _selectedDifficulty = difficulty;
        SaveSelectedDifficulty();
        OnSelectedDifficultyChanged?.Invoke(_selectedDifficulty);

        Debug.Log($"선택 난이도 설정: {GetDifficultyName(difficulty)}");
    }

    /// <summary>
    /// PlayerPrefs에서 선택 난이도 불러오기
    /// </summary>
    void LoadSelectedDifficulty()
    {
        int savedDifficulty = PlayerPrefs.GetInt("SelectedDifficulty", 1); // 기본값: Normal
        _selectedDifficulty = (SelectDifficulty)savedDifficulty;
    }

    /// <summary>
    /// PlayerPrefs에 선택 난이도 저장
    /// </summary>
    void SaveSelectedDifficulty()
    {
        PlayerPrefs.SetInt("SelectedDifficulty", (int)_selectedDifficulty);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 난이도 이름 반환 (디버그 용)
    /// </summary>
    public string GetDifficultyName(SelectDifficulty difficulty)
    {
        switch (difficulty)
        {
            case SelectDifficulty.Easy: return "만월";
            case SelectDifficulty.Normal: return "하현";
            case SelectDifficulty.Hard: return "그믐";
            case SelectDifficulty.Nightmare: return "월식";
            default: return "알 수 없음";
        }
    }
    #endregion

    #region 침식도 관련
    /// <summary>
    /// 침식도 증가 타이머 코루틴
    /// </summary>
    IEnumerator ErosionTimerCoroutine()
    {
        while (_curErosionLevel < _maxErosionLevel)
        {
            _erosionTimer += Time.deltaTime;

            float adjustedInterval = GetAdjustedErosionInterval();

            if (_erosionTimer >= adjustedInterval)
            {
                IncreaseErosionLevel();
                _erosionTimer = 0f;

                SetLightByErosionLevel(_curErosionLevel);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 선택 난이도에 따라 조정된 침식도 증가 간격 반환
    /// </summary>
    float GetAdjustedErosionInterval()
    {
        return _erosionIncreaseInterval / GetErosionIncreaseMultiplier();
    }

    /// <summary>
    /// 침식도 레벨 증가
    /// </summary>
    public void IncreaseErosionLevel()
    {
        if (_curErosionLevel < _maxErosionLevel)
        {
            int oldLevel = _curErosionLevel;
            _curErosionLevel++;
            OnErosionLevelChanged?.Invoke(oldLevel, _curErosionLevel);

            Debug.Log($"침식도 증가: {oldLevel} → {_curErosionLevel}");
        }
    }

    /// <summary>
    /// 침식도 레벨 감소 (디버그용)
    /// </summary>
    public void DecreaseErosionLevel()
    {
        if (_curErosionLevel > 1)
        {
            int oldLevel = _curErosionLevel;
            _curErosionLevel--;
            OnErosionLevelChanged?.Invoke(oldLevel, _curErosionLevel);

            Debug.Log($"침식도 감소: {oldLevel} → {_curErosionLevel}");
        }
    }

    /// <summary>
    /// 침식도 초기화 (새 게임 시작 시)
    /// </summary>
    public void ResetErosion()
    {
        int oldLevel = _curErosionLevel;
        _curErosionLevel = 1;
        _erosionTimer = 0f;
        OnErosionLevelChanged?.Invoke(oldLevel, _curErosionLevel);

        Debug.Log("침식도 초기화");
    }

    /// <summary>
    /// 현재 침식도 단계 이름 반환
    /// </summary>
    public string GetErosionStageName()
    {
        if (_curErosionLevel <= 4) return "쉬움";
        else if (_curErosionLevel <= 8) return "보통";
        else if (_curErosionLevel <= 12) return "어려움";
        else if (_curErosionLevel <= 16) return "매우 어려움";
        else if (_curErosionLevel <= 20) return "광기";
        else if (_curErosionLevel <= 24) return "불가능";
        else return "종말";
    }

    void SetLightByErosionLevel(int level)
    {
        if (_lightingManager == null)
        {
            _lightingManager = FindObjectOfType<LightingManager2D>();

            if (_lightingManager == null)
            {
                Debug.LogWarning("LightingManager2D를 찾을 수 없습니다!");
                return;
            }
        }

        //침식도에 따른 어둠 강도 계산
        float darknessIntensity = CalculateDarknessIntensity(level);

        //어둠 강도에 따른 어둠 색상 계산
        Color darknessColor = CalculateDarknessColor(darknessIntensity);
        //어둠 색상 적용
        ApplyDarknessToLighting(darknessColor);
    }

    /// <summary>
    /// 침식도 레벨(단계)에 따른 어둠 강도 계산
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    float CalculateDarknessIntensity(int level)
    {
        // 침식도 단계별로 어둠 강도 조정
        float baseIntensity = 0.1f;

        //쉬움
        if (level <= 4)
            return baseIntensity;
        //보통
        else if (level <= 8)
            return baseIntensity + 0.15f;
        else if (level <= 12)    // 어려움
            return baseIntensity + 0.3f;
        else if (level <= 16)    // 매우 어려움
            return baseIntensity + 0.45f;
        else if (level <= 20)    // 광기
            return baseIntensity + 0.6f;
        else if (level <= 24)    // 불가능
            return baseIntensity + 0.7f;
        else                           // 종말
            return _maxDarknessAlpha;
    }

    /// <summary>
    /// 어둠 강도에 따른 색상 계산
    /// </summary>
    /// <param name="intensity"></param>
    /// <returns></returns>
    Color CalculateDarknessColor(float intensity)
    {
        Color darknessColor = _baseDarknessColor;

        // 침식도가 높아질수록 더 어둡고 불길한 색상으로 변화
        if (intensity > 0.5f)
        {
            // 높은 침식도에서는 붉은 기운 추가
            darknessColor.r += (intensity - 0.5f) * 0.3f;
            darknessColor.g -= (intensity - 0.5f) * 0.1f;
        }

        // 알파값(투명도) 조정으로 어둠 강도 제어
        darknessColor.a = Mathf.Clamp(intensity, 0f, _maxDarknessAlpha);

        return darknessColor;
    }

    /// <summary>
    /// Smart Lighting 2D에 어둠 설정 적용
    /// </summary>
    void ApplyDarknessToLighting(Color darknessColor)
    {
        // Smart Lighting 2D의 Profile을 통해 어둠 색상 설정
        if (_lightingManager != null && _lightingManager.profile != null)
        {
            _lightingManager.profile.DarknessColor = darknessColor;

            // 실시간으로 변경사항 적용
            _lightingManager.UpdateProfile();
        }
        else
        {
            Debug.LogWarning("LightingManager2D 또는 Profile이 설정되지 않았습니다!");
        }
    }
    #endregion

    #region 선택 난이도 조정 능력치 계산
    /// <summary>
    /// 선택 난이도에 따른 적 공격력 배수 반환
    /// Easy 난이도일 때만 조정 -35%
    /// </summary>
    public float GetEnemyDamageMultiplier()
    {
        if (_selectedDifficulty == SelectDifficulty.Easy)
            return 0.65f;
        else
            return 1f;
    }

    /// <summary>
    /// 선택 난이도에 따른 광원 게이지 감소량 배수 반환
    /// </summary>
    public float GetLightDecreaseMultiplier()
    {
        switch (_selectedDifficulty)
        {
            case SelectDifficulty.Easy: return 0.75f;      // -25%
            case SelectDifficulty.Normal: return 1f;       // 기본
            case SelectDifficulty.Hard: return 1.25f;      // +25%
            case SelectDifficulty.Nightmare: return 1.5f;  // +25%
            default: return 1f;
        }
    }

    /// <summary>
    /// 선택 난이도에 따른 침식도 상승 속도 배수 반환
    /// </summary>
    public float GetErosionIncreaseMultiplier()
    {
        switch (_selectedDifficulty)
        {
            case SelectDifficulty.Easy: return 0.75f;     // -25%
            case SelectDifficulty.Normal: return 1f;       // 기본
            case SelectDifficulty.Hard: return 1.25f;      // +25%
            case SelectDifficulty.Nightmare: return 1.5f;  // +50%
            default: return 1f;
        }
    }
    #endregion
}
