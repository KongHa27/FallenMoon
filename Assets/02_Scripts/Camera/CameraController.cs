using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _player;

    // 맵 경계 설정
    [SerializeField] float mapMinX = -33.5f;
    [SerializeField] float mapMaxX = 33.5f;
    [SerializeField] float mapMinY = 0f;
    [SerializeField] float mapMaxY = 35f;

    // 카메라 오프셋
    [SerializeField] float yOffset = 2f;
    [SerializeField] float zOffset = -10f;

    private Camera cam;
    private float camHalfWidth;
    private float camHalfHeight;

    private void Start()
    {
        cam = GetComponent<Camera>();
        CalculateCameraSize();
    }

    private void CalculateCameraSize()
    {
        // 카메라의 시야 범위 계산
        float distance = Mathf.Abs(zOffset);
        camHalfHeight = distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void Update()
    {
        if (_player == null) return;

        Vector3 pos = transform.position;

        // 플레이어 위치를 기준으로 카메라 위치 설정
        pos.x = _player.position.x;
        pos.y = _player.position.y + yOffset;
        pos.z = _player.position.z + zOffset;

        // 카메라 시야 범위를 고려한 맵 경계 제한
        float clampedX = Mathf.Clamp(pos.x, mapMinX + camHalfWidth, mapMaxX - camHalfWidth);
        float clampedY = Mathf.Clamp(pos.y, mapMinY + camHalfHeight, mapMaxY - camHalfHeight);

        pos.x = clampedX;
        pos.y = clampedY;

        transform.position = pos;
    }
}
