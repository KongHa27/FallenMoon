using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/CharacterData", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("----- 캐릭터 기본 정보 -----")]
    [SerializeField] string _characterName;
    [SerializeField] string _characterDescription;
    [SerializeField] Sprite _characterIcon;
    [SerializeField] GameObject _characterPrefab;


    [Header("----- 캐릭터 능력치 참조 -----")]
    [SerializeField] HeroData _heroData;

    // 프로퍼티
    public string CharacterName => _characterName;
    public string CharacterDescription => _characterDescription;
    public Sprite CharacterIcon => _characterIcon;
    public GameObject CharacterPrefab => _characterPrefab;
    public HeroData HeroData => _heroData;

    /// <summary>
    /// 캐릭터 데이터 유효성 검사
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(_characterName) &&
               _characterPrefab != null &&
               _heroData != null;
    }
}
