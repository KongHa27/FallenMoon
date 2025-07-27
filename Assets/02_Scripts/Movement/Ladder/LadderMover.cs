using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LadderMover : MonoBehaviour
{
    [Header("----- 사다리 설정 -----")]
    [SerializeField] float _climbSpeed = 5f;
    [SerializeField] float _gravityScale = 1f;

    /// <summary>
    /// 사다리 오르는 상태 변화 이벤트
    /// </summary>
    public event Action<bool> OnLadderStateChanged;

    Rigidbody2D _rigid;
    bool _isOnLadder = false;
    bool _isClimbing = false;

    public bool IsOnLadder => _isOnLadder;
    public bool IsClimbing => _isClimbing;


    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();

        _gravityScale = _rigid.gravityScale;
    }

    /// <summary>
    /// 사다리 상태 설정
    /// </summary>
    /// <param name="isOnLadder"></param>
    public void SetLadderState(bool isOnLadder)
    {
        //변경하려는 상태가 원래 상태와 같으면 리턴
        if (_isOnLadder == isOnLadder) return;

        //상태 변경
        _isOnLadder = isOnLadder;

        //사다리 상태 변경 이벤트 발행
        OnLadderStateChanged?.Invoke(_isOnLadder);

        //_isOnLadder가 false, 사다리 위에 있지 않으면
        if (!_isOnLadder)
        {
            StopClimbing();
        }
    }

    /// <summary>
    /// 사다리 이동하는 함수
    /// </summary>
    /// <param name="dir">-1: 아래 / 0: 정지 / 1: 위</param>
    public void ClimbLadder(float dir)
    {
        if (!_isOnLadder) return;

        //정지 상태 + 오르고 있는 상태가 아니면
        if (dir !=0 && !_isClimbing)
            // 사다리 오르기 시작
            StartClimbing();
        //정지 상태 + 오르고 있는 상태이면 정지
        else if (dir == 0 && _isClimbing)
            StopClimbing();

        //사다리 오르고 있는 중이면 속도 설정
        if (_isClimbing)
        {
            Vector2 velocity = _rigid.velocity;
            velocity.y = dir * _climbSpeed;
            _rigid.velocity = velocity;
        }
    }

    /// <summary>
    /// 사다리 타기 시작하는 함수
    /// </summary>
    void StartClimbing()
    {
        _isClimbing = true;
        _rigid.gravityScale = 0;
    }

    /// <summary>
    /// 사다리 타는 것을 중단하는 함수
    /// </summary>
    void StopClimbing()
    {
        _isClimbing = false;
        _rigid.gravityScale = _gravityScale;

        //사다리에서 벗어날 때 Y 속도 초기화
        if (!_isOnLadder)
        {
            Vector2 velo = _rigid.velocity;
            velo.y = 0;
            _rigid.velocity = velo;
        }
    }

    /// <summary>
    /// 사다리 타는 속도 설정
    /// </summary>
    /// <param name="speed"></param>
    public void SetClimbSpeed(float speed)
    {
        _climbSpeed = speed;
    }

    /// <summary>
    /// 사다리에서 벗어날 때 실행하는 함수
    /// </summary>
    public void ExitLadder()
    {
        if (_isClimbing)
            StopClimbing();
    }
}
