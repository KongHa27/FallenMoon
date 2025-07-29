using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("----- ĳ���� ������ ��� -----")]
    [SerializeField] CharacterData[] _characterDatas;

    [Header("----- �� �̸� -----")]
    [SerializeField] string _titleSceneName = "0_Title";
    [SerializeField] string _selectSceneName = "1_Select";
    [SerializeField] string _playSceneName = "2_Game";

    // ���õ� ������
    private int _selectedCharacterIndex = 0;
    private DifficultyManager.SelectDifficulty _selectedDifficulty = DifficultyManager.SelectDifficulty.Normal;

    // ���� ����
    public enum GameState
    {
        Title,
        CharacterSelect,
        Playing,
        GameOver
    }

    private GameState _currentState = GameState.Title;

    /// <summary>
    /// ���� �Ŵ��� �̱���
    /// </summary>
    public static GameManager Instance { get; private set; }

    // �̺�Ʈ
    public event Action<int> OnCharacterSelected;
    public event Action<DifficultyManager.SelectDifficulty> OnDifficultySelected;
    public event Action<GameState> OnGameStateChanged;

    // ������Ƽ
    public CharacterData[] CharacterDatas => _characterDatas;
    public int SelectedCharacterIndex => _selectedCharacterIndex;
    public DifficultyManager.SelectDifficulty SelectedDifficulty => _selectedDifficulty;
    public GameState CurrentState => _currentState;

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // �� �ε� �̺�Ʈ ����
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// ���� �ε�� �� ȣ��Ǵ� �Լ�
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == _titleSceneName)
        {
            SetGameState(GameState.Title);
        }
        else if (sceneName == _selectSceneName)
        {
            SetGameState(GameState.CharacterSelect);
        }
        else if (sceneName == _playSceneName)
        {
            SetGameState(GameState.Playing);
            InitializePlayScene();
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (_currentState != newState)
        {
            GameState oldState = _currentState;
            _currentState = newState;
            OnGameStateChanged?.Invoke(_currentState);

            Debug.Log($"���� ���� ����: {oldState} �� {_currentState}");
        }
    }

    #region ĳ���� ����
    /// <summary>
    /// ĳ���� ����
    /// </summary>
    public void SelectCharacter(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < _characterDatas.Length)
        {
            _selectedCharacterIndex = characterIndex;
            OnCharacterSelected?.Invoke(_selectedCharacterIndex);

            Debug.Log($"ĳ���� ����: {_characterDatas[_selectedCharacterIndex].CharacterName}");
        }
        else
        {
            Debug.LogError($"�߸��� ĳ���� �ε���: {characterIndex}");
        }
    }

    /// <summary>
    /// ���õ� ĳ���� ������ ��ȯ
    /// </summary>
    public CharacterData GetSelectedCharacterData()
    {
        if (_selectedCharacterIndex >= 0 && _selectedCharacterIndex < _characterDatas.Length)
        {
            return _characterDatas[_selectedCharacterIndex];
        }
        return null;
    }
    #endregion

    #region ���̵� ����
    /// <summary>
    /// ���̵� ����
    /// </summary>
    public void SelectDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        _selectedDifficulty = difficulty;
        OnDifficultySelected?.Invoke(_selectedDifficulty);

        Debug.Log($"���̵� ����: {difficulty}");
    }
    #endregion

    #region �� ��ȯ
    /// <summary>
    /// Ÿ��Ʋ ������ �̵�
    /// </summary>
    public void LoadTitleScene()
    {
        SceneManager.LoadScene(_titleSceneName);
    }

    /// <summary>
    /// ���� ������ �̵�
    /// </summary>
    public void LoadSelectScene()
    {
        SceneManager.LoadScene(_selectSceneName);
    }

    /// <summary>
    /// �÷��� ������ �̵�
    /// </summary>
    public void LoadPlayScene()
    {
        // ĳ���Ϳ� ���̵��� ���õǾ����� Ȯ��
        if (GetSelectedCharacterData() == null)
        {
            Debug.LogError("ĳ���Ͱ� ���õ��� �ʾҽ��ϴ�!");
            return;
        }

        SceneManager.LoadScene(_playSceneName);
    }
    #endregion

    #region �÷��� �� �ʱ�ȭ
    /// <summary>
    /// �÷��� �� �ʱ�ȭ (ĳ���� ������ ����, ���̵� ����)
    /// </summary>
    private void InitializePlayScene()
    {
        // ���̵� ����
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetSelectedDifficulty(_selectedDifficulty);
        }

        // ĳ���� ������ ����
        StartCoroutine(SpawnSelectedCharacter());
    }

    /// <summary>
    /// ���õ� ĳ���� �������� Hero ���ӿ�����Ʈ�� �ڽ����� ����
    /// </summary>
    private System.Collections.IEnumerator SpawnSelectedCharacter()
    {
        // Hero ���ӿ�����Ʈ�� ã�� ������ ���
        Hero heroController = null;
        while (heroController == null)
        {
            heroController = FindObjectOfType<Hero>();
            yield return null;
        }

        CharacterData selectedData = GetSelectedCharacterData();
        if (selectedData != null && selectedData.CharacterPrefab != null)
        {
            // ���� �ڽ� ������Ʈ�� �ִٸ� ����
            for (int i = heroController.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = heroController.transform.GetChild(i);
                // ĳ���� ���� ������Ʈ�� ���� �ڽĸ� ����
                if (child.GetComponent<HeroModel>() != null ||
                    child.GetComponent<SpriteRenderer>() != null ||
                    child.GetComponent<Animator>() != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            // �� ĳ���� ������ ����
            GameObject characterInstance = Instantiate(selectedData.CharacterPrefab, heroController.transform);

            // ���� ��ġ�� ȸ�� �ʱ�ȭ
            characterInstance.transform.localPosition = Vector3.zero;
            characterInstance.transform.localRotation = Quaternion.identity;
            characterInstance.transform.localScale = Vector3.one;

            // Hero ��Ʈ�ѷ� �ʱ�ȭ
            heroController.InitializeWithPrefab();

            Debug.Log($"ĳ���� ������ ���� �Ϸ�: {selectedData.CharacterName}");
        }
        else
        {
            Debug.LogError("���õ� ĳ���� ������ �Ǵ� �������� �����ϴ�!");
        }
    }
    #endregion

    #region ���� ����/�����
    /// <summary>
    /// ���� ����� (���� ������)
    /// </summary>
    public void RestartGame()
    {
        LoadSelectScene();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}
