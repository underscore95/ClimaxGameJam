using UnityEngine;

public sealed class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    public GameObject Prefab => _prefab;
}