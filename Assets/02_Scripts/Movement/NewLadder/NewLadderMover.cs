using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLadderMover : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Collider2D _coll;
    [SerializeField] LayerMask _ladderLayer;
    
    [SerializeField] float _speed;
    
    float _gravityScale;
    
    
    public bool isLadder { get; private set; }
    public float OriginalGravity => _gravityScale;

    Jumper _jumper;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>();

        _jumper = GetComponent<Jumper>();

        _gravityScale = _rb.gravityScale;
    }

    public void Climb(float vInput)
    {
        if (!isLadder) return;

        if (_jumper != null && _jumper.CurState == JumpState.Jumping) return;

        if (Mathf.Abs(vInput) > 0.01f)
        {
            _rb.gravityScale = 0;
            _rb.velocity = new Vector2(_rb.velocity.x, vInput * _speed);
        }
        else
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
        }
    }

    public void ResetGravity()
    {
        _rb.gravityScale = _gravityScale;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;

            //if (_jumper != null)
            //{
            //    _jumper.IsOnLadder = true;
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;

            ResetGravity();

            //if (_jumper != null)
            //{
            //    _jumper.IsOnLadder = false;
            //}
        }
    }

    public bool IsOverlappingLadder()
    {
        return Physics2D.OverlapBox(_coll.bounds.center, _coll.bounds.size, 0, _ladderLayer) != null;
    }
}
