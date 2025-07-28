using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    [Header("배경 설정")]
    public GameObject backgroundPrefab;
    public float parallaxSpeed = 0.3f;
    public Vector2 backgroundScale = new Vector2(1f, 1f);

    [Header("위치 설정")]
    public Vector2 backgroundOffset = Vector2.zero;

    private Transform backgroundTransform;
    private Camera mainCamera;
    private Vector3 lastCameraPosition;

    void Start()
    {
        SetupBackground();
    }

    void SetupBackground()
    {
        // 카메라 참조
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다!");
            return;
        }

        // 배경 프리팹 생성
        if (backgroundPrefab != null)
        {
            GameObject background = Instantiate(backgroundPrefab);
            backgroundTransform = background.transform;

            // 배경을 카메라 위치 기준으로 배치
            Vector3 cameraPos = mainCamera.transform.position;
            backgroundTransform.position = new Vector3(
                cameraPos.x + backgroundOffset.x,
                cameraPos.y + backgroundOffset.y,
                20f // 충분히 뒤로
            );

            // 배경 크기 조정
            backgroundTransform.localScale = new Vector3(backgroundScale.x, backgroundScale.y, 1f);

            Debug.Log($"배경이 생성되었습니다. 위치: {backgroundTransform.position}");
        }
        else
        {
            Debug.LogError("Background Prefab이 설정되지 않았습니다!");
        }

        // 카메라 초기 위치 저장
        lastCameraPosition = mainCamera.transform.position;
    }

    void LateUpdate() // Update 대신 LateUpdate 사용
    {
        // 패럴랙스 효과 - 카메라가 움직인 후에 실행
        if (backgroundTransform != null && mainCamera != null)
        {
            // 카메라의 현재 위치를 기준으로 배경 위치 계산
            Vector3 cameraPos = mainCamera.transform.position;

            backgroundTransform.position = new Vector3(
                (cameraPos.x * parallaxSpeed) + backgroundOffset.x,
                (cameraPos.y * parallaxSpeed) + backgroundOffset.y,
                20f
            );
        }
    }
}
