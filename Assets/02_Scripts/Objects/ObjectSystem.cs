using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 시스템을 관리하는 매니저 클래스
/// </summary>
public class ObjectSystem : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] StageManager _stageManager;

    [Header("----- 오브젝트 프리팹 -----")]
    [SerializeField] GameObject _magicCirclePrefab; // 마법진 프리팹
    [SerializeField] GameObject _urnPrefab;      // 항아리 프리팹
    [SerializeField] GameObject _chestPrefab;    // 상자 프리팹
    [SerializeField] GameObject _altarPrefab;    // 제단 프리팹

    [Header("----- 스폰 설정 -----")]
    [SerializeField] int _minUrns = 4;           // 최소 항아리 개수
    [SerializeField] int _maxUrns = 15;           // 최대 항아리 개수
    [SerializeField] int _minChests = 3;         // 최소 상자 개수
    [SerializeField] int _maxChests = 8;         // 최대 상자 개수
    [SerializeField] int _minAltars = 1;         // 최소 제단 개수
    [SerializeField] int _maxAltars = 3;         // 최대 제단 개수
    [SerializeField] float _minDistance = 3f;    // 오브젝트 간 최소 거리

    List<GameObject> _spawnedObjects = new List<GameObject>();
    List<Vector3> _usedPositions = new List<Vector3>();
    GameObject _magicCircleInstance;

    public static ObjectSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Initialize(_stageManager);
    }

    /// <summary>
    /// 오브젝트 시스템 초기화
    /// </summary>
    /// <param name="stageManager"></param>
    public void Initialize(StageManager stageManager)
    {
        _stageManager = stageManager;

        // 스테이지 시작 이벤트 구독
        if (_stageManager != null)
        {
            _stageManager.OnStageStart += OnStageStart;
        }
    }

    /// <summary>
    /// 스테이지 시작 시 호출되는 함수
    /// </summary>
    /// <param name="stageIndex"></param>
    void OnStageStart(int stageIndex)
    {
        StartCoroutine(SpawnObjectsRoutine());
    }

    /// <summary>
    /// 오브젝트들을 스폰하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnObjectsRoutine()
    {
        // 맵 생성 완료 대기
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        // 기존 오브젝트들 및 사용 좌표 초기화(제거)
        ClearObjects();

        // 마법진 가장 먼저 생성
        SpawnMagicCircle();

        // 각 오브젝트 타입별로 스폰
        SpawnObjects(ObjectType.Urn, Random.Range(_minUrns, _maxUrns + 1));
        SpawnObjects(ObjectType.Chest, Random.Range(_minChests, _maxChests + 1));
        SpawnObjects(ObjectType.Altar, Random.Range(_minAltars, _maxAltars + 1));

        Debug.Log($"오브젝트 스폰 완료: 항아리 {_spawnedObjects.FindAll(o => o.GetComponent<InteractableObjects>()?.ObjectType == ObjectType.Urn).Count}개, " +
                  $"상자 {_spawnedObjects.FindAll(o => o.GetComponent<InteractableObjects>()?.ObjectType == ObjectType.Chest).Count}개, " +
                  $"제단 {_spawnedObjects.FindAll(o => o.GetComponent<InteractableObjects>()?.ObjectType == ObjectType.Altar).Count}개");
    }

    /// <summary>
    /// 마법진을 생성하는 함수
    /// </summary>
    void SpawnMagicCircle()
    {
        if (_magicCirclePrefab == null)
        {
            Debug.LogError("마법진 프리팹이 설정되지 않았습니다.");
            return;
        }

        Vector3 spawnPos = GetValidSpawnPosition();
        if (spawnPos != Vector3.zero)
        {
            spawnPos.y += 1f;

            _magicCircleInstance = Instantiate(_magicCirclePrefab, spawnPos, Quaternion.identity);
            _spawnedObjects.Add(_magicCircleInstance);
            _usedPositions.Add(spawnPos);

            Debug.Log($"마법진 스폰: {spawnPos}");

            // 마법진 시스템에게 초기화 요청
            MagicCircleSystem magicCircleSystem = FindObjectOfType<MagicCircleSystem>();
            if (magicCircleSystem != null)
            {
                magicCircleSystem.OnMagicCircleCreated(_magicCircleInstance);
            }
        }
        else
        {
            Debug.LogWarning("마법진 스폰 실패: 적절한 위치를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 특정 타입의 오브젝트들을 스폰하는 함수
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="count"></param>
    void SpawnObjects(ObjectType objectType, int count)
    {
        GameObject prefab = GetPrefabByType(objectType);
        if (prefab == null)
        {
            Debug.LogError($"오브젝트 타입 {objectType}에 대한 프리팹이 없습니다.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            if (spawnPos != Vector3.zero)
            {
                spawnPos.y += 1f;
                GameObject spawnedObject = Instantiate(prefab, spawnPos, Quaternion.identity);
                _spawnedObjects.Add(spawnedObject);
                _usedPositions.Add(spawnPos);

                Debug.Log($"{objectType} 스폰: {spawnPos}");
            }
            else
            {
                Debug.LogWarning($"{objectType} 스폰 실패: 적절한 위치를 찾을 수 없습니다.");
                break;
            }
        }
    }

    /// <summary>
    /// 오브젝트 타입에 맞는 프리팹을 반환하는 함수
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    GameObject GetPrefabByType(ObjectType objectType)
    {
        return objectType switch
        {
            ObjectType.Urn => _urnPrefab,
            ObjectType.Chest => _chestPrefab,
            ObjectType.Altar => _altarPrefab,
            _ => null
        };
    }

    /// <summary>
    /// 유효한 스폰 위치를 찾는 함수
    /// </summary>
    /// <returns></returns>
    Vector3 GetValidSpawnPosition()
    {
        const int maxAttempts = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 candidatePos = StageManager.Instance.GetRanPosOnGround();

            if (candidatePos == Vector3.zero) continue;

            bool isValidPosition = true;

            // 기존 위치들과의 거리 체크
            foreach (Vector3 usedPos in _usedPositions)
            {
                if (Vector3.Distance(candidatePos, usedPos) < _minDistance)
                {
                    isValidPosition = false;
                    break;
                }
            }

            if (isValidPosition)
            {
                return candidatePos;
            }
        }

        return Vector3.zero;
    }

    /// <summary>
    /// 기존 오브젝트들을 제거하는 함수
    /// </summary>
    void ClearObjects()
    {
        foreach (GameObject obj in _spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        _spawnedObjects.Clear();
        _usedPositions.Clear();
        _magicCircleInstance = null;
    }

    /// <summary>
    /// 오브젝트가 파괴되었을 때 리스트에서 제거
    /// </summary>
    /// <param name="destroyedObject"></param>
    public void OnObjectDestroyed(GameObject destroyedObject)
    {
        _spawnedObjects.Remove(destroyedObject);

        if (destroyedObject == _magicCircleInstance)
            _magicCircleInstance = null;

        // 해당 오브젝트의 위치도 사용된 위치 목록에서 제거
        InteractableObjects interactable = destroyedObject.GetComponent<InteractableObjects>();
        if (interactable != null)
        {
            Vector3 objPos = destroyedObject.transform.position;
            _usedPositions.RemoveAll(pos => Vector3.Distance(pos, objPos) < 0.1f);
        }
    }

    public GameObject GetMagicCircleInstance()
    {
        return _magicCircleInstance;
    }

    private void OnDestroy()
    {
        if (_stageManager != null)
        {
            _stageManager.OnStageStart -= OnStageStart;
        }
    }
}
