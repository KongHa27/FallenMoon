using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/CharacterData", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("----- 캐릭터 기본 정보 -----")]
    [SerializeField] string _characterName;
    [SerializeField] string _characterDescription;
    [SerializeField] Sprite _characterIcon; // UI 버튼용 아이콘

    [Header("----- 2D 캐릭터 리소스 -----")]
    [SerializeField] Sprite _characterPreviewSprite; // 선택 화면 프리뷰용 큰 이미지
    [SerializeField] RuntimeAnimatorController _characterAnimatorController; // 2D 애니메이션 컨트롤러
    [SerializeField] GameObject _characterPrefab; // 실제 게임에서 사용할 2D 캐릭터 프리팹

    [Header("----- 캐릭터 능력치 참조 -----")]
    [SerializeField] HeroData _heroData;

    [Header("----- 프리뷰 설정 -----")]
    [SerializeField] Vector3 _previewScale = Vector3.one; // 프리뷰 이미지 크기
    [SerializeField] Vector2 _previewPosition = Vector2.zero; // 프리뷰 이미지 위치 오프셋

    [Header("----- 애니메이션 설정 -----")]
    [SerializeField] string _idleAnimationParameter = "Idle"; // Animator 파라미터 이름
    [SerializeField] string _idleAnimationState = "Idle"; // 애니메이션 상태 이름
    [SerializeField] AnimationParameterType _parameterType = AnimationParameterType.Bool;

    // 애니메이션 파라미터 타입
    public enum AnimationParameterType
    {
        Bool,
        Trigger,
        State // 직접 상태 재생
    }

    // 프로퍼티
    public string CharacterName => _characterName;
    public string CharacterDescription => _characterDescription;
    public Sprite CharacterIcon => _characterIcon;
    public Sprite CharacterPreviewSprite => _characterPreviewSprite;
    public RuntimeAnimatorController CharacterAnimatorController => _characterAnimatorController;
    public GameObject CharacterPrefab => _characterPrefab;
    public HeroData HeroData => _heroData;

    // 프리뷰 관련 프로퍼티
    public Vector3 PreviewScale => _previewScale;
    public Vector2 PreviewPosition => _previewPosition;

    // 애니메이션 관련 프로퍼티
    public string IdleAnimationParameter => _idleAnimationParameter;
    public string IdleAnimationState => _idleAnimationState;
    public AnimationParameterType ParameterType => _parameterType;

    /// <summary>
    /// 캐릭터 데이터 유효성 검사
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(_characterName) &&
               _characterIcon != null &&
               _characterPreviewSprite != null &&
               _heroData != null;
    }

    /// <summary>
    /// 2D Idle 애니메이션 재생 (Animator에 적용)
    /// </summary>
    public void PlayIdleAnimation(Animator animator)
    {
        if (animator == null) return;

        switch (_parameterType)
        {
            case AnimationParameterType.Bool:
                if (HasParameter(animator, _idleAnimationParameter))
                {
                    // 다른 Bool 파라미터들을 false로 설정
                    ResetAnimationBools(animator);
                    animator.SetBool(_idleAnimationParameter, true);
                }
                break;

            case AnimationParameterType.Trigger:
                if (HasParameter(animator, _idleAnimationParameter))
                {
                    animator.SetTrigger(_idleAnimationParameter);
                }
                break;

            case AnimationParameterType.State:
                if (HasState(animator, _idleAnimationState))
                {
                    animator.Play(_idleAnimationState);
                }
                break;
        }
    }

    /// <summary>
    /// Animator에 특정 파라미터가 있는지 확인
    /// </summary>
    private bool HasParameter(Animator animator, string paramName)
    {
        if (string.IsNullOrEmpty(paramName)) return false;

        foreach (var param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Animator에 특정 상태가 있는지 확인
    /// </summary>
    private bool HasState(Animator animator, string stateName)
    {
        if (string.IsNullOrEmpty(stateName)) return false;

        try
        {
            return animator.HasState(0, Animator.StringToHash(stateName));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 2D 애니메이션 Bool 파라미터들 리셋
    /// </summary>
    private void ResetAnimationBools(Animator animator)
    {
        // 일반적인 2D 애니메이션 Bool 파라미터들
        string[] commonBools = { "IsWalking", "IsRunning", "IsAttacking", "IsJumping", "IsDead", "IsMoving" };

        foreach (string boolParam in commonBools)
        {
            if (HasParameter(animator, boolParam) && boolParam != _idleAnimationParameter)
            {
                AnimatorControllerParameterType? paramType = GetParameterType(animator, boolParam);
                if (paramType.HasValue && paramType.Value == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(boolParam, false);
                }
            }
        }
    }

    /// <summary>
    /// Animator 파라미터 타입 가져오기
    /// </summary>
    private AnimatorControllerParameterType? GetParameterType(Animator animator, string paramName)
    {
        foreach (var param in animator.parameters)
        {
            if (param.name == paramName)
                return param.type;
        }
        return null;
    }
}
