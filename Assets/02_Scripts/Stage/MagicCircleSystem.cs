using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 마법진 시스템 클래스
/// </summary>
public class MagicCircleSystem : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] StageManager _stageManager;        //스테이지 매니저 참조
    [SerializeField] MagicCircle _magicCircle;          //마법진 컴포넌트
    [SerializeField] Hero _hero;

    [Header("----- 마법진 UI -----")]
    [SerializeField] GameObject _chargingTimer;
    [SerializeField] Image _chargingTimerBar;

    GameObject _magicCircleInstance;    //마법진 인스턴스
    GameObject _bossInstance;           //보스몹 인스턴스
    StageData _curStageData;            //현재 스테이지 데이터
    
    bool _isCharging = false;           //충전 중인지 여부
    bool _isCharged = false;            //충전 완료 했는지 여부
    bool _isBossDead = false;           //보스가 죽었는지 여부

    public void Initialize(StageData data, StageManager manager, Hero hero)
    {
        _stageManager = manager;
        _curStageData = data;
        _hero = hero;

        _isCharging = false;
        _isCharged = false;
        _isBossDead = false;

        //기존 보스 제거
        if (_bossInstance != null) Destroy(_bossInstance);

        //마법진 생성
        StartCoroutine(FindAndInitializeMagicCircle());
    }

    /// <summary>
    /// ObjectSystem에서 생성된 마법진을 찾아서 초기화하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FindAndInitializeMagicCircle()
    {
        // ObjectSystem에서 마법진 생성을 기다림
        yield return new WaitForSeconds(0.2f);

        ObjectSystem objectSystem = ObjectSystem.Instance;
        if (objectSystem != null)
        {
            _magicCircleInstance = objectSystem.GetMagicCircleInstance();
            if (_magicCircleInstance != null)
            {
                //마법진 컴포넌트 참조
                _magicCircle = _magicCircleInstance.GetComponent<MagicCircle>();
                if (_magicCircle == null)
                    _magicCircle = _magicCircleInstance.AddComponent<MagicCircle>();

                //마법진 초기화
                _magicCircle.Initialize(this, _curStageData.ChargingAnimationSpeed);

                //마법진에서 UI를 찾아서 연결
                FindUIComponentsInMagicCircle();

                Debug.Log("마법진 시스템 초기화 완료");
            }
            else
            {
                Debug.LogError("ObjectSystem에서 생성된 마법진을 찾을 수 없습니다!");
            }
        }
    }

    void FindUIComponentsInMagicCircle()
    {
        if (_magicCircleInstance == null)
        {
            Debug.LogError("마법진 인스턴스가 null입니다!");
            return;
        }

        // 마법진의 자식 오브젝트들 중에서 캔버스를 찾고, 그 안에서 UI 요소들을 찾음
        Canvas canvas = _magicCircleInstance.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            // chargingTimer라는 이름의 오브젝트를 찾음 (대소문자 구분 없이)
            Transform chargingTimerTransform = FindChildByName(canvas.transform, "chargingTimer");
            if (chargingTimerTransform != null)
            {
                _chargingTimer = chargingTimerTransform.gameObject;

                // chargingTimerBar라는 이름의 Image 컴포넌트를 찾음
                Transform chargingTimerBarTransform = FindChildByName(chargingTimerTransform, "chargingTimerBar");
                if (chargingTimerBarTransform != null)
                {
                    _chargingTimerBar = chargingTimerBarTransform.GetComponent<Image>();
                }
            }
        }
    }

    /// <summary>
    /// 자식 오브젝트를 이름으로 찾는 헬퍼 함수 (대소문자 구분 없이)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    Transform FindChildByName(Transform parent, string name)
    {
        // 직접 자식들 검색
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name.ToLower() == name.ToLower())
            {
                return child;
            }
        }

        // 재귀적으로 모든 하위 자식들 검색
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform result = FindChildByName(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// ObjectSystem에서 마법진이 생성되었을 때 호출되는 함수
    /// </summary>
    /// <param name="magicCircleInstance"></param>
    public void OnMagicCircleCreated(GameObject magicCircleInstance)
    {
        _magicCircleInstance = magicCircleInstance;

        //마법진 컴포넌트 참조
        _magicCircle = _magicCircleInstance.GetComponent<MagicCircle>();
        if (_magicCircle == null)
            _magicCircle = _magicCircleInstance.AddComponent<MagicCircle>();

        //마법진 초기화
        _magicCircle.Initialize(this, _curStageData.ChargingAnimationSpeed);

        //마법진에서 UI를 찾아서 연결
        FindUIComponentsInMagicCircle();

        Debug.Log("마법진이 ObjectSystem에서 생성되어 초기화되었습니다.");
    }

    /// <summary>
    /// 충전을 시작하는 함수
    /// </summary>
    public void StartCharging()
    {
        if (_isCharging) return;

        _isCharging = true;
        Debug.Log("마법진 충전 시작");

        if (_magicCircle != null)
        {
            _magicCircle.SetState(MagicCirclrState.Charging);
        }

        if (_chargingTimer != null)
        {
            _chargingTimer.SetActive(true);
            Debug.Log("충전 타이머 UI 활성화");
        }

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
            bossPos.y += 2f;
            if (_curStageData.BossPrefab != null)
                _bossInstance = Instantiate(_curStageData.BossPrefab, bossPos, Quaternion.identity);
            else
            {
                Debug.LogError("보스 소환 실패! 보스 프리팹이 존재하지 않습니다!!");
                return;
            }
        }

        //보스 컴포넌트 참조
        Boss bossComponent = _bossInstance.GetComponent<Boss>();
        if (bossComponent != null)
        {
            int curErosionLevel = DifficultyManager.Instance?.CurrentErosionLevel ?? 1;
            bossComponent.Initialize(curErosionLevel, false);
        }

        //보스에 보스 처치 이벤트 연결
        bossComponent.OnBossDead += OnBossDead;

        Debug.Log($"보스 소환 : {bossPos}");
    }

    /// <summary>
    /// 마법진을 충전하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator ChargingRoutine()
    {
        float chargingTimer = 0f;
        float chargingTime = _curStageData.ChargingTime;

        while (chargingTimer < chargingTime)
        {
            chargingTimer += Time.deltaTime;
            _chargingTimerBar.fillAmount = chargingTimer / chargingTime;
            yield return null;
        }

        _isCharged = true;

        Debug.Log("마법진 충전 완료");

        _chargingTimerBar.fillAmount = 1f;

        // 충전 완료 후 잠시 후 UI 비활성화
        StartCoroutine(HideChargingUIAfterDelay(2f));

        if (_magicCircle != null)
            _magicCircle.SetState(MagicCirclrState.Charged);

        CheckStageCompletion();
    }

    /// <summary>
    /// 지연 시간 후 충전 UI를 숨기는 코루틴
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator HideChargingUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_chargingTimer != null)
        {
            _chargingTimer.SetActive(false);
        }
    }

    /// <summary>
    /// 보스가 죽으면 호출되는 함수
    /// </summary>
    public void OnBossDead()
    {
        _isBossDead = true;
        Debug.Log("보스 처치!!");

        if (_hero != null)
            _hero.AddLightGauge(35f);

        CheckStageCompletion();
    }

    /// <summary>
    /// 스테이지를 완료 했는지 체크하는 함수
    /// 완료 조건 : 마법진 충전 완료 && 보스 처치
    /// </summary>
    void CheckStageCompletion()
    {
        if (_isCharged && _isBossDead)
        {
            Debug.Log("스테이지 클리어 조건 달성! 마법진과 상호작용하세요.");
            //조건 충족 후 마법진과 상호작용 시 실행할 것들
        }
    }

    /// <summary>
    /// 스테이지를 완료할 수 있는지 여부
    /// </summary>
    /// <returns></returns>
    public bool CanCompleteStage()
    {
        return _isCharged && _isBossDead;
    }

    /// <summary>
    /// 스테이지 완료 시 호출할 함수
    /// </summary>
    public void CompleteStage()
    {
        if (CanCompleteStage())
            _stageManager.CompleteStage();
        else
            Debug.Log("아직 스테이지를 클리어할 수 없습니다.");
    }
}
