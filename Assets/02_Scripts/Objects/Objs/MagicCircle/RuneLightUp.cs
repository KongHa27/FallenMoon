using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneLightUp : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float duration = 0.5f;

    public SpriteRenderer SR => sr;

    public void Activate()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        Color c = sr.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            sr.color = new Color(c.r, c.g, c.b, t / duration);
            yield return null;
        }
    }
}
