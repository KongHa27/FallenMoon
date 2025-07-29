using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("----- ĳ���� ���� UI -----")]
    [SerializeField] TextMeshProUGUI _characterNameText;

    [Header("----- 2D ĳ���� ������ -----")]
    [SerializeField] Image _characterPreviewImage; // 2D ĳ���� �̹���
    [SerializeField] Animator _characterPreviewAnimator; // 2D �ִϸ��̼ǿ� Animator
    [SerializeField] Vector3 _characterScale = Vector3.one; // ĳ���� ũ�� ����

    [Header("----- ĳ���� ���� ��ư�� -----")]
    [SerializeField] CharacterSelectButton[] _characterButtons;

    [Header("----- ���̵� ���� UI -----")]
    [SerializeField] Button[] _difficultyButtons;
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
            int index = i;
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

        // 2D ĳ���� ������ ������Ʈ
        UpdateCharacterPreview(selectedCharacter);

        // GameManager�� ���� ���� ����
        GameManager.Instance.SelectCharacter(characterIndex);
    }

    /// <summary>
    /// ĳ���� ���� UI ������Ʈ
    /// </summary>
    private void UpdateCharacterInfo(CharacterData characterData)
    {
        _characterNameText.text = characterData.CharacterName;
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
    /// 2D ĳ���� ������ ������Ʈ
    /// </summary>
    private void UpdateCharacterPreview(CharacterData characterData)
    {
        // ĳ���� ������ �̹��� ����
        if (_characterPreviewImage != null)
        {
            // �̹��� Ȱ��ȭ
            _characterPreviewImage.gameObject.SetActive(true);

            // ������ ����
            _characterPreviewImage.transform.localScale = _characterScale;

            // �ִϸ����� ��Ʈ�ѷ� ���� (Entry -> Idle�� �ڵ� ���)
            if (_characterPreviewAnimator != null && characterData.CharacterAnimatorController != null)
            {
                _characterPreviewAnimator.runtimeAnimatorController = characterData.CharacterAnimatorController;
                // Animator Controller���� Entry -> Idle�� �ڵ� ����Ǿ� ������ �ڵ����� �����
            }

            Debug.Log($"ĳ���� ������ ������Ʈ: {characterData.CharacterName}");
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
    /// �ڷΰ���
    /// </summary>
    private void GoBack()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadTitleScene();
        }
    }
}
