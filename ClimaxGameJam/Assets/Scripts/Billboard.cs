using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private void Update()
    {
        transform.forward = Vector3.Normalize(transform.position - _camera.position);
    }
}