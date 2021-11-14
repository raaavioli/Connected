using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(WireRenderer))]
public class WirePhysics : MonoBehaviour {
    
    private class Point {
        public Vector3 position, prevPosition;
        public bool locked;

		public Point(Vector3 position, Vector3 prevPosition, bool locked) {
			this.position = position;
			this.prevPosition = prevPosition;
			this.locked = locked;
		}
	}

    private class Stick {
        public Point pointA, pointB;

		public Stick(Point pointA, Point pointB) {
			this.pointA = pointA;
			this.pointB = pointB;
		}
	}

	// --- Serialized fields ---
	[SerializeField]
	private Transform startPoint;
	[SerializeField]
	private Transform endPoint;
	[SerializeField]
	private float minimumY;
	[SerializeField]
	[Range(0, 100)]
	private int numMiddlePoints;
	[SerializeField]
	[Range(10, 100)]
	private int numIterations;
	[SerializeField]
	[Range(1.0f, 2.0f)]
	private float stickExtensionModifier;
	[SerializeField]
	private bool drawGizmos;

	// --- Private fields ---
	private WireRenderer wireRenderer;
	private Point[] points;
	private Stick[] sticks;
	private float stickLength;

	private void Awake() {
		wireRenderer = GetComponent<WireRenderer>();
		InitializeWire();
	}

	private void Update() {
		Simulate();
		SetRendererPoints();
	}

	private void OnDrawGizmosSelected() {
		if (!drawGizmos || points == null) {
			return;
		}

		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(startPoint.position, 0.2f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(endPoint.position, 0.2f);

		foreach (Point point in points) {
			Gizmos.color = Color.green;
			Gizmos.DrawCube(point.position, Vector3.one * 0.3f);
		}
	}

	// Creates the point and stick arrays, and places the points between the sticks.
	private void InitializeWire() {
		points = new Point[numMiddlePoints + 2];
		points[0] = new Point(startPoint.position, startPoint.position, true);
		points[points.Length - 1] = new Point(endPoint.position, endPoint.position, false);

		Vector3 direction = UpdateStickLength();

		for (int i = 0; i < numMiddlePoints; ++i) {
			Vector3 position = startPoint.position + stickLength * (i + 1) * direction;
			points[i + 1] = new Point(position, position, false);
		}

		sticks = new Stick[points.Length - 1];

		for (int i = 0; i < points.Length - 1; ++i) {
			sticks[i] = new Stick(points[i], points[i + 1]);
		}
	}

	// Updates the stick length according to the positions of the start point and end point. Returns direction from start to end.
	private Vector3 UpdateStickLength() {
		Vector3 startToEnd = endPoint.position - startPoint.position;
		Vector3 direction = Vector3.Normalize(startToEnd);
		stickLength = startToEnd.magnitude / (numMiddlePoints + 1) * stickExtensionModifier;

		return direction;
	}

	// Performs simulation, updating points and sticks.
	private void Simulate() {
		points[0].position = startPoint.position;
		points[points.Length - 1].position = endPoint.position;

		UpdateStickLength();

		foreach (Point p in points) {
			if (!p.locked) {
				Vector3 positionBeforeUpdate = p.position;
				p.position += p.position - p.prevPosition;
				p.position += Physics.gravity * Time.deltaTime * Time.deltaTime;
				p.prevPosition = positionBeforeUpdate;
			}
		}

		for (int i = 0; i < numIterations; ++i) {
			foreach (Stick stick in sticks) {
				Vector3 stickCenter = (stick.pointA.position + stick.pointB.position) / 2;
				Vector3 stickDirection = (stick.pointA.position - stick.pointB.position).normalized;
				
				if (!stick.pointA.locked) {
					stick.pointA.position = stickCenter + stickDirection * stickLength / 2;
					stick.pointA.position = new Vector3(stick.pointA.position.x, Mathf.Clamp(stick.pointA.position.y, minimumY + wireRenderer.GetRadius(), float.PositiveInfinity), stick.pointA.position.z);
				}

				if (!stick.pointB.locked) {
					stick.pointB.position = stickCenter - stickDirection * stickLength / 2;
					stick.pointB.position = new Vector3(stick.pointB.position.x, Mathf.Clamp(stick.pointB.position.y, minimumY + wireRenderer.GetRadius(), float.PositiveInfinity), stick.pointB.position.z);
				}
			}
		}
	}

	// Sends the points to the wire renderer for rendering.
	private void SetRendererPoints() {
		Vector3[] rawPoints = new Vector3[points.Length];

		for (int i = 0; i < points.Length; ++i) {
			rawPoints[i] = points[i].position;
		}

		wireRenderer.points = rawPoints;
	}
}
