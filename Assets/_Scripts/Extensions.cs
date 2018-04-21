using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Extensions
{
    public static float Map(this float value, float from1, float to1, float from2, float to2)
    {
        value = Mathf.Clamp(value, Mathf.Min(from1, to1), Mathf.Max(from1, to1));
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /** Rotates the vector clockwise by an angle specified in degrees (fuck radians). */
    public static Vector2 Rotate2D(this Vector2 vec, float degrees)
    {
        degrees *= Mathf.Deg2Rad;

        Vector2 newVector;
        newVector.x = vec.x * Mathf.Cos(degrees) - vec.y * Mathf.Sin(degrees);
        newVector.y = vec.y * Mathf.Cos(degrees) + vec.x * Mathf.Sin(degrees);
        return newVector;
    }

    /** Rotates the vector clockwise by an angle specified in degrees (fuck radians). Maintains initial z value. */
    public static Vector3 Rotate2D(this Vector3 vec, float degrees)
    {
        degrees *= Mathf.Deg2Rad;

        Vector3 newVector;
        newVector.x = vec.x * Mathf.Cos(degrees) - vec.y * Mathf.Sin(degrees);
        newVector.y = vec.y * Mathf.Cos(degrees) + vec.x * Mathf.Sin(degrees);
        newVector.z = vec.z;
        return newVector;
    }

    /** Gets degrees from this vector to other vector, clockwise. */
    public static float AngleTo(this Vector2 vec, Vector2 comp)
    {
        float dot = Vector2.Dot(comp, vec);
        float det = comp.x * vec.y - comp.y * vec.x; // determinant
        float angle = Mathf.Rad2Deg * Mathf.Atan2(det, dot);
        if (angle < 0)
            angle += 360;
        return angle;
    }

    /** Gets degrees from this vector to other vector, clockwise. Ignores z. */
    public static float AngleTo(this Vector3 vec, Vector3 comp)
    {
        float dot = Vector3.Dot(comp, vec);
        float det = comp.x * vec.y - comp.y * vec.x; // determinant
        float angle = Mathf.Rad2Deg * Mathf.Atan2(det, dot);
        if (angle < 0)
            angle += 360;
        return angle;
    }
}