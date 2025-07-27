using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGController : MonoBehaviour
{
    [SerializeField] Transform _camera;
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] float _scrollFactor = 1f;

    void Update()
    {
        Vector2 offset = _renderer.material.mainTextureOffset;

        offset.x = (1 / transform.localScale.x) * _camera.position.x;
        offset.x *= _scrollFactor;

        _renderer.material.mainTextureOffset = offset;
    }
}
