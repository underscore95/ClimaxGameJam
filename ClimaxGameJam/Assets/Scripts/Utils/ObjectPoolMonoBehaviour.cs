using UnityEngine;

public class ObjectPoolMonoBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _capacity;

    protected ObjectPool _pool;

    protected void Awake()
    {
        _pool = new(_prefab, _capacity, transform);
    }
    
    public ObjectPool Pool
    {
        get => _pool;
    }
}