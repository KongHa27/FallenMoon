using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    [SerializeField] protected float _speed;
    public float Speed => _speed;

    Rigidbody2D _rigid;

    private void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector3 dir)
    {
        Vector2 velo = _rigid.velocity;
        velo.x = dir.x * _speed;

        _rigid.velocity = velo;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
