using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ������ �ý��� Ŭ����
/// </summary>
public class MagicCircleSystem : MonoBehaviour
{
    [Header("----- ������ ���� -----")]
    [SerializeField] GameObject _magicCirclePrefab;     //������ ������
    [SerializeField] float _chargingTime = 10f;         //���� �ð�

    StageManager _stageManager;         //�������� �Ŵ��� ����

    GameObject _magicCircleInstance;    //������ �ν��Ͻ�
    GameObject _bossInstance;           //������ �ν��Ͻ�
    bool _isCharging = false;           //���� ������ ����
    bool _isCharged = false;            //���� �Ϸ� �ߴ��� ����
    bool _isBossDead = false;           //������ �׾����� ����

    public bool CanCompleteStage() => _isCharged && _isBossDead;

    public void Initialize(GameObject bossPrefab, StageManager manager)
    {
        _stageManager = manager;
        _isCharging = false;
        _isCharged = false;
        _isBossDead = false;

        //���� �������� ���� ����
        if (_magicCircleInstance != null) Destroy(_magicCircleInstance);
        if (_bossInstance != null) Destroy(_bossInstance);

        //������ ����
        CreateMagicCircle();
    }

    /// <summary>
    /// ������ �����ϴ� �Լ�
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
                Debug.LogError("������ ��ȯ ����! ������ �������� �������� �ʽ��ϴ�!!");
                return;
            }

            //�������� ��ȣ�ۿ� ������Ʈ �߰�
            MagicCircle magicCircle = _magicCircleInstance.GetComponent<MagicCircle>();
            if (magicCircle == null)
                magicCircle = _magicCircleInstance.AddComponent<MagicCircle>();

            //magicCircle.Initialize(this);

            Debug.Log($"������ ���� �Ϸ� : {circlePos}");
        }
    }

    /// <summary>
    /// ������ �����ϴ� �Լ�
    /// </summary>
    public void StartCharging()
    {
        if (_isCharging) return;

        _isCharging = true;
        Debug.Log("������ ���� ����");

        //���� ��ȯ
        SpawnBoss();

        //���� �ڷ�ƾ ����
        StartCoroutine(ChargingRoutine());
    }

    /// <summary>
    /// ���� ��ġ�� ������ ��ȯ�ϴ� �Լ�
    /// </summary>
    void SpawnBoss()
    {
        //���� ��ġ ���ϱ�
        Vector3 bossPos = StageManager.Instance.GetRanPosOnGround();

        if (bossPos != Vector3.zero)
        {
            if (_stageManager.CurStage.BossPrefab != null)
                _bossInstance = Instantiate(_stageManager.CurStage.BossPrefab, bossPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("���� ��ȯ ����! ���� �������� �������� �ʽ��ϴ�!!");
            return;
        }

        //������ ���� óġ �̺�Ʈ ����
        Boss bossComponent = _bossInstance.GetComponent<Boss>();
        if (bossComponent == null)
            bossComponent = _bossInstance.AddComponent<Boss>();

        bossComponent.OnBossDead += OnBossDead;

        Debug.Log($"���� ��ȯ : {bossPos}");
    }

    IEnumerator ChargingRoutine()
    {
        float chargingTime = 0f;
         
    }
}
