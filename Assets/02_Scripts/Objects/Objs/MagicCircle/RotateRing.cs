using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRing : MonoBehaviour
{
    [SerializeField] float speed = 30f;
    void Update() => transform.Rotate(Vector3.forward * speed * Time.deltaTime);
}
