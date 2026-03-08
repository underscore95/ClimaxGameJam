using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasUtils
{
    public static IEnumerator FadeIn(CanvasGroup group, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = t / duration;
            yield return null;
        }

        group.alpha = 1f;
    }
}