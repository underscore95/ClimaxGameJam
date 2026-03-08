using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameObjectUtils
{
    static GameObject PromoteToRoot(GameObject obj)
    {
        var t = obj.transform;
        while (t.parent)
            t = t.parent;
        t.SetParent(null);
        return t.gameObject;
    }

    public static IEnumerator DestroyAllExcept(params GameObject[] keepObjects)
    {
        var keep = new HashSet<GameObject>();

        foreach (var obj in keepObjects)
            keep.Add(PromoteToRoot(obj));

        yield return new WaitForEndOfFrame();

        var roots = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var obj in roots)
        {
            if (!keep.Contains(obj))
                Object.Destroy(obj);
        }
    }
}