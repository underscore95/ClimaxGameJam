using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

public class AnimatedSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _quad;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private SpriteAtlas _atlas;
    [SerializeField] private int _numFrames;
    [SerializeField] private float _framesPerSecond;
    [SerializeField] private int _numRotations;
    private Sprite[] _sprites;
    private int _rotation;
    private float _secondsPerFrame;
    private int _frame;
    private float _anglePerRotation;

    private void Awake()
    {
        Debug.Assert(_framesPerSecond > 0);
        Debug.Assert(_numRotations >= 4);
        Debug.Assert(_numRotations % 4 == 0, "Num rotations must be a multiple of 4");
        _secondsPerFrame = 1.0f / _framesPerSecond;
        _sprites = new Sprite[_atlas.spriteCount];
        _atlas.GetSprites(_sprites);
        _anglePerRotation = 360.0f /_numRotations;

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
        
        StartCoroutine(UpdateFrame());
    }

    private void Update()
    {
        MakeBillboarded();
        UpdateRotation();

        _quad.sprite = _sprites[_numFrames * (int)_rotation + _frame];
        
      //  print("rotation: " + _rotation + " frame: " + _frame + " sprite: " + _quad.sprite.name);
    }

    private IEnumerator UpdateFrame()
    {
        yield return new WaitForSeconds(_secondsPerFrame);
        _frame = (_frame + 1) % _numFrames;
        StartCoroutine(UpdateFrame());
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

        _rotation = Mathf.RoundToInt(angle / _anglePerRotation) % _numRotations;
    }

    private void MakeBillboarded()
    {
        transform.forward = Vector3.Normalize(transform.position - _camera.position);
    }
}