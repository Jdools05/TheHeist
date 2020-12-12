using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float fov = 90f;
    public float viewDistance = 50f;
    public static int rayCount = 150;
    private Vector3 origin = new Vector3(0, 0, 0);
    private float startingAngle = 0f;

    [SerializeField] private LayerMask layerMask;

    private Mesh mesh;
    void Start()
    {
        mesh = new Mesh
        {
            bounds = new Bounds(Vector3.zero, Vector3.one * 1000f)
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void FixedUpdate()
    {
        mesh.Clear();
        float angleIncrease = fov / rayCount;
        float angle = startingAngle;
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Ray ray = new Ray(origin, GetVectorFromAngle(angle));
            vertices[vertexIndex] = Physics.Raycast(ray, out RaycastHit raycastHit, viewDistance, layerMask)
             ? raycastHit.point - origin 
             : GetVectorFromAngle(angle) * viewDistance;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.x, -dir.z) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) - (90f - fov / 2f);
    }
}
