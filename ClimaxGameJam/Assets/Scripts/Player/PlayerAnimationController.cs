using System.Collections;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private AnimatedSprite _animatedSprite;

    private bool _animationPlaying = false;

    public IEnumerator Play(string animation)
    {
        if (_animationPlaying) yield break;
        _animationPlaying = true;
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        _animatedSprite.Play(animation);
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        _animatedSprite.PlayDefaultAnimation();
        _animationPlaying = false;
    }
}