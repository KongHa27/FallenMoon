using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatusView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] Image _expBar;

    public void SetGoldText(int gold)
    {
        _goldText.text = $"{gold}";
    }

    public void SetHpText(float curHp, float maxHp)
    {
        _hpText.text = $"{(int)curHp} / {(int)maxHp}";
    }

    public void SetExpBar(float curExp, float maxExp)
    {
        _expBar.fillAmount = curExp / maxExp;
    }

    public void SetLevelText(int level)
    {
        _levelText.text = $"{level + 1}";
    }
}
