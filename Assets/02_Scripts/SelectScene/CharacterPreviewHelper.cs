using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPreviewHelper : MonoBehaviour
{
    [Header("----- 프리뷰 설정 -----")]
    [SerializeField] Transform _previewSpawnPoint; // 캐릭터가 생성될 위치
    [SerializeField] Camera _previewCamera; // 프리뷰용 카메라
    [SerializeField] Light _mainLight; // 메인 조명
    [SerializeField] bool _autoRotate = false; // 자동 회전 여부
    [SerializeField] float _rotationSpeed = 30f; // 회전 속도

    [Header("----- 카메라 설정 -----")]
    [SerializeField] Vector3 _cameraOffset = new Vector3(0, 1.5f, -3f);
    [SerializeField] Vector3 _cameraLookAtOffset = new Vector3(0, 1f, 0);

    [Header("----- 조명 설정 -----")]
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
        // 자동 회전
        if (_autoRotate && _currentPreviewCharacter != null)
        {
            _currentPreviewCharacter.transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
        }
    }

    /// <summary>
    /// 프리뷰 환경 설정
    /// </summary>
    private void SetupPreviewEnvironment()
    {
        // 스폰 포인트 설정
        if (_previewSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("CharacterPreviewSpawnPoint");
            _previewSpawnPoint = spawnPoint.transform;
            _previewSpawnPoint.SetParent(transform);
        }

        // 카메라 설정
        if (_previewCamera != null)
        {
            _previewCamera.transform.position = _previewSpawnPoint.position + _cameraOffset;
            _previewCamera.transform.LookAt(_previewSpawnPoint.position + _cameraLookAtOffset);
        }

        // 조명 설정
        if (_mainLight != null)
        {
            _mainLight.color = _lightColor;
            _mainLight.intensity = _lightIntensity;
            _mainLight.transform.rotation = Quaternion.Euler(_lightRotation);
        }
    }

    /// <summary>
    /// 캐릭터 프리뷰 생성
    /// </summary>
    public void CreateCharacterPreview(CharacterData characterData)
    {
        // 기존 캐릭터 제거
        if (_currentPreviewCharacter != null)
        {
            DestroyImmediate(_currentPreviewCharacter);
        }

        if (characterData?.CharacterPrefab == null) return;

        // 새 캐릭터 생성
        _currentPreviewCharacter = Instantiate(characterData.CharacterPrefab, _previewSpawnPoint);

        // 위치와 회전 설정 (CharacterData 설정 사용)
        _currentPreviewCharacter.transform.localPosition = characterData.PreviewPosition;
        _currentPreviewCharacter.transform.localScale = characterData.PreviewScale;

        // Idle 애니메이션 재생
        Animator animator = _currentPreviewCharacter.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            characterData.PlayIdleAnimation(animator);
        }

        // 프리뷰 태그 추가
        _currentPreviewCharacter.tag = "CharacterPreview";

        Debug.Log($"캐릭터 프리뷰 생성: {characterData.CharacterName}");
    }

    /// <summary>
    /// 캐릭터 수동 회전
    /// </summary>
    public void RotateCharacter(float angle)
    {
        if (_currentPreviewCharacter != null)
        {
            _currentPreviewCharacter.transform.Rotate(0, angle, 0);
        }
    }

    /// <summary>
    /// 자동 회전 토글
    /// </summary>
    public void ToggleAutoRotate()
    {
        _autoRotate = !_autoRotate;
    }

    /// <summary>
    /// 카메라 줌
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
    /// 정리
    /// </summary>
    private void OnDestroy()
    {
        if (_currentPreviewCharacter != null)
        {
            DestroyImmediate(_currentPreviewCharacter);
        }
    }

    /// <summary>
    /// Gizmos로 설정 시각화 (에디터에서만)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_previewSpawnPoint != null)
        {
            // 스폰 포인트 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_previewSpawnPoint.position, 0.1f);

            // 카메라 위치 표시
            Vector3 cameraPos = _previewSpawnPoint.position + _cameraOffset;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(cameraPos, Vector3.one * 0.2f);

            // 카메라에서 스폰 포인트로의 라인
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(cameraPos, _previewSpawnPoint.position + _cameraLookAtOffset);
        }
    }
}
