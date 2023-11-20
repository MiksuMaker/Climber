using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector3 CalcBezierPos(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        //if (points.Count != 4) { Debug.Log("Not enough points for Bezier"); return Vector3.zero; }

        // Debug
        Debug.DrawLine(a, b, Color.cyan);
        Debug.DrawLine(b, c, Color.cyan);
        Debug.DrawLine(c, d, Color.cyan);

        // Lerp
        Vector3 PtX = (1 - t) * a + t * b;
        Vector3 PtY = (1 - t) * b + t * c;
        Vector3 PtZ = (1 - t) * c + t * d;

        Vector3 r_point = (1 - t) * PtX + t * PtY;
        Vector3 s_point = (1 - t) * PtY + t * PtZ;

        Vector3 bezier_point = (1 - t) * r_point + t * s_point;
        Debug.DrawRay(bezier_point, Vector3.up * 0.2f, Color.red, 0.1f);
        Debug.DrawRay(bezier_point, Vector3.right * 0.2f, Color.red, 0.1f);

        return bezier_point;
    }

    public static Vector3 CalcBezierPos(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        //if (points.Count != 4) { Debug.Log("Not enough points for Bezier"); return Vector3.zero; }

        // Debug
        Debug.DrawLine(a, b, Color.cyan);
        Debug.DrawLine(b, c, Color.cyan);

        // Lerp
        Vector3 PtX = (1 - t) * a + t * b;
        Vector3 PtY = (1 - t) * b + t * c;

        Vector3 bezier_point = (1 - t) * PtX + t * PtY;
        Debug.DrawRay(bezier_point, Vector3.up * 0.2f, Color.red, 0.1f);
        Debug.DrawRay(bezier_point, Vector3.right * 0.2f, Color.red, 0.1f);

        return bezier_point;
    }
}
