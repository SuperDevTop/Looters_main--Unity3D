using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;


public static partial class Extensions {
    public static Vector4 ToVector(this Color self) {
        return new Vector4(self.r, self.g, self.b, self.a);
    }

    public static bool AngleApproxEquals(this Mathf m, float angleA, float angleB, float error) {
        return angleA % 360 < (angleB + error) % 360 && angleA % 360 > (angleB - error) % 360;
    }

    public static bool VectorEquals(this Vector3 vec, Vector3 other) {
        return Mathf.Approximately(vec.x, other.x) && Mathf.Approximately(vec.y, other.y) &&
               Mathf.Approximately(vec.z, other.z);
    }

    public static bool VectorEquals(this Vector2 vec, Vector2 other) {
        return Mathf.Approximately(vec.x, other.x) && Mathf.Approximately(vec.y, other.y);
    }

    public static void SetAlpha(this Color c, float alpha) {
        c.a = Mathf.Clamp01(alpha);
    }

    public static int IndexOf<T>(this T[] array, T element) where T : IComparable {
        for (int i = 0; i < array.Length; i++) {
            if (array[i].CompareTo(element) == 0)
                return i;
        }

        return -1;
    }

    public static int IndexOf<T>(this List<T> list, Predicate<T> match) {
        for (int i = 0; i < list.Count; i++) {
            if (match(list[i]))
                return i;
        }

        return -1;
    }

    public static void SwapAt<T>(this IList<T> list, int i, int j) {
        T temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    public static void ForEach<T>(this List<T> list, System.Action<T, int> action) {
        for (int i = 0; i < list.Count; i++) {
            action?.Invoke(list[i], i);
        }
    }

    public static void Shuffle<T>(this IList<T> list) {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public static T GetRandom<T>(this IList<T> list) {
        if (list.Count == 0)
            return default;
        return list[Random.Range(0, list.Count)];
    }

    public static void SetLayerRecursively(this Transform transform, int layer) {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();
        foreach (var t in transforms) {
            t.gameObject.layer = layer;
        }
    }
}