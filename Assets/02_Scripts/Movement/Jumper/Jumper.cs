using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpState
{
    Grounded,
    Jumping,
    Falling,
    Climbing,
}

[RequireComponent(typeof(Rigidbody2D))]
public class Jumper : MonoBehaviour
{
    public event Action<JumpState, JumpState> OnJumpStateChanged;

    [SerializeField] float _jumpPower;

    [SerializeField] Transform _groundChecker;
    [SerializeField] float _groundCheckerRadius;
    [SerializeField] LayerMask _groundLayerMask;


    Rigidbody2D _rigid;

    JumpState _state = JumpState.Grounded;
    public JumpState CurState => _state;

    public bool IsGrounded { get; private set; }
    public bool IsOnLadder { get; set; }

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckerRadius, _groundLayerMask);

        IsGrounded = isGrounded;

        if (IsOnLadder)
        {
            ChangeJumpState(JumpState.Climbing);
            return;
        }

        if (isGrounded)
            ChangeJumpState(JumpState.Grounded);
        else if (_rigid.velocity.y > 0)
            ChangeJumpState(JumpState.Jumping);
        else
            ChangeJumpState(JumpState.Falling);
    }

    /// <summary>
    /// 점프 실행 함수
    /// </summary>
    public void Jump()
    {
        //땅에 있을 때, 사다리 타고 있을 때만 점프 가능
        if (_state != JumpState.Grounded && _state != JumpState.Climbing) return;

        if (_state == JumpState.Climbing)
        {
            IsOnLadder = false;

            NewLadderMover ladderMover = GetComponent<NewLadderMover>();
            if (ladderMover != null)
            {
                _rigid.gravityScale = ladderMover.OriginalGravity;
            }
        }

        Vector2 velo = _rigid.velocity;
        velo.y = 0;
        _rigid.velocity = velo;

        _rigid.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);

        ChangeJumpState(JumpState.Jumping);
    }

    /// <summary>
    /// 점프 상태 변경 함수
    /// </summary>
    /// <param name="state"></param>
    void ChangeJumpState(JumpState state)
    {
        if (_state == state) return;

        JumpState curState = _state;
        _state = state;
        OnJumpStateChanged?.Invoke(curState, state);

        Debug.Log($"JumpState :: {_state}");
    }

    public void SetPower(float power)
    {
        _jumpPower = power;
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundChecker == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundChecker.position, _groundCheckerRadius);
    }
}
