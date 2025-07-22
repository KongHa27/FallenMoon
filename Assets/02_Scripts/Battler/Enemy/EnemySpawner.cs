using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("----- 스폰 설정 -----")]
    [SerializeField] GameObject[] _enemyPrefabs;                //적 프리팹
    [SerializeField] float _baseSpawnInterval;                  //스폰 간격
    [SerializeField] int _maxEnemyCount;                        //최대 적 수

    [Header("----- 맵 설정 -----")]
    [SerializeField] Vector2 _mapSize = new Vector2(80f, 40f);  //맵 크기
    [SerializeField] Vector2 _mapCenter = Vector2.zero;         //맵 중앙
    [SerializeField] float _spawnRadius;                        //스폰 체크 반경
    [SerializeField] int _maxSpawnAttempts;                     //최대 스폰 시도 횟수

    [Header("----- 타일맵 설정 -----")]
    [SerializeField] Tilemap _ground;                           //Ground 타일맵 참조

    [Header("----- 레벨별 스폰 조정 -----")]
    [SerializeField] float _spawnIntervalDecreaseRate;     // 스폰 간격 감소율(레벨당)
    [SerializeField] float _minSpawnInterval;              // 최소 스폰 간격
    [SerializeField] int _maxEnemyIncreaseRate;            // 레벨당 최대 적 수 증가율

    [Header("----- 플레이어 참조(임시) -----")]
    [SerializeField] Hero _hero;

    [Header("----- 디버그 용 -----")]
    [SerializeField] bool _debugMode = false;
    [SerializeField] KeyCode _spawnEliteKey = KeyCode.F3;

    List<Enemy> _activeEnemies = new List<Enemy>();
    
    int _curErosionLevel;       //현재 침식도 레벨
    float _curSpawnInterval;    //현재 스폰 간격
    int _curMaxEnemyCount;      //현재 최대 적 수

    Coroutine _spawnRoutine;    //스폰 코루틴

    private void Start()
    {
        //DifficultyManager 초기화 대기
        StartCoroutine(WaitForDifficultyManager());
    }

    /// <summary>
    /// DifficultyManager 초기화가 되어야 스포너 시작(Initialize)하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForDifficultyManager()
    {
        // DifficultyManager가 준비될 때까지 대기
        while (DifficultyManager.Instance == null)
        {
            yield return null;
        }

        // 난이도 매니저가 준비되면 이벤트 구독 및 초기화
        DifficultyManager.Instance.OnErosionLevelChanged += OnDifficultyLevelChanged;
        _curErosionLevel = DifficultyManager.Instance.CurrentErosionLevel;

        // 초기 스폰 설정 적용
        UpdateSpawnSettings();

        // 스폰 시작
        _spawnRoutine = StartCoroutine(SpawnEnemies());
    }

    private void OnDestroy()
    {
        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.OnErosionLevelChanged -= OnDifficultyLevelChanged;
    }

    private void Update()
    {
        // 죽은 적들을 리스트에서 제거
        _activeEnemies.RemoveAll(enemy => enemy == null);

        //디버그 (강제 엘리트 소환)
        if (Input.GetKeyDown(_spawnEliteKey))
        {
            if (_hero != null)
            {
                Vector3 heroPos = _hero.transform.position;
                Vector3 spawnPos = heroPos + new Vector3(Random.Range(-5f, 5f), 0.5f, 0f);
                ForceSpawnEliteEnemy(spawnPos);
            }
        }
    }

    /// <summary>
    /// 적 스폰 코루틴
    /// </summary>
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (_activeEnemies.Count < _curMaxEnemyCount)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(_curSpawnInterval);
        }
    }

    /// <summary>
    /// 적 스폰 실행
    /// </summary>
    void SpawnEnemy()
    {
        if (_enemyPrefabs.Length == 0) return;

        // 랜덤 적 선택
        GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];

        // 랜덤 스폰 위치 선택      
        Vector3 spawnPos = GetRandomSpawnPos();

        // 적 생성
        if (spawnPos == Vector3.zero)
        {
            Debug.LogError("적절한 스폰 위치를 찾을 수 없습니다.");
            SpawnEnemy();
            return;
        }

        spawnPos.y += 1f;

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            //레벨에 따라 적 초기화
            enemy.Initialize(_curErosionLevel);

            //적 사망 이벤트 구독
            enemy.OnEnemyDeath += OnEnemyDeath;

            //활성화된 적 리스트에 추가
            _activeEnemies.Add(enemy);
        }
    }

    /// <summary>
    /// 맵 내 랜덤 스폰 위치 생성
    /// </summary>
    /// <returns></returns>
    Vector3 GetRandomSpawnPos()
    {
        for (int i = 0; i < _maxSpawnAttempts; i++)
        {
            // 맵 범위 내에서 랜덤 위치 생성
            float randomX = Random.Range(_mapCenter.x - _mapSize.x / 2f, _mapCenter.x + _mapSize.x / 2f);
            float randomY = Random.Range(_mapCenter.y - _mapSize.y / 2f, _mapCenter.y + _mapSize.y / 2f);

            Vector3 candidatePosition = new Vector3(randomX, randomY, 0f);

            // 해당 위치가 스폰 가능한지 확인
            if (IsValidSpawnPosition(candidatePosition))
            {
                return candidatePosition;
            }
        }

        // 유효한 위치를 찾지 못한 경우 Vector3.zero 반환
        return Vector3.zero;
    }

    /// <summary>
    /// 스폰 위치가 유효한지 확인
    /// </summary>
    bool IsValidSpawnPosition(Vector3 position)
    {
        // Ground 타일맵에 타일이 있는지 확인
        if (_ground != null)
        {
            Vector3Int cellPosition = _ground.WorldToCell(position);
            if (!_ground.HasTile(cellPosition))
            {
                return false; // Ground 타일이 없으면 스폰 불가
            }
        }

        return true;
    }

    /// <summary>
    /// 침식도 레벨에 따른 스폰 간격 계산
    /// </summary>
    float GetSpawnInterval()
    {
        float interval = _baseSpawnInterval * (1f - (_spawnIntervalDecreaseRate / 100f) * (_curErosionLevel - 1));
        return Mathf.Max(interval, _minSpawnInterval);
    }

    /// <summary>
    /// 침식도 레벨에 따른 최대 적 수 계산
    /// </summary>
    int GetMaxEnemyCount()
    {
        return _maxEnemyCount + (_maxEnemyIncreaseRate * (_curErosionLevel - 1));
    }

    /// <summary>
    /// 스폰 설정 업데이트
    /// </summary>
    void UpdateSpawnSettings()
    {
        _curSpawnInterval = GetSpawnInterval();
        _curMaxEnemyCount = GetMaxEnemyCount();

        //디버그
        if (_debugMode)
        {
            Debug.Log($"스폰 설정 업데이트 - 침식도 레벨: {_curErosionLevel}, " +
                      $"스폰 간격: {_curSpawnInterval:F2}초, 최대 적 수: {_curMaxEnemyCount}");
        }
    }

    /// <summary>
    /// 침식도 레벨 변화 시 호출
    /// </summary>
    void OnDifficultyLevelChanged(int oldLevel, int newLevel)
    {
        _curErosionLevel = newLevel;
        UpdateSpawnSettings();
    }

    /// <summary>
    /// 적 사망 시 호출
    /// 리스트에서 사망한 적 제거
    /// </summary>
    void OnEnemyDeath(Enemy enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
        }
    }

    /// <summary>
    /// 특정 위치에 엘리트 적 확정 스폰 (디버깅용)
    /// </summary>
    public void ForceSpawnEliteEnemy(Vector3 position)
    {
        if (_enemyPrefabs.Length == 0) return;

        GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            // 강제로 엘리트로 스폰
            enemy.Initialize(_curErosionLevel, true);
            enemy.OnEnemyDeath += OnEnemyDeath;
            _activeEnemies.Add(enemy);

            Debug.Log($"엘리트 적 강제 스폰 완료 (침식도 레벨: {_curErosionLevel})");
        }
    }

    /// <summary>
    /// 모든 적 제거
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (Enemy enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        _activeEnemies.Clear();
    }
}
