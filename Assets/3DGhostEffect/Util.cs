using UnityEngine;

public static class Util
{
    public static T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();

        if (comp == null)
            comp = go.AddComponent<T>();

        return comp;
    }
}
