using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arcs
{
    public class ArcScript : MonoBehaviour
    {
            // Arc calcuations
    public static List<Vector2> GetArcPoints(float angleInDegrees, int trianglesPerRad, float width, bool reverse)
    {
            var triangleCount = GetTriangleCount(trianglesPerRad);
            float sectorAngle = Mathf.Deg2Rad * angleInDegrees;
            var vertices = new List<Vector2>();
            //vertices.Add(Vector2.zero);
            if (reverse)
            {
                for (int i = triangleCount + 1; i >= 0; i--)
                {
                    float theta = i / (float)triangleCount * sectorAngle;
                    var vertex = -(new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * width) * 2;
                    vertices.Add(new Vector2(vertex.x, vertex.z));
                }
            }
            else
            {
                for (int i = 0; i <= triangleCount + 1; i++)
                {
                    float theta = i / (float)triangleCount * sectorAngle;
                    var vertex = (new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * width) * 2;
                    vertices.Add(new Vector2(vertex.x, vertex.z));
                }
            }

            //vertices.Add();
            return vertices;
        }

    // Get how many triangles the arc will have
    static private int GetTriangleCount(int trianglesPerRad)
    {
        return Mathf.CeilToInt(12 * Mathf.PI * trianglesPerRad);
    }
    }
}

