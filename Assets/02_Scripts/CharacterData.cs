using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/CharacterData", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("----- ĳ���� �⺻ ���� -----")]
    [SerializeField] string _characterName;
    [SerializeField] Sprite _characterIcon; // UI ��ư�� ������

    [Header("----- 2D ĳ���� ���ҽ� -----")]
    [SerializeField] Sprite _characterPreviewSprite; // ���� ȭ�� ������� ū �̹���
    [SerializeField] GameObject _characterPrefab; // ���� ���ӿ��� ����� 2D ĳ���� ������

    [Header("----- ������ ���̵� �ִϸ��̼� ��������Ʈ -----")]
    [SerializeField] Sprite[] _previewIdleSprites; // ������� ���̵� �ִϸ��̼� ��������Ʈ��
    [SerializeField] float _animationFrameRate = 8f; // �ʴ� ������ ��

    [Header("----- ĳ���� �ɷ�ġ ���� -----")]
    [SerializeField] HeroData _heroData;

    // ������Ƽ
    public string CharacterName => _characterName;
    public Sprite CharacterIcon => _characterIcon;
    public Sprite CharacterPreviewSprite => _characterPreviewSprite;
    public GameObject CharacterPrefab => _characterPrefab;
    public Sprite[] PreviewIdleSprites => _previewIdleSprites;
    public float AnimationFrameRate => _animationFrameRate;
    public HeroData HeroData => _heroData;
}
