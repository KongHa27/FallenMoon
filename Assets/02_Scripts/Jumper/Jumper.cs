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
    public event Action<JumpState> OnJumpStateChanged;

    [SerializeField] float _jumpPower;

    [SerializeField] Transform _groundChecker;
    [SerializeField] float _groundCheckerRadius;
    [SerializeField] LayerMask _groundLayerMask;

    Rigidbody2D _rigid;
    JumpState _state = JumpState.Grounded;

    public bool IsGrounded;
    public bool IsOnLadder { get; set; }

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckerRadius, _groundLayerMask);
        IsGrounded = isGrounded;

        float velocityY = _rigid.velocity.y;

        //사다리 타고 있을 때
        if (IsOnLadder)
        {
            ChangeJumpState(JumpState.Climbing);
        }
        //사다리 타고 있다가, 사다리에서 벗어났을 때
        else if (_state == JumpState.Climbing && !IsOnLadder)
        {
            ChangeJumpState(JumpState.Falling);
        }
        //점프 중(공중)이지만, 중력이 0 이하일 때 (점프 했다가 떨어지는 중)
        else if (_state == JumpState.Jumping && velocityY < 0)
        {
            ChangeJumpState(JumpState.Falling);
        }
        //추락 중에 땅에 닿았을 때
        else if (_state == JumpState.Falling && isGrounded)
        {
            ChangeJumpState(JumpState.Grounded);
        }
        //점프 중(공중)지만 땅에 닿지 않았을 때 (점프 안했는데 떨어짐)
        else if (_state == JumpState.Grounded && !isGrounded)
        {
            ChangeJumpState(JumpState.Falling);
        }
    }

    /// <summary>
    /// 점프 실행 함수
    /// </summary>
    public void Jump()
    {
        //땅에 있을 때, 사다리 타고 있을 때 제외 점프 불가능
        if (_state != JumpState.Grounded &&  _state != JumpState.Climbing) return;

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

        switch (state)
        {
            case JumpState.Grounded:
                Debug.Log("착지");
                break;

            case JumpState.Jumping:
                Debug.Log("점프");
                break;

            case JumpState.Falling:
                Debug.Log("추락");
                break;

            case JumpState.Climbing:
                Debug.Log("사다리");
                break;
        }

        _state = state;
        OnJumpStateChanged(_state);
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
