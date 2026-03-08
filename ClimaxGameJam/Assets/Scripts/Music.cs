using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioClip[] _tracks;
    [SerializeField] private AudioSource _source;
    private int _index;

    void Awake()
    {
        Debug.Assert(_tracks.Length > 0);
    }

    void Start()
    {
        if (_tracks == null || _tracks.Length == 0) return;
        Play(_index);
    }

    void Update()
    {
        if (!_source.isPlaying && _tracks.Length > 0)
        {
            _index++;
            if (_index >= _tracks.Length) _index = 0;
            Play(_index);
        }
    }

    void Play(int i)
    {
        _source.clip = _tracks[i];
        _source.Play();
    }
}