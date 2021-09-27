using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Avante
{
	public class FulldomeGizmo
	{
		public float horizon = 0;

		float wireStep = 45.0f * Mathf.Deg2Rad;
		float meshStep = 10.0f * Mathf.Deg2Rad;

		//float PI = Mathf.PI;
		float PI2 = Mathf.PI * 2.0f;
		float PI05 = Mathf.PI * 0.5f;

		List<Vector3> _linePoints = new List<Vector3>();
		List<Vector3> _meshPoints = new List<Vector3>();
		List<Vector3> _meshNormals = new List<Vector3>();
		List<int> _meshTriangles = new List<int>();
		Mesh _mesh;

		public FulldomeGizmo(float h)
		{
			if (h == horizon)
				return;
			horizon = h;

			float radius = 1.0f;
			float fovH = PI2;
			float fovVN = HorizonToAltitude(horizon * Mathf.Deg2Rad);
			float fovVP = PI05;

			float fovHN = -PI05 + fovH * -0.5f;
			float fovHP = -PI05 + fovH * 0.5f;

			// WIRE VERTICALS
			for (float lng = fovHN; lng <= fovHP;)
			{
				List<Vector3> ps = new List<Vector3>();
				for (float lat = fovVN; lat <= fovVP;)
				{
					ps.Add(LatLngToDome(lat, lng) * radius);
					lat = NextAngle(lat, fovVN, fovVP, meshStep);
				}
				AddLines(ps);
				lng = NextAngle(lng, fovHN, fovHP, wireStep);
			}

			// WIRE HORIZONTALS
			if (fovVN > -PI05)
			{
				List<Vector3> ps = new List<Vector3>();
				for (float lng = fovHN; lng <= fovHP;)
				{
					ps.Add(LatLngToDome(fovVN, lng) * radius);
					lng = NextAngle(lng, fovHN, fovHP, meshStep);
				}
				AddLines(ps);
			}
			if (fovVN != 0)
			{
				List<Vector3> ps = new List<Vector3>();
				for (float lng = fovHN; lng <= fovHP;)
				{
					ps.Add(LatLngToDome(0, lng) * radius);
					lng = NextAngle(lng, fovHN, fovHP, meshStep);
				}
				AddLines(ps);
			}

			// MESH
			for (float lat = fovVN; lat < fovVP;)
			{
				float lat2 = NextAngle(lat, fovVN, fovVP, meshStep);
				Vector3[] ps = new Vector3[4];
				for (float lng = fovHN; lng < fovHP;)
				{
					float lng2 = NextAngle(lng, fovHN, fovHP, meshStep);
					ps[0] = LatLngToDome(lat, lng) * radius;
					ps[1] = LatLngToDome(lat, lng2) * radius;
					ps[2] = LatLngToDome(lat2, lng2) * radius;
					ps[3] = LatLngToDome(lat2, lng) * radius;
					addMeshQuad(ps);
					lng = lng2;
				}
				lat = lat2;
			}

			_mesh = new Mesh();
			_mesh.vertices = _meshPoints.ToArray();
			_mesh.normals = _meshNormals.ToArray();
			_mesh.triangles = _meshTriangles.ToArray();
		}

		float NextAngle(float a, float from, float to, float step)
		{
			// first
			if (a == from)
			{
				float mod = Mathf.Abs(a % step);
				if (mod < 0.001)
					a += step;
				else if (a < 0)
					a += mod;
				else
					a += (step - mod);
			}
			// last
			else if (a == to)
			{
				a += step;
				return a;
			}
			// in between
			else
				a += step;
			// last one will stay at end
			if (a > to)
				a = to;
			return a;
		}

		void AddLines(List<Vector3> ps)
		{
			for (int i = 0; i < ps.Count - 1; i++)
			{
				if (ps[i] != ps[i + 1])
				{
					_linePoints.Add(ps[i]);
					_linePoints.Add(ps[i + 1]);
				}
			}
		}

		void addMeshQuad(Vector3[] ps)
		{
			int start = _meshPoints.Count;
			for (int i = 0; i < 4; ++i)
			{
				_meshPoints.Add(ps[i]);
				_meshNormals.Add(ps[i].normalized);
			}
			_meshTriangles.Add(start + 0);
			_meshTriangles.Add(start + 1);
			_meshTriangles.Add(start + 2);
			_meshTriangles.Add(start + 0);
			_meshTriangles.Add(start + 2);
			_meshTriangles.Add(start + 3);
		}

		// Draw gizmo!

		public void Draw(Transform transform, bool connected, bool isFulldome, float domeTilt)
		{
			Color savedColor = Gizmos.color;
			Gizmos.color = connected ? Color.yellow : Color.red;

			Matrix4x4 savedMatrix = Gizmos.matrix;
			Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			if (isFulldome)
				matrix *= Matrix4x4.Rotate(Quaternion.Euler(-90+domeTilt, 0, 0));
			Gizmos.matrix = matrix;

			// Draw Wire
			for (int v = 0; v < (int)_linePoints.Count; v += 2)
				Gizmos.DrawLine(_linePoints[v], _linePoints[v + 1]);
			// Draw Mesh
			Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.33f);
			Gizmos.DrawMesh(_mesh);

			Gizmos.color = savedColor;
			Gizmos.matrix = savedMatrix;
		}

		// Math

		float HorizonToAltitude(float h)
		{
			return (PI05 - ((h) * 0.5f));
		}

		Vector3 LatLngToDome(float lat, float lng)
		{
			float r = Mathf.Cos(lat);
			float x = Mathf.Cos(lng) * r;
			float y = Mathf.Sin(lng) * r;
			float z = Mathf.Sin(lat);
			return new Vector3(x, y, z);
		}
	}
}
