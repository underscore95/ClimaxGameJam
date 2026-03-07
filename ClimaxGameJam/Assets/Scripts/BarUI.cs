using System;
using UnityEngine;

public class BarUI : MonoBehaviour
{
    [SerializeField] private RectTransform _progress;
    private float _maxWidth;

    private void Awake()
    {
        _maxWidth = _progress.sizeDelta.x;
    }

    // Set progress in 0 to 1 range
    public void SetProgress(float progress)
    {
        Debug.Assert(progress >= 0);
        Debug.Assert(progress <= 1);

        Vector2 size = _progress.sizeDelta;
        size.x = _maxWidth * progress;
        _progress.sizeDelta = size;
    }
}