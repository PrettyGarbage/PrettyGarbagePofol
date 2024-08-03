using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Hexagon : MonoBehaviour
{
	public enum Orientation
	{
		Pointy,
		Flat,
	}

	[SerializeField] Orientation _orientation = Orientation.Pointy;
	[SerializeField] float _outerRadius = 0.5f;

	public Orientation orientation
	{
		get
		{
			return _orientation;
		}
	}

	public float outerRadius
	{
		get
		{
			return _outerRadius;
		}
	}

	public float innerRadius
	{
		get
		{
			return GetInnerRadius(_outerRadius);
		}
	}

	public float heightOffset
	{
		get
		{
			return GetPointyOffset(innerRadius);
		}
	}

	public Vector2[] GetCorners()
	{
		return GetCorners(_orientation, _outerRadius);
	}

	public bool AddCollier(bool forced = false)
	{
		if (GetComponent<PolygonCollider2D>() != null && !forced)
		{
			return false;
		}

		PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();
		collider.points = GetCorners();

		return true;
	}

	#region UTILITIES
	public static float GetInnerRadius(float outerRadius)
	{
		return Mathf.Sqrt(3.0f) * outerRadius * 0.5f;
	}

	public static float GetPointyOffset(float innerRadius)
	{
		return innerRadius * 1.5f;
	}

	public static Vector2[] GetPointyCorners(float outerRadius)
	{
		Vector2[] points = new Vector2[6];
		for (int i = 0; i < 6; ++i)
		{
			points[i] = Quaternion.Euler(Vector3.forward * 60.0f * i) * (Vector3.up * outerRadius);
		}

		return points;
	}

	public static Vector2[] GetFlatCorners(float outerRadius)
	{
		Vector2[] points = new Vector2[6];

		for (int i = 0; i < 6; ++i)
		{
			points[i] = Quaternion.Euler(Vector3.forward * 60.0f * i) * (Vector3.left * outerRadius);
		}

		return points;
	}

	public static Vector2[] GetCorners(Orientation orientation, float outerRadius)
	{
		return (orientation == Orientation.Pointy) ?
			GetPointyCorners(outerRadius) :
			GetFlatCorners(outerRadius);
	}
	#endregion UTILITIES
}