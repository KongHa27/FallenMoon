using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformInteractor : MonoBehaviour
{
    [SerializeField] Collider2D _coll;
    [SerializeField] CompositeCollider2D _groundColl;

    bool _isPassThroughPlatform = false;

    Coroutine GetGroundRoutine;

    private void Awake()
    {
        _coll = GetComponent<Collider2D>();
        
        GetGroundRoutine = StartCoroutine(GetGroundColliderRoutine());
    }

    IEnumerator GetGroundColliderRoutine()
    {
        yield return new WaitForEndOfFrame();
        _groundColl = GameObject.FindWithTag("EffectorGround").GetComponent<CompositeCollider2D>();
    }

    public void PassThroughPlatform()
    {
        if (!_isPassThroughPlatform)
        {
            StartCoroutine(DisableCollision());
        }
    }

    IEnumerator DisableCollision()
    {
        _isPassThroughPlatform = true;

        Physics2D.IgnoreCollision(_coll, _groundColl, true);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(_coll, _groundColl, false);

        _isPassThroughPlatform = false;
    }
}
