using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _player;

    private void Update()
    {
        Vector3 pos = transform.position;

        pos = _player.position;

        pos.y += 2f;
        pos.z -= 10f;

        transform.position = pos;
    }
}
