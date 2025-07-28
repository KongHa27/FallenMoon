using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("----- ĳ���� ���� UI -----")]
    [SerializeField] TextMeshProUGUI _characterNameText;
    [SerializeField] TextMeshProUGUI _characterDescriptionText;
    [SerializeField] Image _characterIconImage;
    [SerializeField] Image[] _starImages; // ���� UI (5��)

    [Header("----- ĳ���� ���� ��ư�� -----")]
    [SerializeField] CharacterSelectButton[] _characterButtons;

    [Header("----- ���̵� ���� UI -----")]
    [SerializeField] Button[] _difficultyButtons; // 4�� ���̵� ��ư
    [SerializeField] TextMeshProUGUI _selectedDifficultyText;
    [SerializeField] Color _selectedButtonColor = Color.yellow;
    [SerializeField] Color _normalButtonColor = Color.white;

    [Header("----- ���� ���� ��ư -----")]
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _backButton;

    // ���� ���� ����
    private int _currentCharacterIndex = 0;
    private DifficultyManager.SelectDifficulty _currentDifficulty = DifficultyManager.SelectDifficulty.Normal;

    private void Start()
    {
        InitializeUI();
        SetupButtons();

        // �ʱ� ĳ���� ����
        SelectCharacter(0);
        SelectDifficulty(DifficultyManager.SelectDifficulty.Normal);
    }

    /// <summary>
    /// UI �ʱ�ȭ
    /// </summary>
    private void InitializeUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager Instance�� �����ϴ�!");
            return;
        }

        CharacterData[] characters = GameManager.Instance.CharacterDatas;

        // ĳ���� ��ư�� �ʱ�ȭ
        for (int i = 0; i < _characterButtons.Length && i < characters.Length; i++)
        {
            _characterButtons[i].Initialize(i, characters[i]);
        }

        // ������� �ʴ� ��ư�� ��Ȱ��ȭ
        for (int i = characters.Length; i < _characterButtons.Length; i++)
        {
            _characterButtons[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ��ư �̺�Ʈ ����
    /// </summary>
    private void SetupButtons()
    {
        // ĳ���� ���� ��ư��
        for (int i = 0; i < _characterButtons.Length; i++)
        {
            int index = i; // Ŭ���� ���� �ذ�
            _characterButtons[i].OnButtonClicked += () => SelectCharacter(index);
        }

        // ���̵� ���� ��ư��
        for (int i = 0; i < _difficultyButtons.Length; i++)
        {
            int difficultyIndex = i;
            _difficultyButtons[i].onClick.AddListener(() =>
                SelectDifficulty((DifficultyManager.SelectDifficulty)difficultyIndex));
        }

        // ���� ���� ��ư
        _startGameButton.onClick.AddListener(StartGame);

        // �ڷΰ��� ��ư
        _backButton.onClick.AddListener(GoBack);
    }

    /// <summary>
    /// ĳ���� ����
    /// </summary>
    public void SelectCharacter(int characterIndex)
    {
        if (GameManager.Instance == null) return;

        CharacterData[] characters = GameManager.Instance.CharacterDatas;
        if (characterIndex < 0 || characterIndex >= characters.Length) return;

        _currentCharacterIndex = characterIndex;
        CharacterData selectedCharacter = characters[characterIndex];

        // UI ������Ʈ
        UpdateCharacterInfo(selectedCharacter);
        UpdateCharacterButtons(characterIndex);

        // GameManager�� ���� ���� ����
        GameManager.Instance.SelectCharacter(characterIndex);
    }

    /// <summary>
    /// ĳ���� ���� UI ������Ʈ
    /// </summary>
    private void UpdateCharacterInfo(CharacterData characterData)
    {
        _characterNameText.text = characterData.CharacterName;
        _characterDescriptionText.text = characterData.CharacterDescription;
        _characterIconImage.sprite = characterData.CharacterIcon;
    }

    /// <summary>
    /// ĳ���� ���� ��ư�� ���� ������Ʈ
    /// </summary>
    private void UpdateCharacterButtons(int selectedIndex)
    {
        for (int i = 0; i < _characterButtons.Length; i++)
        {
            _characterButtons[i].SetSelected(i == selectedIndex);
        }
    }

    /// <summary>
    /// ���̵� ����
    /// </summary>
    public void SelectDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        _currentDifficulty = difficulty;

        // UI ������Ʈ
        UpdateDifficultyButtons((int)difficulty);
        UpdateDifficultyText(difficulty);

        // GameManager�� ���� ���� ����
        GameManager.Instance.SelectDifficulty(difficulty);
    }

    /// <summary>
    /// ���̵� ��ư�� ���� ������Ʈ
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
    /// ���õ� ���̵� �ؽ�Ʈ ������Ʈ
    /// </summary>
    private void UpdateDifficultyText(DifficultyManager.SelectDifficulty difficulty)
    {
        if (DifficultyManager.Instance != null)
        {
            _selectedDifficultyText.text = $"���õ� ���̵�: {DifficultyManager.Instance.GetDifficultyName(difficulty)}";
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadPlayScene();
        }
    }

    /// <summary>
    /// �ڷΰ��� (Ÿ��Ʋ��)
    /// </summary>
    private void GoBack()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadTitleScene();
        }
    }
}
