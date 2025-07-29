using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectButton : MonoBehaviour
{
    [Header("----- ���̵� ���� -----")]
    [SerializeField] DifficultyManager.SelectDifficulty _difficulty;

    [Header("----- ��ư ������Ʈ -----")]
    [SerializeField] Button _button;

    private CharacterSelectUI _selectUI;

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();
    }

    private void Start()
    {
        // CharacterSelectUI ã��
        _selectUI = FindObjectOfType<CharacterSelectUI>();

        // ��ư �̺�Ʈ ����
        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    /// <summary>
    /// Inspector���� ȣ�� ������ �Լ��� (�� ���̵���)
    /// </summary>
    public void SelectEasy()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Easy);
    }

    public void SelectNormal()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Normal);
    }

    public void SelectHard()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Hard);
    }

    public void SelectNightmare()
    {
        SelectDifficulty(DifficultyManager.SelectDifficulty.Nightmare);
    }

    /// <summary>
    /// ��ư Ŭ�� �� ȣ�� (�ڵ忡�� �ڵ� ����)
    /// </summary>
    private void OnButtonClick()
    {
        SelectDifficulty(_difficulty);
    }

    /// <summary>
    /// ���̵� ���� ����
    /// </summary>
    private void SelectDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        if (_selectUI != null)
        {
            _selectUI.SelectDifficulty(difficulty);
        }
        else if (GameManager.Instance != null)
        {
            // CharacterSelectUI�� ���ٸ� ���� GameManager ȣ��
            GameManager.Instance.SelectDifficulty(difficulty);
        }

        Debug.Log($"���̵� ����: {difficulty}");
    }

    /// <summary>
    /// Inspector���� ���̵� ���� (������ ����)
    /// </summary>
    public void SetDifficulty(DifficultyManager.SelectDifficulty difficulty)
    {
        _difficulty = difficulty;
    }
}
