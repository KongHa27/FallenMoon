using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("----- 캐릭터 정보 UI -----")]
    [SerializeField] TextMeshProUGUI _characterNameText;
    [SerializeField] TextMeshProUGUI _characterDescriptionText;
    [SerializeField] Image _characterIconImage;
    [SerializeField] Image[] _starImages; // 별점 UI (5개)

    [Header("----- 캐릭터 선택 버튼들 -----")]
    [SerializeField] CharacterSelectButton[] _characterButtons;

    [Header("----- 난이도 선택 UI -----")]
    [SerializeField] Button[] _difficultyButtons; // 4개 난이도 버튼
    [SerializeField] TextMeshProUGUI _selectedDifficultyText;
    [SerializeField] Color _selectedButtonColor = Color.yellow;
    [SerializeField] Color _normalButtonColor = Color.white;

    [Header("----- 게임 시작 버튼 -----")]
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _backButton;

    // 현재 선택 상태
    private int _currentCharacterIndex = 0;
    private DifficultyManager.SelectDifficulty _currentDifficulty = DifficultyManager.SelectDifficulty.Normal;

    private void Start()
    {
        InitializeUI();
        SetupButtons();

        // 초기 캐릭터 선택
        SelectCharacter(0);
        SelectDifficulty(DifficultyManager.SelectDifficulty.Normal);
    }

    /// <summary>
    /// UI 초기화
    /// </summary>
    private void InitializeUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager Instance가 없습니다!");
            return;
        }

        CharacterData[] characters = GameManager.Instance.CharacterDatas;

        // 캐릭터 버튼들 초기화
        for (int i = 0; i < _characterButtons.Length && i < characters.Length; i++)
        {
            _characterButtons[i].Initialize(i, characters[i]);
        }

        // 사용하지 않는 버튼들 비활성화
        for (int i = characters.Length; i < _characterButtons.Length; i++)
        {
            _characterButtons[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 버튼 이벤트 설정
    /// </summary>
    private void SetupButtons()
    {
        // 캐릭터 선택 버튼들
        for (int i = 0; i < _characterButtons.Length; i++)
        {
            int index = i; // 클로저 문제 해결
            _characterButtons[i].OnButtonClicked += () => SelectCharacter(index);
        }

        // 난이도 선택 버튼들
        for (int i = 0; i < _difficultyButtons.Length; i++)
        {
            int difficultyIndex = i;
            _difficultyButtons[i].onClick.AddListener(() =>
                SelectDifficulty((DifficultyManager.SelectDifficulty)difficultyIndex));
        }

        // 게임 시작 버튼
        _startGameButton.onClick.AddListener(StartGame);

        // 뒤로가기 버튼
        _backButton.onClick.AddListener(GoBack);
    }

    /// <summary>
    /// 캐릭터 선택
    /// </summary>
    public void SelectCharacter(int characterIndex)
    {
        if (GameManager.Instance == null) return;

        CharacterData[] characters = GameManager.Instance.CharacterDatas;
        if (characterIndex < 0 || characterIndex >= characters.Length) return;

        _currentCharacterIndex = characterIndex;
        CharacterData selectedCharacter = characters[characterIndex];

        // UI 업데이트
        UpdateCharacterInfo(selectedCharacter);
        UpdateCharacterButtons(characterIndex);

        // GameManager에 선택 정보 전달
        GameManager.Instance.SelectCharacter(characterIndex);
    }

    /// <summary>
    /// 캐릭터 정보 UI 업데이트
    /// </summary>
    private void UpdateCharacterInfo(CharacterData characterData)
    {
        _characterNameText.text = characterData.CharacterName;
        _characterDescriptionText.text = characterData.CharacterDescription;
        _characterIconImage.sprite = characterData.CharacterIcon;
    }

    /// <summary>
    /// 캐릭터 선택 버튼들 상태 업데이트
    /// </summary>
    private void UpdateCharacterButtons(int selectedIndex)
    {
        for (int i = 0; i < _characterButtons.Length; i++)
        {
            _characterButtons[i].SetSelected(i == selectedIndex);
        }
    }

    /// <summary>
    /// 난이도 선택
    /// </summary>
    public void SelectDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        _currentDifficulty = difficulty;

        // UI 업데이트
        UpdateDifficultyButtons((int)difficulty);
        UpdateDifficultyText(difficulty);

        // GameManager에 선택 정보 전달
        GameManager.Instance.SelectDifficulty(difficulty);
    }

    /// <summary>
    /// 난이도 버튼들 상태 업데이트
    /// </summary>
    private void UpdateDifficultyButtons(int selectedIndex)
    {
        for (int i = 0; i < _difficultyButtons.Length; i++)
        {
            ColorBlock colors = _difficultyButtons[i].colors;
            colors.normalColor = (i == selectedIndex) ? _selectedButtonColor : _normalButtonColor;
            _difficultyButtons[i].colors = colors;
        }
    }

    /// <summary>
    /// 선택된 난이도 텍스트 업데이트
    /// </summary>
    private void UpdateDifficultyText(DifficultyManager.SelectDifficulty difficulty)
    {
        if (DifficultyManager.Instance != null)
        {
            _selectedDifficultyText.text = $"선택된 난이도: {DifficultyManager.Instance.GetDifficultyName(difficulty)}";
        }
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    private void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadPlayScene();
        }
    }

    /// <summary>
    /// 뒤로가기 (타이틀로)
    /// </summary>
    private void GoBack()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadTitleScene();
        }
    }
}
