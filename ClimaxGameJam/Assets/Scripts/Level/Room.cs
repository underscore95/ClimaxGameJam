using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject _door;

    public int EnemiesAlive { get; internal set; }
    private List<Vector3> _spawnPositions = new();

    private void Awake()
    {
        foreach (Spawner spawner in GetComponentsInChildren<Spawner>())
        {
            EnemiesAlive++;
            _spawnPositions.Add(spawner.transform.position);
            Destroy(spawner);
        }

        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<MeshCollider>().sharedMesh =
                child.gameObject.GetComponent<MeshFilter>().sharedMesh;
        }
    }
}