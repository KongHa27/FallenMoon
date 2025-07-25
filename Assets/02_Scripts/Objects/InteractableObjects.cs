using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 타입 enum
/// </summary>
public enum ObjectType
{
    MagicCircle,    //마법진
    Urn,            //항아리
    Chest,          //상자
    Altar,          //제단
}

/// <summary>
/// 상호작용 가능한 오브젝트들의 베이스 클래스
/// </summary>
public abstract class InteractableObjects : MonoBehaviour, IInteractable
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected Collider2D _collider;

    [Header("----- 기본 설정 -----")]
    [SerializeField] protected ObjectType _objectType;
    [SerializeField] protected string _objectName;
    [SerializeField] protected Sprite _sprite;          //기본 스프라이트 이미지

    [Header("----- 상호작용 설정 -----")]
    [SerializeField] protected bool _canInteract = true;                //상호작용 가능 여부
    [SerializeField] protected bool _destroyAfterInteraction = false;   //상호작용 후 파괴 여부 (항아리)
    [SerializeField] protected float _destroyDelay = 1f;                // 파괴 지연 시간
    [SerializeField] protected float _interactionCooldown = 0f;         //상호작용 쿨타임

    protected float _lastInteractionTime;   //마지막 상호작용 시간 (쿨타임 재기 용)
    protected bool _hasInteracted = false;  //상호작용 했는지 여부

    //프로퍼티
    public ObjectType ObjectType => _objectType;
    public string ObjectName => _objectName;
    public virtual bool CanInteract => _canInteract && Time.time >= _lastInteractionTime + _interactionCooldown
                                       && (!_destroyAfterInteraction || !_hasInteracted);
    // : (상호작용 가능한지) && (상호작용 가능 쿨타임이 돌았는지) && (상호작용 후 파괴 또는 이미 상호작용하지 않았는지)
    //  모든 괄호 안이 true를 반환하면 CanInteract는 true를 반환


    protected virtual void Awake()
    {
        if (_renderer == null)
            _renderer = GetComponentInChildren<SpriteRenderer>();
        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        if (_sprite != null && _renderer != null)
            _renderer.sprite = _sprite;
    }

    protected virtual void Start()
    {
        //오브젝트 매니저에 등록
    }

    protected virtual void OnDestroy()
    {
        //오브젝트 매니저에서 해제
    }

    /// <summary>
    /// 상호작용 가능 여부를 체크하고, 상호작용 함수를 호출하는 함수
    /// </summary>
    /// <param name="interactor"></param>
    public virtual void Interact(GameObject interactor)
    {
        if (!CanInteract) return;

        _lastInteractionTime = Time.time;
        _hasInteracted = true;

        OnInteract(interactor);

        if (_destroyAfterInteraction)
            StartCoroutine(DestroyAfterDelay());
    }

    /// <summary>
    /// 상호작용했을 때 호출되는 함수,
    /// 각 오브젝트 클래스에서 오버라이드
    /// </summary>
    /// <param name="interactor">상호작용한 사람</param>
    protected abstract void OnInteract(GameObject interactor);

    /// <summary>
    /// 상호작용 범위 내로 다가갔을 시 Text를 표시하는 함수
    /// </summary>
    /// <returns></returns>
    public abstract string GetInteractionText();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //UI 표시 등 처리

        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //UI 숨김 등 처리

        }
    }

    /// <summary>
    /// 지연 후 파괴하는 코루틴
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(_destroyDelay);

        // 오브젝트 시스템에 파괴 알림
        if (ObjectSystem.Instance != null)
        {
            ObjectSystem.Instance.OnObjectDestroyed(gameObject);
        }

        Destroy(gameObject);
    }
}
