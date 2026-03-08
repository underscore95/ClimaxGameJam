using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

// Animated pre-rendered sprite
public class AnimatedSprite : MonoBehaviour
{
    [Serializable]
    public struct Animation
    {
        public string Name;
        public SpriteAtlas Atlas;
        public int NumFrames;
        public int NumRotations;
        public float FPS;
    }

    [SerializeField] private SpriteRenderer _quad;
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private Animation[] _animations;
    [SerializeField] private string _defaultAnimation;
    private Sprite[] _sprites;
    private int _rotation;
    private float _secondsPerFrame;
    private int _frame;
    private float _anglePerRotation;
    private Transform _camera;
    private int _animation;
    private Coroutine _updateFrameCoroutine;
    private float _timestampOfLastFrameChange;

    private void Awake()
    {
        Debug.Assert(_animations.Length > 0);
        HashSet<string> foundNames = new();
        foreach (Animation animation in _animations)
        {
            Debug.Assert(animation.NumRotations >= 1);
            Debug.Assert(animation.NumFrames >= 1);
            Debug.Assert(animation.FPS > 0);
            Debug.Assert(animation.Atlas);
            Debug.Assert(animation.Name != null);
            Debug.Assert(animation.Name != "");
            Debug.Assert(!foundNames.Contains(animation.Name));
            foundNames.Add(animation.Name);
        }

        _camera = Camera.main.transform;

        PlayDefaultAnimation();
    }

    // Return the number of seconds until the next loop
    public float SecondsUntilLoop()
    {
        float secondsSinceLastFrameChange = Time.time - _timestampOfLastFrameChange;
        float secondsOfRemainingFrames = (_animations[_animation].NumFrames - (_frame + 1)) * _secondsPerFrame;
        return secondsOfRemainingFrames + (_secondsPerFrame - secondsSinceLastFrameChange);
    }

    public void PlayDefaultAnimation()
    {
        Play(_defaultAnimation);
    }

    public void Play(string animation)
    {
        // if (transform.parent.name.Contains("Wizard")) print("playing: " + animation + " (unity frame " + Time.frameCount + ")");
        bool found = false;
        for (_animation = 0; _animation < _animations.Length; _animation++)
        {
            if (_animations[_animation].Name == animation)
            {
                found = true;
                break;
            }
        }

        Debug.Assert(found, gameObject.name + " does not have animation " + animation);
        if (_updateFrameCoroutine != null)
        {
            StopCoroutine(_updateFrameCoroutine);
        }

        _secondsPerFrame = 1.0f / _animations[_animation].FPS;
        _sprites = new Sprite[_animations[_animation].Atlas.spriteCount];
        _animations[_animation].Atlas.GetSprites(_sprites);
        _anglePerRotation = 360.0f / _animations[_animation].NumRotations;
        _frame = 0;

        // Sort numerically
        Array.Sort(_sprites, (a, b) =>
        {
            if (a == b) return 0;

            // nulls sort after anything else
            if (a == null) return 1;
            if (b == null) return -1;

            // Convert to int, ignore all non-numeric
            int aNum = int.Parse(new string(a.name.Where(char.IsDigit).ToArray()));
            int bNum = int.Parse(new string(b.name.Where(char.IsDigit).ToArray()));
            return aNum.CompareTo(bNum);
        });

        _updateFrameCoroutine = StartCoroutine(UpdateFrame());
        _timestampOfLastFrameChange = Time.time;
    }

    private void Update()
    {
        UpdateRotation();

        _quad.sprite = _sprites[_animations[_animation].NumFrames * (int)_rotation + _frame];
    }

    private IEnumerator UpdateFrame()
    {
        yield return new WaitForSeconds(_secondsPerFrame);
        _frame = (_frame + 1) % _animations[_animation].NumFrames;
        _timestampOfLastFrameChange = Time.time;
        _updateFrameCoroutine = StartCoroutine(UpdateFrame());
    }

    private void UpdateRotation()
    {
        float angle = Vector3.SignedAngle(
            _parentTransform.forward,
            _camera.position - _parentTransform.position,
            Vector3.up
        );

        angle -= 180; // needs to be flipped
        angle += _anglePerRotation / 2; // so we transition halfway through the rotation
        angle = (angle + 360) % 360;

        _rotation = Mathf.RoundToInt(angle / _anglePerRotation) % _animations[_animation].NumRotations;
    }
}