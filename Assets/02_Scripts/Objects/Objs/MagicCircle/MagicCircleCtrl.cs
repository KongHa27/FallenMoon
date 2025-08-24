using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircleCtrl : MonoBehaviour
{
    [Range(0f, 1f)] public float charge;
    public MagicCircleLight coreLight;
    public RotateRing ring;
    public RuneLightUp[] runes;

    void Update()
    {
        charge = Mathf.Clamp01(charge + Time.deltaTime * 0.1f);

        coreLight.SetCharge(charge);

        int activeRunes = Mathf.FloorToInt(runes.Length * charge);
        for (int i = 0; i < activeRunes; i++)
            if (runes[i].SR.color.a < 1f) runes[i].Activate();
    }

}
