using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/CharacterData", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("----- 캐릭터 기본 정보 -----")]
    [SerializeField] string _characterName;
    [SerializeField] Sprite _characterIcon; // UI 버튼용 아이콘

    [Header("----- 2D 캐릭터 리소스 -----")]
    [SerializeField] Sprite _characterPreviewSprite; // 선택 화면 프리뷰용 큰 이미지
    [SerializeField] GameObject _characterPrefab; // 실제 게임에서 사용할 2D 캐릭터 프리팹

    [Header("----- 프리뷰 아이들 애니메이션 스프라이트 -----")]
    [SerializeField] Sprite[] _previewIdleSprites; // 프리뷰용 아이들 애니메이션 스프라이트들
    [SerializeField] float _animationFrameRate = 8f; // 초당 프레임 수

    [Header("----- 캐릭터 능력치 정보 -----")]
    [SerializeField] HeroData _heroData;

    // 프로퍼티
    public string CharacterName => _characterName;
    public Sprite CharacterIcon => _characterIcon;
    public Sprite CharacterPreviewSprite => _characterPreviewSprite;
    public GameObject CharacterPrefab => _characterPrefab;
    public Sprite[] PreviewIdleSprites => _previewIdleSprites;
    public float AnimationFrameRate => _animationFrameRate;
    public HeroData HeroData => _heroData;
}
