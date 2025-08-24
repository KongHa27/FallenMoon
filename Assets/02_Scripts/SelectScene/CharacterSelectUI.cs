using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("----- 캐릭터 정보 UI -----")]
    [SerializeField] TextMeshProUGUI _characterNameText;

    [Header("----- 2D 캐릭터 프리뷰 -----")]
    [SerializeField] Transform _previewSpawnPoint; // 프리뷰 캐릭터가 생성될 위치
    [SerializeField] SpriteRenderer _previewSpriteRenderer; // 프리뷰용 스프라이트 렌더러

    [Header("----- 캐릭터 선택 버튼들 -----")]
    [SerializeField] CharacterSelectButton[] _characterButtons;

    [Header("----- 난이도 선택 UI -----")]
    [SerializeField] Button[] _difficultyButtons;
    [SerializeField] GameObject[] _difficultyHighlights;
    [SerializeField] TextMeshProUGUI _selectedDifficultyText;

    [Header("----- 게임 시작 버튼 -----")]
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _backButton;

    // 현재 선택 상태
    private int _currentCharacterIndex = 0;
    private DifficultyManager.SelectDifficulty _currentDifficulty = DifficultyManager.SelectDifficulty.Normal;

    // 프리뷰 애니메이션 관련
    private Coroutine _previewAnimationCoroutine;

    private void Awake()
    {
        InitializeUI();
        SetupButtons();

        // 초기 설정 (캐릭터는 선택하지 않고 프리뷰만 숨김)
        if (_previewSpriteRenderer != null)
        {
            _previewSpriteRenderer.gameObject.SetActive(false);
        }

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
        for (int i = 0; i < _characterButtons.Length; i++)
        {
            if (i < characters.Length)
            {
                // 캐릭터 데이터가 있는 버튼들은 초기화
                _characterButtons[i].Initialize(i, characters[i]);
                _characterButtons[i].gameObject.SetActive(true);
            }
            else
            {
                // 캐릭터 데이터가 없는 버튼들은 비활성화
                _characterButtons[i].gameObject.SetActive(false);
            }
        }

        Debug.Log($"캐릭터 데이터 개수: {characters.Length}");
        for (int i = 0; i < characters.Length; i++)
        {
            Debug.Log($"캐릭터 {i}: {characters[i].CharacterName}");
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
            int index = i;
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

        Debug.Log($"캐릭터 선택: {selectedCharacter.CharacterName} (인덱스: {characterIndex})");

        // UI 업데이트
        UpdateCharacterInfo(selectedCharacter);
        UpdateCharacterButtons(characterIndex);

        // 2D 캐릭터 프리뷰 업데이트
        UpdateCharacterPreview(selectedCharacter);
    }

    /// <summary>
    /// 캐릭터 정보 UI 업데이트
    /// </summary>
    private void UpdateCharacterInfo(CharacterData characterData)
    {
        _characterNameText.text = characterData.CharacterName;
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
    /// 2D 캐릭터 프리뷰 업데이트
    /// </summary>
    private void UpdateCharacterPreview(CharacterData characterData)
    {
        // 기존 애니메이션 중지
        if (_previewAnimationCoroutine != null)
        {
            StopCoroutine(_previewAnimationCoroutine);
        }

        // 프리뷰 스프라이트 렌더러 활성화
        if (_previewSpriteRenderer != null)
        {
            _previewSpriteRenderer.gameObject.SetActive(true);

            // 아이들 애니메이션 시작
            if (characterData.PreviewIdleSprites != null && characterData.PreviewIdleSprites.Length > 0)
            {
                _previewAnimationCoroutine = StartCoroutine(PlayPreviewIdleAnimation(characterData));
            }
            else
            {
                // 스프라이트 배열이 없으면 프리뷰 스프라이트 사용
                _previewSpriteRenderer.sprite = characterData.CharacterPreviewSprite;
            }

            Debug.Log($"캐릭터 프리뷰 업데이트: {characterData.CharacterName}");
        }
    }

    /// <summary>
    /// 프리뷰 아이들 애니메이션 재생
    /// </summary>
    private IEnumerator PlayPreviewIdleAnimation(CharacterData characterData)
    {
        Sprite[] sprites = characterData.PreviewIdleSprites;
        float frameTime = 1.5f / characterData.AnimationFrameRate;
        int currentFrame = 0;

        while (true)
        {
            _previewSpriteRenderer.sprite = sprites[currentFrame];
            currentFrame = (currentFrame + 1) % sprites.Length;
            yield return new WaitForSeconds(frameTime);
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
    }

    /// <summary>
    /// 난이도 버튼들 상태 업데이트
    /// </summary>
    private void UpdateDifficultyButtons(int selectedIndex)
    {
        for (int i = 0; i < _difficultyHighlights.Length; i++)
        {
            _difficultyHighlights[i].SetActive(i == selectedIndex);
        }
    }

    /// <summary>
    /// 선택된 난이도 텍스트 업데이트
    /// </summary>
    private void UpdateDifficultyText(DifficultyManager.SelectDifficulty difficulty)
    {
        if (DifficultyManager.Instance != null)
        {
            string diffName = DifficultyManager.Instance.GetDifficultyName(difficulty);

            _selectedDifficultyText.text = diffName;
        }
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    private void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectCharacter(_currentCharacterIndex);

            GameManager.Instance.SelectDifficulty(_currentDifficulty);

            GameManager.Instance.LoadPlayScene();
        }
    }

    /// <summary>
    /// 뒤로가기
    /// </summary>
    private void GoBack()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadTitleScene();
        }
    }

    /// <summary>
    /// 씬 비활성화 시 애니메이션 정리
    /// </summary>
    private void OnDisable()
    {
        if (_previewAnimationCoroutine != null)
        {
            StopCoroutine(_previewAnimationCoroutine);
            _previewAnimationCoroutine = null;
        }
    }
}
