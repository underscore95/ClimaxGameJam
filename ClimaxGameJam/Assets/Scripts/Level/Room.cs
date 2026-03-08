using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int EnemiesAlive { get; internal set; }

    private void Awake()
    {
        EnemiesAlive = GetComponentsInChildren<WizardController>().Length;
        
        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<MeshCollider>().sharedMesh =
                child.gameObject.GetComponent<MeshFilter>().sharedMesh;
        }
    }
}