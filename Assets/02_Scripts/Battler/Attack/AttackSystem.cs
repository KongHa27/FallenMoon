using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
    [SerializeField] protected BattlerModel _model;
    protected Transform _transform;

    public virtual void Initialize(BattlerModel model, Transform transform)
    { 
        _model = model;
        _transform = transform;
    }

    public abstract void PerformAttack(IDamageable target = null);
    public abstract bool CanAttack();
    public abstract float GetAttackRange();
}
