using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectButton : MonoBehaviour
{
    [Header("----- 난이도 설정 -----")]
    [SerializeField] DifficultyManager.SelectDifficulty _difficulty;

    [Header("----- 버튼 컴포넌트 -----")]
    [SerializeField] Button _button;

    private CharacterSelectUI _selectUI;

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();
    }

    private void Start()
    {
        // CharacterSelectUI 찾기
        _selectUI = FindObjectOfType<CharacterSelectUI>();

        // 버튼 이벤트 연결
        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    /// <summary>
    /// Inspector에서 호출 가능한 함수들 (각 난이도별)
    /// </summary>
    public void SelectEasy()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Easy);
    }

    public void SelectNormal()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Normal);
    }

    public void SelectHard()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Hard);
    }

    public void SelectNightmare()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Nightmare);
    }

    /// <summary>
    /// 버튼 클릭 시 호출 (코드에서 자동 연결)
    /// </summary>
    private void OnButtonClick()
    {
        SelectDifficulty(_difficulty);
    }

    /// <summary>
    /// 난이도 선택 실행
    /// </summary>
    private void SelectDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        if (_selectUI != null)
        {
            _selectUI.SelectDifficulty(difficulty);
        }
        else if (GameManager.Instance != null)
        {
            // CharacterSelectUI가 없다면 직접 GameManager 호출
            GameManager.Instance.SelectDifficulty(difficulty);
        }

        Debug.Log($"난이도 선택: {difficulty}");
    }

    /// <summary>
    /// Inspector에서 난이도 설정 (에디터 전용)
    /// </summary>
    public void SetDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        _difficulty = difficulty;
    }
}
