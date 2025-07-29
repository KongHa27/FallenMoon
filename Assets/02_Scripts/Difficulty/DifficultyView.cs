using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyView : MonoBehaviour
{
    [Header("----- UI 컴포넌트 -----")]
    [SerializeField] TextMeshProUGUI _playTimeText;          // 플레이 타임 텍스트
    [SerializeField] TextMeshProUGUI _erosionStageText;      // 침식도 단계 이름 텍스트

    [Header("----- 침식도 게이지 바 -----")]
    [SerializeField] Image _erosionGaugeBar;                 // 침식도 게이지 바 이미지 (fillAmount 사용)
    [SerializeField] Gradient _erosionGaugeGradient;         // 침식도에 따른 색상 그라데이션

    // 플레이 타임 관련
    float _gameStartTime;

    //DifficultyManager 참조 변수
    DifficultyManager _difficultyManager;

    private void Start()
    {
        // 게임 시작 시간 기록
        _gameStartTime = Time.time;

        // DifficultyManager 참조 획득
        _difficultyManager = DifficultyManager.Instance;
        if (_difficultyManager == null)
        {
            Debug.LogError("DifficultyManager를 찾을 수 없습니다!");
            return;
        }

        // 이벤트 구독
        _difficultyManager.OnErosionLevelChanged += OnErosionLevelChanged;

        // 초기 UI 설정
        UpdateErosionUI(_difficultyManager.CurrentErosionLevel);
    }

    private void Update()
    {
        UpdatePlayTimeUI();
        UpdateErosionProgressUI();
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (_difficultyManager != null)
        {
            _difficultyManager.OnErosionLevelChanged -= OnErosionLevelChanged;
        }
    }

    #region 플레이 타임 UI
    /// <summary>
    /// 플레이 타임 UI 업데이트
    /// </summary>
    void UpdatePlayTimeUI()
    {
        if (_playTimeText == null) return;

        float playTime = Time.time - _gameStartTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(playTime);

        // 분:초 형식으로 표시
        string timeString = string.Format("{0:00}:{1:00}",
            (int)timeSpan.Minutes,
            timeSpan.Seconds);

        _playTimeText.text = $"{timeString}";
    }
    #endregion

    #region 침식도 UI
    /// <summary>
    /// 침식도 레벨 변경 시 호출되는 이벤트 핸들러
    /// </summary>
    void OnErosionLevelChanged(int oldLevel, int newLevel)
    {
        UpdateErosionUI(newLevel);
    }

    /// <summary>
    /// 침식도 관련 UI 업데이트
    /// </summary>
    void UpdateErosionUI(int currentLevel)
    {
        // 침식도 단계 이름 업데이트
        if (_erosionStageText != null)
        {
            string stageName = _difficultyManager.GetErosionStageName();
            _erosionStageText.text = stageName;

            // 단계에 따른 텍스트 색상 변경
            _erosionStageText.color = GetStageColor(stageName);
        }
    }

    /// <summary>
    /// 침식도 진행도 UI 업데이트 (실시간)
    /// </summary>
    void UpdateErosionProgressUI()
    {
        if (_difficultyManager == null || _erosionGaugeBar == null) return;

        // 부드러운 게이지 바 채우기를 위한 계산
        float smoothFillAmount = CalculateSmoothErosionProgress();

        _erosionGaugeBar.fillAmount = smoothFillAmount;

        // 진행도에 따른 색상 변경
        if (_erosionGaugeGradient != null)
        {
            _erosionGaugeBar.color = _erosionGaugeGradient.Evaluate(smoothFillAmount);
        }
    }

    /// <summary>
    /// 부드러운 침식도 진행도 계산
    /// </summary>
    float CalculateSmoothErosionProgress()
    {
        int currentLevel = _difficultyManager.CurrentErosionLevel;
        float erosionProgress = _difficultyManager.ErosionProgress;

        // 종말 단계 기준점 계산 (침식도 25부터 종말 시작)
        float apocalypseStartLevel = 25f;

        // 현재 진행도 (레벨 + 현재 레벨 내 진행도)
        float totalCurrentProgress = currentLevel + erosionProgress;

        // 종말 단계에 도달했으면 게이지 100%
        if (currentLevel >= apocalypseStartLevel)
        {
            return 1f;
        }

        // 종말 단계 전까지의 진행도를 0~1 사이로 정규화
        float normalizedProgress = totalCurrentProgress / apocalypseStartLevel;

        return Mathf.Clamp01(normalizedProgress);
    }

    /// <summary>
    /// 침식도 단계에 따른 텍스트 색상 반환
    /// </summary>
    Color GetStageColor(string stageName)
    {
        switch (stageName)
        {
            case "쉬움": return Color.green;
            case "보통": return Color.yellow;
            case "어려움": return new Color(1f, 0.5f, 0f); // 주황색
            case "매우 어려움": return Color.red;
            case "광기": return Color.magenta;
            case "불가능": return new Color(0.5f, 0f, 0.5f); // 보라색
            case "종말": return Color.black;
            default: return Color.white;
        }
    }
    #endregion

    #region 공개 메서드
    /// <summary>
    /// 플레이 타임 리셋 (새 게임 시작 시)
    /// </summary>
    public void ResetPlayTime()
    {
        _gameStartTime = Time.time;
    }
    #endregion
}
