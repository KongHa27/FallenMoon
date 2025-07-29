using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [Header("----- UI 컴포넌트 -----")]
    [SerializeField] Button _button;
    [SerializeField] Image _characterIconImage;
    [SerializeField] GameObject _selectedFrame; // 선택 시 활성화되는 프레임

    // 이벤트
    public event Action OnButtonClicked;

    // 데이터
    private int _characterIndex;
    private CharacterData _characterData;
    private bool _isSelected = false;

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// 캐릭터 버튼 초기화
    /// </summary>
    public void Initialize(int index, CharacterData characterData)
    {
        _characterIndex = index;
        _characterData = characterData;

        UpdateUI();
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    private void UpdateUI()
    {
        if (_characterData == null) return;

        // 캐릭터 아이콘 설정
        if (_characterIconImage != null && _characterData.CharacterIcon != null)
        {
            _characterIconImage.sprite = _characterData.CharacterIcon;
        }
    }

    /// <summary>
    /// 선택 상태 설정
    /// </summary>
    public void SetSelected(bool selected)
    {
        _isSelected = selected;

        // 선택 프레임 활성화/비활성화만 처리
        if (_selectedFrame != null)
        {
            _selectedFrame.SetActive(_isSelected);
        }
    }

    /// <summary>
    /// 버튼 클릭 이벤트
    /// </summary>
    private void OnClick()
    {
        OnButtonClicked?.Invoke();
    }

    /// <summary>
    /// 캐릭터 데이터 반환
    /// </summary>
    public CharacterData GetCharacterData()
    {
        return _characterData;
    }

    /// <summary>
    /// 캐릭터 인덱스 반환
    /// </summary>
    public int GetCharacterIndex()
    {
        return _characterIndex;
    }
}
