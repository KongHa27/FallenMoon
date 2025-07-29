using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/CharacterData", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("----- ĳ���� �⺻ ���� -----")]
    [SerializeField] string _characterName;
    [SerializeField] string _characterDescription;
    [SerializeField] Sprite _characterIcon; // UI ��ư�� ������

    [Header("----- 2D ĳ���� ���ҽ� -----")]
    [SerializeField] Sprite _characterPreviewSprite; // ���� ȭ�� ������� ū �̹���
    [SerializeField] RuntimeAnimatorController _characterAnimatorController; // 2D �ִϸ��̼� ��Ʈ�ѷ�
    [SerializeField] GameObject _characterPrefab; // ���� ���ӿ��� ����� 2D ĳ���� ������

    [Header("----- ĳ���� �ɷ�ġ ���� -----")]
    [SerializeField] HeroData _heroData;

    [Header("----- ������ ���� -----")]
    [SerializeField] Vector3 _previewScale = Vector3.one; // ������ �̹��� ũ��
    [SerializeField] Vector2 _previewPosition = Vector2.zero; // ������ �̹��� ��ġ ������

    [Header("----- �ִϸ��̼� ���� -----")]
    [SerializeField] string _idleAnimationParameter = "Idle"; // Animator �Ķ���� �̸�
    [SerializeField] string _idleAnimationState = "Idle"; // �ִϸ��̼� ���� �̸�
    [SerializeField] AnimationParameterType _parameterType = AnimationParameterType.Bool;

    // �ִϸ��̼� �Ķ���� Ÿ��
    public enum AnimationParameterType
    {
        Bool,
        Trigger,
        State // ���� ���� ���
    }

    // ������Ƽ
    public string CharacterName => _characterName;
    public string CharacterDescription => _characterDescription;
    public Sprite CharacterIcon => _characterIcon;
    public Sprite CharacterPreviewSprite => _characterPreviewSprite;
    public RuntimeAnimatorController CharacterAnimatorController => _characterAnimatorController;
    public GameObject CharacterPrefab => _characterPrefab;
    public HeroData HeroData => _heroData;

    // ������ ���� ������Ƽ
    public Vector3 PreviewScale => _previewScale;
    public Vector2 PreviewPosition => _previewPosition;

    // �ִϸ��̼� ���� ������Ƽ
    public string IdleAnimationParameter => _idleAnimationParameter;
    public string IdleAnimationState => _idleAnimationState;
    public AnimationParameterType ParameterType => _parameterType;

    /// <summary>
    /// ĳ���� ������ ��ȿ�� �˻�
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(_characterName) &&
               _characterIcon != null &&
               _characterPreviewSprite != null &&
               _heroData != null;
    }

    /// <summary>
    /// 2D Idle �ִϸ��̼� ��� (Animator�� ����)
    /// </summary>
    public void PlayIdleAnimation(Animator animator)
    {
        if (animator == null) return;

        switch (_parameterType)
        {
            case AnimationParameterType.Bool:
                if (HasParameter(animator, _idleAnimationParameter))
                {
                    // �ٸ� Bool �Ķ���͵��� false�� ����
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
    /// Animator�� Ư�� �Ķ���Ͱ� �ִ��� Ȯ��
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
    /// Animator�� Ư�� ���°� �ִ��� Ȯ��
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
    /// 2D �ִϸ��̼� Bool �Ķ���͵� ����
    /// </summary>
    private void ResetAnimationBools(Animator animator)
    {
        // �Ϲ����� 2D �ִϸ��̼� Bool �Ķ���͵�
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
    /// Animator �Ķ���� Ÿ�� ��������
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
