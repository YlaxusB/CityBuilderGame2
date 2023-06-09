using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace StraightRoadMeshMainNamespace
{
    public static class StraightRoadMeshMain
    {
        public static Mesh BuildMeshAlongLocalPoints(List<Vector3> pointsWorld, int roadWidth)
        {
            List<Vector3> pointsLocal = new List<Vector3>();

            pointsLocal.Add(new Vector3(0, pointsWorld[0].y, 0));
            pointsLocal.Add(new Vector3(Vector3.Distance(pointsWorld[0], pointsWorld[1]), pointsWorld[1].y, 0));

            List<Vector3> verts = new List<Vector3>();
            for (int i = 0; i < pointsLocal.Count; i++)
            {
                verts.Add(new Vector3(pointsLocal[i].x, pointsLocal[i].y, roadWidth));
                verts.Add(new Vector3(pointsLocal[i].x, pointsLocal[i].y, -roadWidth));
            }

            // Create the uvs
            List<Vector2> uvs = new List<Vector2>();
            for (int i = 0; i < pointsLocal.Count; i++)
            {
                float completionPercent = 1 / (pointsLocal.Count - 1);
                uvs.Add(new Vector2(0, completionPercent));
                uvs.Add(new Vector2(1, completionPercent));
            }

            // Create the triangles
            List<int> triangles = new List<int>();
            for (int i = 0; i < 2 * (pointsLocal.Count - 1); i += 2)
            {
                triangles.Add(i);
                triangles.Add(i + 2);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + 2);
                triangles.Add(i + 3);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            return mesh;
        }

        //  The circle mesh
        public static Mesh CreatePrePreviewMesh(GameObject road, Vector2 initialPoint, float roadWidth)
        {
            List<Vector3> points = new List<Vector3>();

            // Create the points where
            float steps = roadWidth * 16;
            for (int currentStep = 0; currentStep <= steps; currentStep++)
            {
                float circumferenceProgress = currentStep / steps;

                float currentRadian = circumferenceProgress * 2 * Mathf.PI;

                float zScaled = Mathf.Cos(currentRadian);
                float xScaled = Mathf.Sin(currentRadian);
                points.Add(new Vector3(xScaled * roadWidth, 0, zScaled * roadWidth));
            }

            List<Vector3> verts = new List<Vector3>();
            List<int> triangles = new List<int>();


            // The first vertice is the middle and others are just the circle points
            verts.Add(new Vector3(0, 0, 0));
            for (int i = 0; i < points.Count - 1; i++)
            {
                verts.Add(new Vector3(points[i].x, points[i].y, points[i].z));
            }

            // Make triangles, going from index, to 0, to index + 1 /
            // point - middle point - next point 
            for (int i = 1; i < points.Count - 1; i++)
            {
                triangles.Add(i + 1);
                triangles.Add(0);
                triangles.Add(i);
            }
            triangles.Add(1);
            triangles.Add(0);
            triangles.Add(points.Count - 1);

            List<Vector2> uvs = new List<Vector2>();
            foreach (Vector3 vert in verts)
            {
                uvs.Add(new Vector2(vert.x, vert.z));
            }
            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            return mesh;
        }

        // Build Arc Mesh
        public static Mesh CreateMeshAlongPointsB(List<Vector2> pointsList, float roadWidth, bool reverseNormals)
        {
            // Iterate to get the distance of startPoint and endPoint traveled by the road
            float distance = 0;
            for (int i = pointsList.Count - 1; i > 1; i--)
            {
                distance += Vector3.Distance(pointsList[i], pointsList[i - 1]);
            }

            Vector2[] points = pointsList.ToArray();
            // vertices = 2 * number of points
            // triangles = (2 * (number of points - 1) * 3) 
            Vector3[] verts = new Vector3[points.Length * 2];
            Vector2[] uvs = new Vector2[verts.Length];
            int[] tris = new int[2 * (points.Length - 1) * 3];
            int vertIndex = 0;
            int triIndex = 0;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 forward = Vector2.zero;
                if (i < points.Length - 1)
                {
                    forward += points[i + 1] - points[i];
                }
                if (i > 0)
                {
                    forward += points[i] - points[i - 1];
                }
                forward.Normalize();
                Vector2 left = new Vector2(-forward.y, forward.x);

                //verts[vertIndex] = points[i] + (left * 2) * roadWidth;
                verts[vertIndex] = Vector3.zero;
                verts[vertIndex + 1] = points[i];// - (left / 4) * roadWidth;

                float completionPercent = i / (float)(points.Length - 1);
                uvs[vertIndex] = new Vector2(0, completionPercent);
                uvs[vertIndex + 1] = new Vector2(1, completionPercent);


                if (i < points.Length - 1)
                {
                        tris[triIndex + 3] = vertIndex + 1;
                        tris[triIndex + 4] = vertIndex + 2;
                        tris[triIndex + 5] = vertIndex + 3;


                        tris[triIndex] = vertIndex;
                        tris[triIndex + 1] = vertIndex + 2;
                        tris[triIndex + 2] = vertIndex + 1;

                }

                vertIndex += 2;
                triIndex += 6;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = tris;
            if (reverseNormals)
            {
                mesh.triangles = mesh.triangles.Reverse().ToArray();
            }
            mesh.uv = uvs;
            return mesh;
        }
    }
}


