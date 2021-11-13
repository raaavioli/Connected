using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WireRenderer : MonoBehaviour {
    // --- Serialized fields ---

    // Positions.
    [SerializeField]
    private Vector3[] points;
    [SerializeField]
    [Range(3, 20)]
    private int radialResolution = 8;
    [SerializeField]
    [Range(0.01f, 20.0f)]
    private float radius = 1.0f;
    [SerializeField]
    private bool drawGizmos = false;

    // --- Private fields ---

    // Components.
    private MeshFilter meshFilter;

    // Mesh fields.
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    // Other fields.
    private Vector3[] tangents;

	private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        UpdateWire();
	}
    
    private void Update() {
        UpdateWire();
        OnDrawGizmosSelected();
    }

    private void UpdateWire() {
        UpdateTangents();

        GenerateVertices();
        GenerateTriangles();

        UpdateMesh();
    }

	private void OnDrawGizmosSelected() {
        if (!drawGizmos) {
            return;
		}

        foreach (Vector3 point in points) {
            Gizmos.DrawCube(point, Vector3.one * 0.2f);
		}

        float redStep = 1.0f / radialResolution;
        for (int i = 0; i < vertices.Length; ++i) {
            Gizmos.color = new Color(i % 8 * redStep, 0.0f, 0.0f);
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

	private void GenerateVertices() {
        vertices = new Vector3[points.Length * radialResolution];
        float rotationAngle = 360.0f / radialResolution;

        for (int i = 0; i < points.Length; ++i) {
            Vector3 point = points[i];
            Vector3 averagedTangent = Vector3.zero;
            if (i > 0 && i < points.Length - 1) {
                averagedTangent = Vector3.Normalize(tangents[Mathf.Clamp(i - 1, 0, tangents.Length - 1)] + tangents[Mathf.Clamp(i, 0, tangents.Length - 1)]);
            } else {
                averagedTangent = tangents[i];
            }
            Vector3 orthogonalOffset = Vector3.Cross(averagedTangent, ParallellVectors(averagedTangent, Vector3.one) ? Vector3.up : Vector3.one).normalized * radius;
            for (int j = 0; j < radialResolution; ++j) {
                vertices[i * radialResolution + j] = point + Quaternion.AngleAxis(rotationAngle * j, averagedTangent) * orthogonalOffset;
			}
		}
	}

    private void GenerateTriangles() {
        triangles = new int[3 * 2 * (points.Length -1 ) * radialResolution];
        int triangleIndex = 0;

        for (int i = 0; i < points.Length - 1; ++i) {
            for (int j = 0; j < radialResolution; ++j) {
                int loopingIndex = j == radialResolution - 1 ? 0 : j + 1;
                
                triangles[triangleIndex++] = i * radialResolution + j;
                triangles[triangleIndex++] = i * radialResolution + loopingIndex;
                triangles[triangleIndex++] = (i + 1) * radialResolution + loopingIndex;

                triangles[triangleIndex++] = i * radialResolution + j;
                triangles[triangleIndex++] = (i + 1) * radialResolution + loopingIndex;
                triangles[triangleIndex++] = (i + 1) * radialResolution + j;
            }
		}
	}

    // Calculates tangent vectors along the wire's curve at each point. The last tangent is equal to the next to last tangent.
    private void UpdateTangents() {
        tangents = new Vector3[points.Length];
        
        for (int i = 0; i < points.Length - 1; ++i) {
            tangents[i] = Vector3.Normalize(points[i + 1] - points[i]);
		}

        if (tangents.Length > 1) {
            tangents[tangents.Length - 1] = tangents[tangents.Length - 2];
        }
	}

    private void UpdateMesh() {
        mesh.Clear();
        mesh.name = "WireMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
	}

    private bool ParallellVectors(Vector3 a, Vector3 b) {
        return a.normalized == b.normalized;
	}
}
