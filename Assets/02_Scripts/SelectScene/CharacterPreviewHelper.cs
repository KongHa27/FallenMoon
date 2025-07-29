using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPreviewHelper : MonoBehaviour
{
    [Header("----- ������ ���� -----")]
    [SerializeField] Transform _previewSpawnPoint; // ĳ���Ͱ� ������ ��ġ
    [SerializeField] Camera _previewCamera; // ������� ī�޶�
    [SerializeField] Light _mainLight; // ���� ����
    [SerializeField] bool _autoRotate = false; // �ڵ� ȸ�� ����
    [SerializeField] float _rotationSpeed = 30f; // ȸ�� �ӵ�

    [Header("----- ī�޶� ���� -----")]
    [SerializeField] Vector3 _cameraOffset = new Vector3(0, 1.5f, -3f);
    [SerializeField] Vector3 _cameraLookAtOffset = new Vector3(0, 1f, 0);

    [Header("----- ���� ���� -----")]
    [SerializeField] Color _lightColor = Color.white;
    [SerializeField] float _lightIntensity = 1.0f;
    [SerializeField] Vector3 _lightRotation = new Vector3(45f, -45f, 0f);

    private GameObject _currentPreviewCharacter;
    private CharacterSelectUI _characterSelectUI;

    private void Start()
    {
        _characterSelectUI = FindObjectOfType<CharacterSelectUI>();
        SetupPreviewEnvironment();
    }

    private void Update()
    {
        // �ڵ� ȸ��
        if (_autoRotate && _currentPreviewCharacter != null)
        {
            _currentPreviewCharacter.transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
        }
    }

    /// <summary>
    /// ������ ȯ�� ����
    /// </summary>
    private void SetupPreviewEnvironment()
    {
        // ���� ����Ʈ ����
        if (_previewSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("CharacterPreviewSpawnPoint");
            _previewSpawnPoint = spawnPoint.transform;
            _previewSpawnPoint.SetParent(transform);
        }

        // ī�޶� ����
        if (_previewCamera != null)
        {
            _previewCamera.transform.position = _previewSpawnPoint.position + _cameraOffset;
            _previewCamera.transform.LookAt(_previewSpawnPoint.position + _cameraLookAtOffset);
        }

        // ���� ����
        if (_mainLight != null)
        {
            _mainLight.color = _lightColor;
            _mainLight.intensity = _lightIntensity;
            _mainLight.transform.rotation = Quaternion.Euler(_lightRotation);
        }
    }

    /// <summary>
    /// ĳ���� ������ ����
    /// </summary>
    public void CreateCharacterPreview(CharacterData characterData)
    {
        // ���� ĳ���� ����
        if (_currentPreviewCharacter != null)
        {
            DestroyImmediate(_currentPreviewCharacter);
        }

        if (characterData?.CharacterPrefab == null) return;

        // �� ĳ���� ����
        _currentPreviewCharacter = Instantiate(characterData.CharacterPrefab, _previewSpawnPoint);

        // ��ġ�� ȸ�� ���� (CharacterData ���� ���)
        _currentPreviewCharacter.transform.localPosition = characterData.PreviewPosition;
        _currentPreviewCharacter.transform.localScale = characterData.PreviewScale;

        // Idle �ִϸ��̼� ���
        Animator animator = _currentPreviewCharacter.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            characterData.PlayIdleAnimation(animator);
        }

        // ������ �±� �߰�
        _currentPreviewCharacter.tag = "CharacterPreview";

        Debug.Log($"ĳ���� ������ ����: {characterData.CharacterName}");
    }

    /// <summary>
    /// ĳ���� ���� ȸ��
    /// </summary>
    public void RotateCharacter(float angle)
    {
        if (_currentPreviewCharacter != null)
        {
            _currentPreviewCharacter.transform.Rotate(0, angle, 0);
        }
    }

    /// <summary>
    /// �ڵ� ȸ�� ���
    /// </summary>
    public void ToggleAutoRotate()
    {
        _autoRotate = !_autoRotate;
    }

    /// <summary>
    /// ī�޶� ��
    /// </summary>
    public void ZoomCamera(float zoomAmount)
    {
        if (_previewCamera != null)
        {
            Vector3 direction = (_previewCamera.transform.position - _previewSpawnPoint.position).normalized;
            _previewCamera.transform.position += direction * zoomAmount;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    private void OnDestroy()
    {
        if (_currentPreviewCharacter != null)
        {
            DestroyImmediate(_currentPreviewCharacter);
        }
    }

    /// <summary>
    /// Gizmos�� ���� �ð�ȭ (�����Ϳ�����)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_previewSpawnPoint != null)
        {
            // ���� ����Ʈ ǥ��
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_previewSpawnPoint.position, 0.1f);

            // ī�޶� ��ġ ǥ��
            Vector3 cameraPos = _previewSpawnPoint.position + _cameraOffset;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(cameraPos, Vector3.one * 0.2f);

            // ī�޶󿡼� ���� ����Ʈ���� ����
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(cameraPos, _previewSpawnPoint.position + _cameraLookAtOffset);
        }
    }
}
