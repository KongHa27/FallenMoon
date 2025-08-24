using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class MagicCircleLight : MonoBehaviour
{
    //[SerializeField] UnityEngine.Rendering.Universal.Light2D coreLight;
    [SerializeField] Light2D coreLight;
    [SerializeField] float maxIntensity = 5f;
    [SerializeField] float maxRadius = 3f;

    public Transform coreTr;
    public float maxScale = 0.69f;

    public void SetCharge(float charge) // 0~1
    {
        /*
        coreLight.intensity = Mathf.Lerp(0, maxIntensity, charge);
        coreLight.pointLightOuterRadius = Mathf.Lerp(0.5f, maxRadius, charge);
        */

        coreLight.color.a = Mathf.Lerp(0, maxIntensity, charge);
        coreLight.size = Mathf.Lerp(0.5f, maxRadius, charge);

        float scale = Mathf.Lerp(0.1f, maxScale, charge);
        coreTr.localScale = new Vector3(scale, scale, 1f);
    }
}
