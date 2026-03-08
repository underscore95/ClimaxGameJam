using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class GameSound
{
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.2f;

    public IEnumerator Play(Transform transform)
    {
        Debug.Assert(_clips.Length > 0);
        GameObject obj = new GameObject("OneShotAudio");
        obj.transform.position = transform.position;

        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = _clips[Random.Range(0, _clips.Length)];
        audioSource.pitch = Random.Range(_minPitch, _maxPitch);
        audioSource.loop = false;
        audioSource.Play();

        yield return DestroyAfter(audioSource);
    }

    private IEnumerator DestroyAfter(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length / source.pitch);
        Object.Destroy(source.gameObject);
    }
}