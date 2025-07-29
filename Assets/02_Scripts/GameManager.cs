using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("----- 캐릭터 프리팹 목록 -----")]
    [SerializeField] CharacterData[] _characterDatas;

    [Header("----- 씬 이름 -----")]
    [SerializeField] string _titleSceneName = "0_Title";
    [SerializeField] string _selectSceneName = "1_Select";
    [SerializeField] string _playSceneName = "2_Game";

    // 선택된 데이터
    private int _selectedCharacterIndex = 0;
    private DifficultyManager.SelectDifficulty _selectedDifficulty = DifficultyManager.SelectDifficulty.Normal;

    // 게임 상태
    public enum GameState
    {
        Title,
        CharacterSelect,
        Playing,
        GameOver
    }

    private GameState _currentState = GameState.Title;

    /// <summary>
    /// 게임 매니저 싱글톤
    /// </summary>
    public static GameManager Instance { get; private set; }

    // 이벤트
    public event Action<int> OnCharacterSelected;
    public event Action<DifficultyManager.SelectDifficulty> OnDifficultySelected;
    public event Action<GameState> OnGameStateChanged;

    // 프로퍼티
    public CharacterData[] CharacterDatas => _characterDatas;
    public int SelectedCharacterIndex => _selectedCharacterIndex;
    public DifficultyManager.SelectDifficulty SelectedDifficulty => _selectedDifficulty;
    public GameState CurrentState => _currentState;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬 로드 이벤트 구독
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
    /// 씬이 로드될 때 호출되는 함수
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
    /// 게임 상태 변경
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (_currentState != newState)
        {
            GameState oldState = _currentState;
            _currentState = newState;
            OnGameStateChanged?.Invoke(_currentState);

            Debug.Log($"게임 상태 변경: {oldState} → {_currentState}");
        }
    }

    #region 캐릭터 선택
    /// <summary>
    /// 캐릭터 선택
    /// </summary>
    public void SelectCharacter(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < _characterDatas.Length)
        {
            _selectedCharacterIndex = characterIndex;
            OnCharacterSelected?.Invoke(_selectedCharacterIndex);

            Debug.Log($"캐릭터 선택: {_characterDatas[_selectedCharacterIndex].CharacterName}");
        }
        else
        {
            Debug.LogError($"잘못된 캐릭터 인덱스: {characterIndex}");
        }
    }

    /// <summary>
    /// 선택된 캐릭터 데이터 반환
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

    #region 난이도 선택
    /// <summary>
    /// 난이도 선택
    /// </summary>
    public void SelectDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        _selectedDifficulty = difficulty;
        OnDifficultySelected?.Invoke(_selectedDifficulty);

        Debug.Log($"난이도 선택: {difficulty}");
    }
    #endregion

    #region 씬 전환
    /// <summary>
    /// 타이틀 씬으로 이동
    /// </summary>
    public void LoadTitleScene()
    {
        SceneManager.LoadScene(_titleSceneName);
    }

    /// <summary>
    /// 선택 씬으로 이동
    /// </summary>
    public void LoadSelectScene()
    {
        SceneManager.LoadScene(_selectSceneName);
    }

    /// <summary>
    /// 플레이 씬으로 이동
    /// </summary>
    public void LoadPlayScene()
    {
        // 캐릭터와 난이도가 선택되었는지 확인
        if (GetSelectedCharacterData() == null)
        {
            Debug.LogError("캐릭터가 선택되지 않았습니다!");
            return;
        }

        SceneManager.LoadScene(_playSceneName);
    }
    #endregion

    #region 플레이 씬 초기화
    /// <summary>
    /// 플레이 씬 초기화 (캐릭터 프리팹 생성, 난이도 설정)
    /// </summary>
    private void InitializePlayScene()
    {
        // 난이도 설정
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetSelectedDifficulty(_selectedDifficulty);
        }

        // 캐릭터 프리팹 생성
        StartCoroutine(SpawnSelectedCharacter());
    }

    /// <summary>
    /// 선택된 캐릭터 프리팹을 Hero 게임오브젝트의 자식으로 생성
    /// </summary>
    private System.Collections.IEnumerator SpawnSelectedCharacter()
    {
        // Hero 게임오브젝트를 찾을 때까지 대기
        Hero heroController = null;
        while (heroController == null)
        {
            heroController = FindObjectOfType<Hero>();
            yield return null;
        }

        CharacterData selectedData = GetSelectedCharacterData();
        if (selectedData != null && selectedData.CharacterPrefab != null)
        {
            // 기존 자식 오브젝트가 있다면 제거
            for (int i = heroController.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = heroController.transform.GetChild(i);
                // 캐릭터 관련 컴포넌트를 가진 자식만 제거
                if (child.GetComponent<HeroModel>() != null ||
                    child.GetComponent<SpriteRenderer>() != null ||
                    child.GetComponent<Animator>() != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            // 새 캐릭터 프리팹 생성
            GameObject characterInstance = Instantiate(selectedData.CharacterPrefab, heroController.transform);

            // 로컬 위치와 회전 초기화
            characterInstance.transform.localPosition = Vector3.zero;
            characterInstance.transform.localRotation = Quaternion.identity;
            characterInstance.transform.localScale = Vector3.one;

            // Hero 컨트롤러 초기화
            heroController.InitializeWithPrefab();

            Debug.Log($"캐릭터 프리팹 생성 완료: {selectedData.CharacterName}");
        }
        else
        {
            Debug.LogError("선택된 캐릭터 데이터 또는 프리팹이 없습니다!");
        }
    }
    #endregion

    #region 게임 종료/재시작
    /// <summary>
    /// 게임 재시작 (선택 씬으로)
    /// </summary>
    public void RestartGame()
    {
        LoadSelectScene();
    }

    /// <summary>
    /// 게임 종료
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
