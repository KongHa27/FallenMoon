using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 전체 스테이지를 관리하는 클래스
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("----- 스테이지 설정 -----")]
    [SerializeField] StageData[] _stageDatas;   //스테이지 배열
    [SerializeField] Transform _player;         //플레이어
    [SerializeField] LayerMask _ground;         //ground 타일 레이어

    [Header("----- UI -----")]
    [SerializeField] GameObject _stageUI;
    [SerializeField] TextMeshProUGUI _stageNameTMP;
    [SerializeField] TextMeshProUGUI _mapNameTMP;
    [SerializeField] float _fadeDuration = 1.5f;

    MagicCircleSystem _magicCircleSystem;       //마법진 시스템 참조

    int _curStageIndex = 0;         //현재 스테이지 인덱스
    GameObject _curMapInstance;     //현재 스테이지 맵

    //스테이지 매니저 인스턴스
    public static StageManager Instance { get; private set; }
    public int CurStageIndex => _curStageIndex;
    public StageData CurStage => _stageDatas[_curStageIndex];
    public int TotalStageCount => _stageDatas.Length;

    //이벤트
    public event Action<int> OnStageStart;      //스테이지 시작 이벤트
    public event Action<int> OnStageComplete;   //스테이지 완료 이벤트
    public event Action OnAllStageComplete;     //모든 스테이지 완료 이벤트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        GameObject defaultMap = GameObject.Find("Grid");
        Destroy(defaultMap);
    }

    private void Start()
    {
        if (_stageDatas == null || _stageDatas.Length == 0)
        {
            Debug.LogError("스테이지 데이터가 설정되지 않았습니다.");
            return;
        }

        StartStage(0);
    }

    /// <summary>
    /// 스테이지를 시작하는 함수
    /// 랜덤 맵 선택, 플레이어 및 마법진을 랜덤 위치에 생성
    /// </summary>
    /// <param name="stageIndex"></param>
    public void StartStage(int stageIndex)
    {
        //모든 스테이지 클리어 시
        if (stageIndex >= _stageDatas.Length)
        {
            OnAllStageComplete?.Invoke();
            Debug.Log("모든 스테이지 클리어!");
            return;
        }

        _curStageIndex = stageIndex;
        StageData curStage = _stageDatas[_curStageIndex];

        //기존 맵 제거
        if (_curMapInstance != null)
            Destroy(_curMapInstance);

        //랜덤 맵 선택
        int ranMapIndex = UnityEngine.Random.Range(0, curStage.MapPrefabs.Length);
        GameObject selectedMap = curStage.MapPrefabs[ranMapIndex];

        if (selectedMap == null)
        {
            Debug.LogError($"스테이지 {stageIndex}의 맵 {ranMapIndex}가 존재하지 않습니다.");
            return;
        }

        //맵 생성
        _curMapInstance = Instantiate(selectedMap);
        Debug.Log($"스테이지 {stageIndex + 1} 시작, 맵 {ranMapIndex + 1} 선택");

        //UI 표시
        StartCoroutine(SetStageUIRoutine(stageIndex, ranMapIndex));

        //플레이어 랜덤 위치 스폰
        StartCoroutine(SpawnPlayerRoutine());

        //마법진 시스템 초기화
        InitializeMagicCircleSystem(curStage);

        //스테이지 시작 이벤트 발행
        OnStageStart?.Invoke(_curStageIndex);
    }

    /// <summary>
    /// 플레이어를 랜덤 위치에 생성하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnPlayerRoutine()
    {
        //맵 생성 완료 대기
        yield return new WaitForEndOfFrame();

        Vector3 spawnPos = GetRanPosOnGround();

        if (spawnPos != Vector3.zero)
        {
            _player.position = spawnPos;
            Debug.Log($"플레이어 스폰 : {spawnPos}");
        }
        else
        {
            Debug.LogError("적절한 스폰 위치를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 마법진 시스템 초기화 함수
    /// </summary>
    /// <param name="data"></param>
    void InitializeMagicCircleSystem(StageData data)
    {
        //마법진 시스템 컴포넌트 참조
        _magicCircleSystem = FindObjectOfType<MagicCircleSystem>();

        if (_magicCircleSystem == null)
        {
            GameObject magicCircleObj = new GameObject("MagicCircleSystem");
            _magicCircleSystem = magicCircleObj.AddComponent<MagicCircleSystem>();
        }

        //마법진 시스템 초기화
        _magicCircleSystem.Initialize(data, this, _player.GetComponent<Hero>());
    }

    /// <summary>
    /// Gound 타일 위에서 랜덤 위치를 반환하는 함수
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
    /// 스테이지를 클리어하고 다음 스테이지로 넘어가는 함수
    /// </summary>
    public void CompleteStage()
    {
        OnStageComplete?.Invoke(_curStageIndex);
        Debug.Log($"스테이지 {_curStageIndex + 1} 클리어!");

        //다음 스테이지로
        StartCoroutine(StartNextStageafterDelay(2f));
    }

    /// <summary>
    /// 딜레이 후 다음 스테이지로 넘어가는 코루틴
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator StartNextStageafterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartStage(_curStageIndex + 1);
    }

    /// <summary>
    /// 특정 스테이지로 이동 (디버그용)
    /// </summary>
    /// <param name="stageIndex"></param>
    public void GoToStage(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < _stageDatas.Length)
            StartStage(stageIndex);
        else
            Debug.LogWarning($"잘못된 스테이지 인덱스 {stageIndex}");
    }

    /// <summary>
    /// 스테이지UI 텍스트를 표시하는 코루틴
    /// </summary>
    /// <param name="stageIndex"></param>
    /// <param name="selectedMap"></param>
    /// <returns></returns>
    IEnumerator SetStageUIRoutine(int stageIndex, int mapIndex)
    {
        //맵 생성 대기
        yield return new WaitForEndOfFrame();

        // 알파값 초기화
        SetTextAlpha(_stageNameTMP, 1f);
        SetTextAlpha(_mapNameTMP, 1f);

        _stageNameTMP.text = $"Stage {stageIndex}. {_stageDatas[stageIndex].StageName}";
        _mapNameTMP.text = _stageDatas[stageIndex].MapNames[mapIndex];

        _stageUI.SetActive(true);

        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeOutText(_stageNameTMP));
        StartCoroutine(FadeOutText(_mapNameTMP));

        yield return new WaitForSeconds(_fadeDuration);

        _stageUI.SetActive(false);
    }

    void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        var c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
    }

    IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        float elapsed = 0f;
        Color startColor = text.color;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        text.color = new Color(startColor.r, startColor.g, startColor.b, 0f); // 완전히 투명하게
    }
}