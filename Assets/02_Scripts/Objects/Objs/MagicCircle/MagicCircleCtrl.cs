using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircleCtrl : MonoBehaviour
{
    [Range(0f, 1f)] public float charge;
    public MagicCircleLight coreLight;
    public RotateRing[] rings;
    public RuneLightUp[] runes;

    /*
    void Update()
    {
        charge = Mathf.Clamp01(charge + Time.deltaTime * 0.1f);

        coreLight.SetCharge(charge);

        int activeRunes = Mathf.FloorToInt(runes.Length * charge);
        for (int i = 0; i < activeRunes; i++)
            if (runes[i].SR.color.a < 1f) runes[i].Activate();
    }
    */

    public void SetCharge(float chargeValue)
    {
        this.charge = Mathf.Clamp01(chargeValue);

        // 코어 라이트와 룬 활성화 로직을 이 함수로 옮깁니다.
        coreLight.SetCharge(this.charge);

        for (int i = 0; i < rings.Length; i++)
        {
            rings[i].enabled = true;
        }

        int activeRunes = Mathf.FloorToInt(runes.Length * this.charge);
        for (int i = 0; i < activeRunes; i++)
        {
            // 아직 완전히 켜지지 않은 룬만 활성화합니다.
            if (runes[i].SR.color.a < 1f)
            {
                runes[i].Activate();
            }
        }
    }
}
