using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [Header("----- UI ������Ʈ -----")]
    [SerializeField] Button _button;
    [SerializeField] Image _characterIconImage;
    [SerializeField] GameObject _selectedFrame; // ���� �� Ȱ��ȭ�Ǵ� ������

    // �̺�Ʈ
    public event Action OnButtonClicked;

    // ������
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
    /// ĳ���� ��ư �ʱ�ȭ
    /// </summary>
    public void Initialize(int index, CharacterData characterData)
    {
        _characterIndex = index;
        _characterData = characterData;

        UpdateUI();
    }

    /// <summary>
    /// UI ������Ʈ
    /// </summary>
    private void UpdateUI()
    {
        if (_characterData == null) return;

        // ĳ���� ������ ����
        if (_characterIconImage != null && _characterData.CharacterIcon != null)
        {
            _characterIconImage.sprite = _characterData.CharacterIcon;
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void SetSelected(bool selected)
    {
        _isSelected = selected;

        // ���� ������ Ȱ��ȭ/��Ȱ��ȭ�� ó��
        if (_selectedFrame != null)
        {
            _selectedFrame.SetActive(_isSelected);
        }
    }

    /// <summary>
    /// ��ư Ŭ�� �̺�Ʈ
    /// </summary>
    private void OnClick()
    {
        OnButtonClicked?.Invoke();
    }

    /// <summary>
    /// ĳ���� ������ ��ȯ
    /// </summary>
    public CharacterData GetCharacterData()
    {
        return _characterData;
    }

    /// <summary>
    /// ĳ���� �ε��� ��ȯ
    /// </summary>
    public int GetCharacterIndex()
    {
        return _characterIndex;
    }
}
