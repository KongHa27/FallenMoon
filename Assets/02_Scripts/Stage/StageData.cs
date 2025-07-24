using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    [SerializeField] int _stageNumber;         //스테이지 번호 (인덱스)
    [SerializeField] string _stageName;        //스테이지 이름

    [SerializeField] GameObject[] _mapPrefabs = new GameObject[3];     //스데이지 당 3개의 맵 프리팹

    [SerializeField] GameObject _bossPrefab;   //스테이지 보스 프리팹

    public int StageNumber => _stageNumber;
    public string StageName => _stageName;
    public GameObject[] MapPrefabs => _mapPrefabs;
    public GameObject BossPrefab => _bossPrefab;
}
