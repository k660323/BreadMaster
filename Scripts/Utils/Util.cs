using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static Vector3 BezierCurves(Vector3 vec1, Vector3 vec2, Vector3 vec3, float t)
    {
        t = Mathf.Clamp(t, 0.0f, 1.0f);

        Vector3 A = Vector3.Lerp(vec1, vec2, t);
        Vector3 B = Vector3.Lerp(vec2, vec3, t);

        Vector3 C = Vector3.Lerp(A, B, t);

        return C;
    }

    public static Vector3 BezierCurves(Vector3 vec1, float vec2yOffset, Vector3 vec4, float vec3yOffset, float t)
    {
        Vector3 vec2 = new Vector3(vec1.x, vec1.y + vec2yOffset, vec1.z);
        Vector3 vec3 = new Vector3(vec4.x, vec4.y + vec3yOffset, vec4.z);

        return BezierCurves(vec1, vec2, vec3, vec4, t);
    }

    public static Vector3 BezierCurves(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4, float t)
    {
        t = Mathf.Clamp(t, 0.0f, 1.0f);

        Vector3 A = Vector3.Lerp(vec1, vec2, t);
        Vector3 B = Vector3.Lerp(vec2, vec3, t);
        Vector3 C = Vector3.Lerp(vec3, vec4, t);

        Vector3 D = Vector3.Lerp(A, B, t);
        Vector3 E = Vector3.Lerp(B, C, t);

        Vector3 F = Vector3.Lerp(D, E, t);

        return F;
    }
}
