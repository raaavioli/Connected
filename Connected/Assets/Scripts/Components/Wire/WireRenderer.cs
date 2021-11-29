using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WireRenderer : MonoBehaviour {
    // --- Public properties ---

    public Vector3[] points { get; set; } = new Vector3[0];
    public int connected {
        set {
            mpb.SetInt("_Connected", value);
            meshRenderer.SetPropertyBlock(mpb, 0);
        }
    }
    public Color startColor {
        set {
            mpb.SetColor("_StartColor", value);
            meshRenderer.SetPropertyBlock(mpb, 0);
        }
    }
    public Color endColor {
        set {
            mpb.SetColor("_EndColor", value);
            meshRenderer.SetPropertyBlock(mpb, 0);
        }
    }

    // --- Serialized fields ---

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
    private MeshRenderer meshRenderer;

    // Mesh fields.
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    // Other fields.
    private Vector3[] tangents;
    private MaterialPropertyBlock mpb;

    private void Awake() {
        // Initialize mesh.
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();

        mpb = new MaterialPropertyBlock();
    }

    private void Update() {
        if (points.Length > 0) {
            UpdateWire();
		}
    }

    public float GetRadius() {
        return radius;
	}

    // Performs all the actions required for updating the wire.
    private void UpdateWire() {
        // Pre-requisites for the mesh update.
        UpdateTangents();

        // Update the data of the mesh.
        GenerateVertices();
        GenerateTriangles();

        // Update the visuals of the mesh.
        UpdateMesh();
    }

	private void OnDrawGizmosSelected() {
        if (!drawGizmos || vertices == null) {
            return;
		}

        // Draw cubes for the points along the line.
        foreach (Vector3 point in points) {
            Gizmos.DrawCube(transform.TransformPoint(point), Vector3.one * 0.2f);
		}

        // Draw cyan spheres for the vertices.
        for (int i = 0; i < vertices.Length; ++i) {
            Gizmos.color = Color.cyan + Color.white * 0.7f;
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
        }
    }

    // Generates the vertices of the mesh and populates the vertex array.
	private void GenerateVertices() {
        // Initialize vertex array and calculate angle between vertices.
        vertices = new Vector3[points.Length * (radialResolution + 1)];
        float rotationAngle = 360.0f / radialResolution;

        // For aach point along the wire, place a number of vertices as the corners of a polygon around the point.
        for (int i = 0; i < points.Length; ++i) {
            // Calculate the axis around which the vertices will be placed.
            Vector3 averagedTangent = Vector3.zero;
            if (i > 0 && i < points.Length - 1) {
                // If the point is not an end point, let the axis be the normalized sum of the previous point's tangent and the own tangent.
                averagedTangent = Vector3.Normalize(tangents[Mathf.Clamp(i - 1, 0, tangents.Length - 1)] + tangents[Mathf.Clamp(i, 0, tangents.Length - 1)]);
            } else {
                // If the point is an endpoint, use its precalculated tangent.
                averagedTangent = tangents[i];
            }
            // Find the vector with which to offset the vertices from the point.
            // The vector is found using the cross product between the tangent and an arbitrary, non-parallel vector, for the direction and the radius for the magnitude.
            Vector3 orthogonalOffset = Vector3.Cross(averagedTangent, ParallelVectors(averagedTangent, Vector3.one) ? Vector3.up : Vector3.one).normalized * radius;
            for (int j = 0; j <= radialResolution; ++j) {
                // For each vertex, rotate the offset around the tangent and place the vertex at the sum of the point and the offset.
                vertices[i * (radialResolution + 1) + j] = points[i] + Quaternion.AngleAxis(rotationAngle * j, averagedTangent) * orthogonalOffset;
			}
		}
	}

    // Populates the triangle array appropriately.
    private void GenerateTriangles() {
        // The number of triangles is twice the number of vertices for all except the final set of vertices.
        // This is because each vertex except in the final set of vertices is associated with a single quad.
        triangles = new int[3 * 2 * (points.Length - 1) * radialResolution];
        int triangleIndex = 0;

        // For each point along the wire, go through each of the vertices around the point.
        for (int i = 0; i < points.Length - 1; ++i) {
            // For each vertex in the ring, create the two triangles of the associated quad.
            for (int j = 0; j < radialResolution; ++j) {
                // Calculate the special term that is able to handle the last vertex in the ring.
                // If the current vertex is the last vertex, we need to access the first vertex again, so the index offset is 0.
                //int loopingIndex = j == radialResolution - 1 ? 0 : j + 1;
                
                // The first quad.
                triangles[triangleIndex++] = i * (radialResolution + 1) + j;
                triangles[triangleIndex++] = i * (radialResolution + 1) + j + 1;
                triangles[triangleIndex++] = (i + 1) * (radialResolution + 1) + j + 1;

                // The second quad.
                triangles[triangleIndex++] = i * (radialResolution + 1) + j;
                triangles[triangleIndex++] = (i + 1) * (radialResolution + 1) + j + 1;
                triangles[triangleIndex++] = (i + 1) * (radialResolution + 1) + j;
            }
		}
	}

    // Calculates tangent vectors along the wire's curve at each point. The last tangent is equal to the next to last tangent.
    private void UpdateTangents() {
        tangents = new Vector3[points.Length];
        float[] lengths = new float[Mathf.Clamp(points.Length - 1, 0, int.MaxValue)];
        float fullLength = 0.0f;

        // Each tangent is the normalized direction vector from the current point to the next.
        // The lengths of the segments and the full length of the wire are calculated.
        for (int i = 0; i < points.Length - 1; ++i) {
            Vector3 differenceVector = points[i + 1] - points[i];
            float length = differenceVector.magnitude;
            lengths[i] = length;
            fullLength += length;
            tangents[i] = Vector3.Normalize(differenceVector);
		}

        // The final tangent is equal to the next to last tangent.
        if (tangents.Length > 1) {
            tangents[tangents.Length - 1] = tangents[tangents.Length - 2];
        }

        // Calculate the uv coordinates based on how far along the wire it has gone.
        // The u coordinate increases linearly along the wire.
        uv = new Vector2[points.Length * (radialResolution + 1)];
        float accumulatedRelativeLength = 0.0f;

        SetUvForRing(0, uv, 0.0f);

        for (int i = 0; i < lengths.Length; ++i) {
            accumulatedRelativeLength += Mathf.Clamp(lengths[i] / fullLength, 0.0f, 1.0f);
            SetUvForRing(i + 1, uv, accumulatedRelativeLength);
		}
	}

    private void SetUvForRing(int ringIndex, Vector2[] uv, float u) {
        for (int i = 0; i <= radialResolution; ++i) {
            uv[ringIndex * (radialResolution + 1) + i] = new Vector2(u, (float) i / radialResolution);
		}
	}

    // Updates the visual mesh with the current mesh data.
    private void UpdateMesh() {
        mesh.Clear();
        mesh.name = "WireMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
	}

    // Returns true if the input vectors are parallel.
    private bool ParallelVectors(Vector3 a, Vector3 b) {
        return a.normalized == b.normalized;
	}
}
