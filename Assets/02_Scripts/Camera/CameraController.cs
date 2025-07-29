using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _player;

    // �� ��� ����
    [SerializeField] float mapMinX = -33.5f;
    [SerializeField] float mapMaxX = 33.5f;
    [SerializeField] float mapMinY = 0f;
    [SerializeField] float mapMaxY = 35f;

    // ī�޶� ������
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
        // ī�޶��� �þ� ���� ���
        float distance = Mathf.Abs(zOffset);
        camHalfHeight = distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void Update()
    {
        if (_player == null) return;

        Vector3 pos = transform.position;

        // �÷��̾� ��ġ�� �������� ī�޶� ��ġ ����
        pos.x = _player.position.x;
        pos.y = _player.position.y + yOffset;
        pos.z = _player.position.z + zOffset;

        // ī�޶� �þ� ������ ����� �� ��� ����
        float clampedX = Mathf.Clamp(pos.x, mapMinX + camHalfWidth, mapMaxX - camHalfWidth);
        float clampedY = Mathf.Clamp(pos.y, mapMinY + camHalfHeight, mapMaxY - camHalfHeight);

        pos.x = clampedX;
        pos.y = clampedY;

        transform.position = pos;
    }
}
