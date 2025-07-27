using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SlotType
{
    Skill,      // 스킬 슬롯
    Attack      // 기본 공격 슬롯
}

public class HeroSkillView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("----- 스킬 슬롯 UI -----")]
    [SerializeField] Image _skillIcon;                              // 스킬 아이콘
    [SerializeField] Image _cooldownFill;                           // 쿨다운 fillAmount 이미지
    [SerializeField] TextMeshProUGUI _cooldownText;                 // 쿨다운 시간 텍스트

    [Header("----- 툴팁 UI -----")]
    [SerializeField] GameObject _tooltipPanel;                      // 툴팁 패널
    [SerializeField] TextMeshProUGUI _skillNameText;                // 스킬 이름
    [SerializeField] TextMeshProUGUI _cooldownInfoText;             // 쿨타임 정보
    [SerializeField] TextMeshProUGUI _descriptionText;              // 스킬 설명

    [Header("----- 설정 -----")]
    [SerializeField] Color _numberHighlightColor = Color.green;     // 수치 강조 색상
    [SerializeField] bool _isBasicAttack = false;                   // 기본 공격 슬롯인지 여부

    [Header("----- 기본 공격 설정 -----")]
    [SerializeField] Sprite _basicAttackIcon;                       // 기본 공격 아이콘
    [SerializeField] string _basicAttackName;                       //기본 공격 이름
    [SerializeField] string _basicAttackDescription;                //기본 공격 설명

    SkillManager.SkillType _skillType;
    SkillManager _skillManager;
    AdventurerAttack _adventurerAttack;

    /// <summary>
    /// 스킬 슬롯 초기화 (스킬용)
    /// </summary>
    /// <param name="skillType">스킬 타입</param>
    /// <param name="skillManager">스킬 매니저</param>
    public void Initialize(SkillManager.SkillType skillType, SkillManager skillManager)
    {
        _skillType = skillType;
        _skillManager = skillManager;
        _isBasicAttack = false;

        // 스킬 데이터로 아이콘 설정
        var skill = _skillManager.GetSkill(_skillType);
        if (skill != null && skill.Data.icon != null)
        {
            _skillIcon.sprite = skill.Data.icon;
        }

        // 툴팁 초기에는 비활성화
        if (_tooltipPanel != null)
            _tooltipPanel.SetActive(false);

        // 쿨다운 이벤트 구독
        _skillManager.OnSkillCooldownChanged += OnSkillCooldownChanged;
    }

    /// <summary>
    /// 기본 공격 슬롯 초기화 (기본 공격용)
    /// </summary>
    /// <param name="adventurerAttack">원거리 공격 시스템</param>
    public void InitializeAsBasicAttack(AdventurerAttack adventurerAttack)
    {
        _adventurerAttack = adventurerAttack;
        _isBasicAttack = true;
        _basicAttackIcon = _adventurerAttack.AttackIcon;
        _basicAttackName = _adventurerAttack.AttackName;
        _basicAttackDescription = _adventurerAttack.AttackDesc;


        // 기본 공격 아이콘 설정
        if (_basicAttackIcon != null)
            _skillIcon.sprite = _basicAttackIcon;

        // 툴팁 초기에는 비활성화
        if (_tooltipPanel != null)
            _tooltipPanel.SetActive(false);
    }

    void Update()
    {
        // 기본 공격 슬롯인 경우 쿨다운 업데이트
        if (_isBasicAttack && _adventurerAttack != null)
        {
            float remainingCooldown = _adventurerAttack.GetCooldownRemaining();
            float maxCooldown = _adventurerAttack.AttackCooldown;

            UpdateCooldownUI(remainingCooldown, maxCooldown);
        }
    }
    /// <summary>
    /// 스킬 쿨다운 업데이트 (스킬매니저 이벤트용)
    /// </summary>
    /// <param name="skillType">스킬 타입</param>
    /// <param name="currentCooldown">현재 쿨다운</param>
    /// <param name="maxCooldown">최대 쿨다운</param>
    void OnSkillCooldownChanged(SkillManager.SkillType skillType, float currentCooldown, float maxCooldown)
    {
        // 자신의 스킬 타입이 아니거나 기본 공격 슬롯이면 무시
        if (skillType != _skillType || _isBasicAttack) return;

        UpdateCooldownUI(currentCooldown, maxCooldown);
    }

    /// <summary>
    /// 쿨다운 UI 업데이트 (공통)
    /// </summary>
    /// <param name="curCooldown">현재 쿨다운</param>
    /// <param name="maxCooldown">최대 쿨다운</param>
    void UpdateCooldownUI(float curCooldown, float maxCooldown)
    {
        // 쿨다운 fillAmount 업데이트
        if (_cooldownFill != null && maxCooldown > 0)
        {
            _cooldownFill.fillAmount = curCooldown / maxCooldown;
        }

        // 쿨다운 텍스트 업데이트
        if (_cooldownText != null)
        {
            if (curCooldown > 0)
            {
                _cooldownText.text = curCooldown.ToString("F1");
                _cooldownText.gameObject.SetActive(true);
            }
            else
            {
                _cooldownText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 마우스가 스킬 슬롯에 올라왔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("마우스 포인터 인");
        ShowTooltip();
    }

    /// <summary>
    /// 마우스가 스킬 슬롯에서 나갔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("마우스 포인터 아웃");
        HideTooltip();
    }

    /// <summary>
    /// 툴팁 표시
    /// </summary>
    void ShowTooltip()
    {
        if (_tooltipPanel == null) return;

        if (_isBasicAttack)
        {
            // 기본 공격 정보 표시
            ShowBasicAttackTooltip();
        }
        else
        {
            // 스킬 정보 표시
            ShowSkillTooltip();
        }

        // 툴팁 위치 설정 (스킬 슬롯 위쪽에 표시)
        Vector3 tooltipPosition = transform.position + Vector3.right * 150f + Vector3.up * 128f;
        _tooltipPanel.transform.position = tooltipPosition;

        _tooltipPanel.SetActive(true);
    }

    /// <summary>
    /// 스킬 툴팁 표시
    /// </summary>
    void ShowSkillTooltip()
    {
        var skill = _skillManager.GetSkill(_skillType);
        if (skill == null) return;

        var data = skill.Data;

        // 스킬 이름 설정
        if (_skillNameText != null)
            _skillNameText.text = data.skillName;

        // 쿨타임 정보 설정
        if (_cooldownInfoText != null)
            _cooldownInfoText.text = $"(쿨타임: {data.cooldown}초)";

        // 스킬 설명 설정 (수치 강조)
        if (_descriptionText != null)
            _descriptionText.text = HighlightNumbers(data.description);
    }

    /// <summary>
    /// 기본 공격 툴팁 표시
    /// </summary>
    void ShowBasicAttackTooltip()
    {
        if (_adventurerAttack == null) return;

        // 기본 공격 이름 설정
        if (_skillNameText != null)
            _skillNameText.text = _basicAttackName;

        // 쿨타임 정보 설정
        if (_cooldownInfoText != null)
            _cooldownInfoText.text = $"(쿨타임: {_adventurerAttack.AttackCooldown}초)";

        // 기본 공격 설명 설정
        if (_descriptionText != null)
            _descriptionText.text = HighlightNumbers(_basicAttackDescription);
    }

    /// <summary>
    /// 툴팁 숨기기
    /// </summary>
    void HideTooltip()
    {
        if (_tooltipPanel != null)
            _tooltipPanel.SetActive(false);
    }

    /// <summary>
    /// 텍스트에서 수치(숫자+%)를 강조 색상으로 변경
    /// </summary>
    /// <param name="text">원본 텍스트</param>
    /// <returns>수치가 강조된 텍스트</returns>
    string HighlightNumbers(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // 숫자+% 패턴 찾기 (예: 230%, 15초, 5m 등)
        string pattern = @"(\d+(?:\.\d+)?%?)";
        string colorHex = ColorUtility.ToHtmlStringRGB(_numberHighlightColor);

        string highlightedText = Regex.Replace(text, pattern, $"<color=#{colorHex}>$1</color>");

        return highlightedText;
    }

    /// <summary>
    /// 메모리 누수 방지를 위한 이벤트 구독 해제
    /// </summary>
    void OnDestroy()
    {
        if (_skillManager != null)
            _skillManager.OnSkillCooldownChanged -= OnSkillCooldownChanged;
    }
}
