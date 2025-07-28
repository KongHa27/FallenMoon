using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/CharacterData", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("----- ĳ���� �⺻ ���� -----")]
    [SerializeField] string _characterName;
    [SerializeField] string _characterDescription;
    [SerializeField] Sprite _characterIcon;
    [SerializeField] GameObject _characterPrefab;


    [Header("----- ĳ���� �ɷ�ġ ���� -----")]
    [SerializeField] HeroData _heroData;

    // ������Ƽ
    public string CharacterName => _characterName;
    public string CharacterDescription => _characterDescription;
    public Sprite CharacterIcon => _characterIcon;
    public GameObject CharacterPrefab => _characterPrefab;
    public HeroData HeroData => _heroData;

    /// <summary>
    /// ĳ���� ������ ��ȿ�� �˻�
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(_characterName) &&
               _characterPrefab != null &&
               _heroData != null;
    }
}
