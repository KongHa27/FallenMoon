using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    event Action<float, float> OnHpChanged;

    event Action OnDead;

    void TakeHit(float damage);
}
