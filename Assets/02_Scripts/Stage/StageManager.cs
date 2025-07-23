using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ��ü ���������� �����ϴ� Ŭ����
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("----- �������� ���� -----")]
    [SerializeField] StageData[] _stages = new StageData[5];    //�������� �迭
    [SerializeField] Transform _player;         //�÷��̾�
    [SerializeField] LayerMask _ground;         //ground Ÿ�� ���̾�

    [Header("----- UI -----")]
    [SerializeField] GameObject _stageUI;

    MagicCircleSystem _magicCircleSystem;       //������ �ý��� ����

    int _curStageIndex = 0;         //���� �������� �ε���
    GameObject _curMapInstance;     //���� �������� ��

    //�������� �Ŵ��� �ν��Ͻ�
    public static StageManager Instance { get; private set; }
    public int CurStageIndex => _curStageIndex;
    public StageData CurStage => _stages[_curStageIndex];

    //�̺�Ʈ
    public event Action<int> OnStageStart;
    public event Action<int> OnStageComplete;
    public event Action OnAllStageComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartStage(0);
    }

    /// <summary>
    /// ���������� �����ϴ� �Լ�
    /// ���� �� ����, �÷��̾� �� �������� ���� ��ġ�� ����
    /// </summary>
    /// <param name="stageIndex"></param>
    public void StartStage(int stageIndex)
    {
        //��� �������� Ŭ���� ��
        if (stageIndex >= _stages.Length)
        {
            OnAllStageComplete?.Invoke();
            Debug.Log("��� �������� Ŭ����!");
            return;
        }

        _curStageIndex = stageIndex;
        StageData curStage = _stages[_curStageIndex];

        //���� �� ����
        if (_curMapInstance != null)
            Destroy(_curMapInstance);

        //���� �� ����
        int ranMapIndex = UnityEngine.Random.Range(0, curStage.MapPrefabs.Length);
        GameObject selectedMap = curStage.MapPrefabs[ranMapIndex];

        //�� ����
        _curMapInstance = Instantiate(selectedMap);

        Debug.Log($"�������� {stageIndex + 1} ����, �� {ranMapIndex + 1} ����");

        //�÷��̾� ���� ��ġ ����
        StartCoroutine(SpawnPlayerRoutine());

        //������ �ý��� �ʱ�ȭ
        InitializeMagicCircleSystem(curStage);

        OnStageStart?.Invoke(_curStageIndex);
    }

    /// <summary>
    /// �÷��̾ ���� ��ġ�� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnPlayerRoutine()
    {
        //�� ���� �Ϸ� ���
        yield return new WaitForEndOfFrame();

        Vector3 spawnPos = GetRanPosOnGround();

        if (spawnPos != Vector3.zero)
        {
            _player.position = spawnPos;
            Debug.Log($"�÷��̾� ���� : {spawnPos}");
        }
        else
        {
            Debug.LogError("������ ���� ��ġ�� ã�� �� �����ϴ�!");
        }
    }

    /// <summary>
    /// ������ �ý��� �ʱ�ȭ �Լ�
    /// </summary>
    /// <param name="data"></param>
    void InitializeMagicCircleSystem(StageData data)
    {
        //������ �ý��� ������Ʈ ����
        _magicCircleSystem = FindObjectOfType<MagicCircleSystem>();

        if (_magicCircleSystem == null)
        {
            GameObject magicCircleObj = new GameObject("MagicCircleSystem");
            _magicCircleSystem = magicCircleObj.AddComponent<MagicCircleSystem>();
        }

        //������ �ý��� �ʱ�ȭ
        _magicCircleSystem.Initialize(data.BossPrefab, this);
    }

    /// <summary>
    /// Gound Ÿ�� ������ ���� ��ġ�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRanPosOnGround()
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        List<Vector3> groundPositions = new();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                BoundsInt bounds = tilemap.cellBounds;

                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        Vector3Int cellPos = new Vector3Int(x, y, 0);
                        if (tilemap.HasTile(cellPos))
                        {
                            Vector3 woldPos = tilemap.CellToWorld(cellPos);
                            woldPos += new Vector3(0.5f, 0.5f, 0);
                            groundPositions.Add(woldPos);
                        }
                    }
                }
            }
        }

        if (groundPositions.Count > 0)
            return groundPositions[UnityEngine.Random.Range(0, groundPositions.Count)];

        return Vector3.zero;
    }

    /// <summary>
    /// ���������� Ŭ�����ϰ� ���� ���������� �Ѿ�� �Լ�
    /// </summary>
    public void CompleteStage()
    {
        OnStageComplete?.Invoke(_curStageIndex);
        Debug.Log($"�������� {_curStageIndex + 1} Ŭ����!");

        //���� ����������
        StartCoroutine(StartNextStageafterDelay(2f));
    }

    /// <summary>
    /// ���� ���������� �Ѿ�� �ڷ�ƾ
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator StartNextStageafterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartStage(_curStageIndex + 1);
    }
}