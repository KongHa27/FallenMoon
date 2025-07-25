using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 오브젝트 간 상호작용을 관리하는 클래스
/// </summary>
public class InteractionManager : MonoBehaviour
{
    [Header("----- 상호작용 설정 -----")]
    [SerializeField] float _interactionRange = 1f;      // 상호작용 가능 범위
    [SerializeField] LayerMask _interactableLayer = -1; // 상호작용 가능한 레이어

    Hero _player;
    IInteractable _currentInteractable;     // 현재 상호작용 가능한 오브젝트

    public static InteractionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 상호작용 매니저 초기화
    /// </summary>
    /// <param name="player"></param>
    public void Initialize(Hero player)
    {
        _player = player;
    }

    private void Update()
    {
        if (_player != null)
        {
            CheckForInteractables();
        }
    }

    /// <summary>
    /// 주변의 상호작용 가능한 오브젝트를 확인하는 함수
    /// </summary>
    void CheckForInteractables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_player.transform.position, _interactionRange, _interactableLayer);

        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract)
            {
                float distance = Vector2.Distance(_player.transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        // 현재 상호작용 대상이 변경되었을 때
        if (_currentInteractable != closestInteractable)
        {
            _currentInteractable = closestInteractable;

            // UI 업데이트 (상호작용 텍스트 표시/숨김)
            if (_currentInteractable != null)
            {
                ShowInteractionUI(_currentInteractable.GetInteractionText());
            }
            else
            {
                HideInteractionUI();
            }
        }
    }

    /// <summary>
    /// 상호작용 시도 함수
    /// </summary>
    public void TryInteract()
    {
        if (_currentInteractable != null && _currentInteractable.CanInteract)
        {
            _currentInteractable.Interact(_player.gameObject);

            // 상호작용 후 다시 체크 (1회성 오브젝트의 경우 CanInteract가 false가 될 수 있음)
            CheckForInteractables();
        }
    }

    /// <summary>
    /// 상호작용 UI 표시 함수
    /// </summary>
    /// <param name="text"></param>
    void ShowInteractionUI(string text)
    {
        // UI 매니저에 상호작용 텍스트 표시 요청
        // TODO: UI 매니저 구현 후 연결
        Debug.Log($"상호작용 가능: {text}");
    }

    /// <summary>
    /// 상호작용 UI 숨김 함수
    /// </summary>
    void HideInteractionUI()
    {
        // UI 매니저에 상호작용 텍스트 숨김 요청
        // TODO: UI 매니저 구현 후 연결
    }

    private void OnDrawGizmosSelected()
    {
        if (_player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_player.transform.position, _interactionRange);
        }
    }
}
