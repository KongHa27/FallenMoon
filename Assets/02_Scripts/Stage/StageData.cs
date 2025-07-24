using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/StageData", fileName = "StageData")]
public class StageData : ScriptableObject
{
    [Header("----- 스테이지 정보 -----")]
    [SerializeField] int _stageNumber;         //스테이지 번호 (인덱스)
    [SerializeField] string _stageName;        //스테이지 이름

    [Header("----- 맵 설정 -----")]
    [SerializeField] GameObject[] _mapPrefabs = new GameObject[3];     //스데이지 당 3개의 맵 프리팹

    [Header("----- 보스 설정 -----")]
    [SerializeField] GameObject _bossPrefab;   //스테이지 보스 프리팹

    [Header("----- 마법진 설정 -----")]
    [SerializeField] float _chargingTime = 10f;             //마법진 충전 시간
    [SerializeField] float _chargingAnimationSpeed = 2f;    //충전 중 애니메이션 속도

    //프로퍼티
    public int StageNumber => _stageNumber;
    public string StageName => _stageName;
    public GameObject[] MapPrefabs => _mapPrefabs;
    public GameObject BossPrefab => _bossPrefab;
    public float ChargingTime => _chargingTime;
    public float ChargingAnimationSpeed => _chargingAnimationSpeed;

    private void OnValidate()
    {
        // 스테이지 번호가 음수가 되지 않도록
        if (_stageNumber < 0)
            _stageNumber = 0;

        // 충전 시간이 0보다 작아지지 않도록
        if (_chargingTime <= 0)
            _chargingTime = 1f;

        // 애니메이션 속도가 0보다 작아지지 않도록
        if (_chargingAnimationSpeed <= 0)
            _chargingAnimationSpeed = 1f;
    }
}
