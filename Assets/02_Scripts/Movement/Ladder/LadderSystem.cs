using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderSystem : MonoBehaviour
{
    [Header("----- 사다리 설정 -----")]
    [SerializeField] Transform _topPoint;           //사다리 상단 지점
    [SerializeField] Transform _bottomPoint;        //사다리 하단 지점
    [SerializeField] bool _canExitAnyWhere = true;  //사다리 어느 지점에서든지 나갈 수 있는지 여부
    [SerializeField] LayerMask _ladderUserLayerMask = -1;

    List<ILadderUser> _ladderUsers = new();

    public Vector3 TopPoint => _topPoint.position;
    public Vector3 BottomPoint => _bottomPoint.position;
    public bool CanExitAnyWhere => _canExitAnyWhere;
    public List<ILadderUser> LadderUsers => _ladderUsers;


    private void Awake()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //레이어 마스크 확인
        if (((1 << collision.gameObject.layer) & _ladderUserLayerMask)  == 0)
            return;

        ILadderUser user = collision.gameObject.GetComponent<ILadderUser>();
        if (user != null && user.CanUseLadder && !_ladderUsers.Contains(user))
        {
            _ladderUsers.Add(user);
            user.OnLadderStateChanged(true, this);

            //디버깅용
            Debug.Log($"{collision.gameObject.name} 사다리 입장");
        }

        //collision이 Hero였을 때
        if (collision.gameObject.GetComponent<Hero>() != null)
        {
            Jumper jumper = collision.gameObject.GetComponent<Jumper>();
            if (jumper != null)
                jumper.IsOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //레이어 마스크 확인
        if (((1 << collision.gameObject.layer) & _ladderUserLayerMask) == 0)
            return;

        ILadderUser user = collision.gameObject.GetComponent<ILadderUser>();
        if (user != null && _ladderUsers.Contains(user))
        {
            _ladderUsers.Remove(user);
            user.OnLadderStateChanged(false, this);

            //디버깅용
            Debug.Log($"{collision.gameObject.name} 사다리 퇴장");
        }

        //collision이 Hero였을 때
        if (collision.gameObject.GetComponent<Hero>() != null)
        {
            Jumper jumper = collision.gameObject.GetComponent<Jumper>();
            if (jumper != null)
                jumper.IsOnLadder = false;
        }
    }

    /// <summary>
    /// 사다리 특정 지점에서 타기가 가능한지 확인
    /// ex. 사다리 중간
    /// </summary>
    /// <param name="pos">사다리를 타려는 특정 지점</param>
    /// <returns></returns>
    public bool CanClimbAt(Vector3 pos)
    {
        //사다리 범위 확인
        float minY = Mathf.Min(_topPoint.position.y, _bottomPoint.position.y);
        float maxY = Mathf.Max(_topPoint.position.y, _bottomPoint.position.y);

        //특정 지점이 사다리 범위 내에 있는지 확인
        return pos.y >= minY && pos.y <= maxY;
    }

    /// <summary>
    /// 사다리에서 가장 가까운 지점 찾기
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 GetClosestPointOnLadder(Vector3 pos)
    {
        Vector3 ladderDir = (_topPoint.position - _bottomPoint.position).normalized;
        Vector3 toPos = pos - _bottomPoint.position;

        float projection = Vector3.Dot(toPos, ladderDir);
        projection = Mathf.Clamp(projection, 0f, Vector3.Distance(_topPoint.position, _bottomPoint.position));
        
        return _bottomPoint.position + ladderDir * projection;
    }

    /// <summary>
    /// 사다리 상단에 도달했는지 여부
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public bool IsAtTop(Vector3 pos, float threshold = 0.5f)
    {
        return Vector3.Distance(pos, _topPoint.position) < threshold;
    }

    /// <summary>
    /// 사다리 하단에 도달했는지 여부
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public bool IsAtBottom(Vector3 pos, float threshold = 0.5f)
    {
        return Vector3.Distance(pos, _bottomPoint.position) < threshold;
    }

    /// <summary>
    /// 사다리에서 사용자 강제 퇴장
    /// </summary>
    /// <param name="user"></param>
    public void ForceExitLadder(ILadderUser user)
    {
        if ( _ladderUsers.Contains(user))
        {
            _ladderUsers.Remove(user);
            user.ExitLadder();
        }
    }

    /// <summary>
    /// 사다리 중간 위치 계산 (AI용)
    /// </summary>
    /// <param name="progress"></param>
    /// <returns></returns>
    public Vector3 GetPosAtProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        return Vector3.Lerp(_bottomPoint.position, _topPoint.position, progress);
    }





    private void OnDrawGizmosSelected()
    {
        if (_topPoint == null || _bottomPoint == null) return;

        //사다리 라인 그리기
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_topPoint.position, _bottomPoint.position);

        //사다리 상단, 하단 지점 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_topPoint.position, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_bottomPoint.position, 0.3f);
    }
}
