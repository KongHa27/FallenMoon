using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 마법진 시스템 클래스
/// </summary>
public class MagicCircleSystem : MonoBehaviour
{
    [Header("----- 마법진 설정 -----")]
    [SerializeField] GameObject _magicCirclePrefab;     //마법진 프리팹
    [SerializeField] float _chargingTime = 10f;         //충전 시간

    StageManager _stageManager;         //스테이지 매니저 참조

    GameObject _magicCircleInstance;    //마법진 인스턴스
    GameObject _bossInstance;           //보스몹 인스턴스
    bool _isCharging = false;           //충전 중인지 여부
    bool _isCharged = false;            //충전 완료 했는지 여부
    bool _isBossDead = false;           //보스가 죽었는지 여부

    public bool CanCompleteStage() => _isCharged && _isBossDead;

    public void Initialize(GameObject bossPrefab, StageManager manager)
    {
        _stageManager = manager;
        _isCharging = false;
        _isCharged = false;
        _isBossDead = false;

        //기존 마법진과 보스 제거
        if (_magicCircleInstance != null) Destroy(_magicCircleInstance);
        if (_bossInstance != null) Destroy(_bossInstance);

        //마법진 생성
        CreateMagicCircle();
    }

    /// <summary>
    /// 마법진 생성하는 함수
    /// </summary>
    void CreateMagicCircle()
    {
        Vector3 circlePos = StageManager.Instance.GetRanPosOnGround();

        if (circlePos != Vector3.zero)
        {
            if (_magicCirclePrefab != null)
                _magicCircleInstance = Instantiate(_magicCirclePrefab, circlePos, Quaternion.identity);
            else
            {
                Debug.LogError("마법진 소환 실패! 마법진 프리팹이 존재하지 않습니다!!");
                return;
            }

            //마법진에 상호작용 컴포넌트 추가
            MagicCircle magicCircle = _magicCircleInstance.GetComponent<MagicCircle>();
            if (magicCircle == null)
                magicCircle = _magicCircleInstance.AddComponent<MagicCircle>();

            //magicCircle.Initialize(this);

            Debug.Log($"마법진 생성 완료 : {circlePos}");
        }
    }

    /// <summary>
    /// 충전을 시작하는 함수
    /// </summary>
    public void StartCharging()
    {
        if (_isCharging) return;

        _isCharging = true;
        Debug.Log("마법진 충전 시작");

        //보스 소환
        SpawnBoss();

        //충전 코루틴 시작
        StartCoroutine(ChargingRoutine());
    }

    /// <summary>
    /// 랜덤 위치에 보스를 소환하는 함수
    /// </summary>
    void SpawnBoss()
    {
        //랜덤 위치 구하기
        Vector3 bossPos = StageManager.Instance.GetRanPosOnGround();

        if (bossPos != Vector3.zero)
        {
            if (_stageManager.CurStage.BossPrefab != null)
                _bossInstance = Instantiate(_stageManager.CurStage.BossPrefab, bossPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("보스 소환 실패! 보스 프리팹이 존재하지 않습니다!!");
            return;
        }

        //보스에 보스 처치 이벤트 연결
        Boss bossComponent = _bossInstance.GetComponent<Boss>();
        if (bossComponent == null)
            bossComponent = _bossInstance.AddComponent<Boss>();

        bossComponent.OnBossDead += OnBossDead;

        Debug.Log($"보스 소환 : {bossPos}");
    }

    IEnumerator ChargingRoutine()
    {
        float chargingTime = 0f;
         
    }
}
