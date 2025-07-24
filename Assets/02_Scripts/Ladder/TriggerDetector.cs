using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] Hero _hero;

    private void Awake()
    {
        if (_hero ==  null)
            _hero = GetComponentInParent<Hero>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("트리거 감지");

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            Debug.Log("사다리 감지 시작");
            LadderSystem ladder = collision.gameObject.GetComponent<LadderSystem>();
            if (ladder != null && _hero != null)
                _hero.OnLadderStateChanged(true, ladder);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("사다리 감지 종료");
        LadderSystem ladder = collision.gameObject.GetComponent<LadderSystem>();
        if (ladder != null && _hero != null)
            _hero.OnLadderStateChanged(false, ladder);
    }
}
