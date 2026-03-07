using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _camera;

    private void Awake()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        transform.forward = Vector3.Normalize(transform.position - _camera.position);
    }
}