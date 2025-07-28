using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    [Header("��� ����")]
    public GameObject backgroundPrefab;
    public float parallaxSpeed = 0.3f;
    public Vector2 backgroundScale = new Vector2(1f, 1f);

    [Header("��ġ ����")]
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
        // ī�޶� ����
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera�� ã�� �� �����ϴ�!");
            return;
        }

        // ��� ������ ����
        if (backgroundPrefab != null)
        {
            GameObject background = Instantiate(backgroundPrefab);
            backgroundTransform = background.transform;

            // ����� ī�޶� ��ġ �������� ��ġ
            Vector3 cameraPos = mainCamera.transform.position;
            backgroundTransform.position = new Vector3(
                cameraPos.x + backgroundOffset.x,
                cameraPos.y + backgroundOffset.y,
                20f // ����� �ڷ�
            );

            // ��� ũ�� ����
            backgroundTransform.localScale = new Vector3(backgroundScale.x, backgroundScale.y, 1f);

            Debug.Log($"����� �����Ǿ����ϴ�. ��ġ: {backgroundTransform.position}");
        }
        else
        {
            Debug.LogError("Background Prefab�� �������� �ʾҽ��ϴ�!");
        }

        // ī�޶� �ʱ� ��ġ ����
        lastCameraPosition = mainCamera.transform.position;
    }

    void LateUpdate() // Update ��� LateUpdate ���
    {
        // �з����� ȿ�� - ī�޶� ������ �Ŀ� ����
        if (backgroundTransform != null && mainCamera != null)
        {
            // ī�޶��� ���� ��ġ�� �������� ��� ��ġ ���
            Vector3 cameraPos = mainCamera.transform.position;

            backgroundTransform.position = new Vector3(
                (cameraPos.x * parallaxSpeed) + backgroundOffset.x,
                (cameraPos.y * parallaxSpeed) + backgroundOffset.y,
                20f
            );
        }
    }
}
