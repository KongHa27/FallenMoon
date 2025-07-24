using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    [SerializeField] int _stageNumber;         //�������� ��ȣ (�ε���)
    [SerializeField] string _stageName;        //�������� �̸�

    [SerializeField] GameObject[] _mapPrefabs = new GameObject[3];     //�������� �� 3���� �� ������

    [SerializeField] GameObject _bossPrefab;   //�������� ���� ������

    public int StageNumber => _stageNumber;
    public string StageName => _stageName;
    public GameObject[] MapPrefabs => _mapPrefabs;
    public GameObject BossPrefab => _bossPrefab;
}
