using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사다리를 사용할 수 있는 캐릭터들이 구현해야 하는 인터페이스
/// </summary>
public interface ILadderUser
{
    /// <summary>
    /// 사다리 사용 가능 여부
    /// </summary>
    bool CanUseLadder {  get; }

    /// <summary>
    /// 현재 사다리 위에 있는지 여부
    /// </summary>
    bool IsOnLadder { get; }

    /// <summary>
    /// 현재 사다리를 타고 있는지 여부
    /// </summary>
    bool IsClimbing { get; }

    /// <summary>
    /// 사다리 입장 시 호출
    /// </summary>
    /// <param name="ladderSystem"></param>
    void EnterLadder(LadderSystem ladderSystem); 

    /// <summary>
    /// 사다리에서 나갈 때 호출
    /// </summary>
    /// <param name="ladderSystem"></param>
    void ExitLadder();

    /// <summary>
    /// 사다리 상태 변경 시 호출
    /// </summary>
    /// <param name="v"></param>
    /// <param name="ladderSystem"></param>
    void OnLadderStateChanged(bool v, LadderSystem ladderSystem);
}

[System.Serializable]
public class LadderUserState
{
    public bool isOnLadder = false;
    public bool isClimbing = false;
    public LadderSystem CurLadder = null;
    public float LastEnterTime = 0f;

    /// <summary>
    /// 사다리 사용 상태를 리셋하는 함수
    /// </summary>
    public void Reset()
    {
        isOnLadder = false;
        isClimbing = false;
        CurLadder = null;
        LastEnterTime = 0f;
    }
}
