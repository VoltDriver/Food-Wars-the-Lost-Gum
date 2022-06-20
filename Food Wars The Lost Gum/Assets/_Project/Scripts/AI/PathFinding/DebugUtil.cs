using UnityEngine;
// CODE BY DANIEL RINALDI
// MODIFIED BY: Joel Lajoie-Corriveau
public static class DebugUtil
{
	public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1f)
	{
		up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
		Vector3 _forward = Vector3.Slerp(up, -up, 0.5f);
		Vector3 _right = Vector3.Cross(up, _forward).normalized * radius;

		Matrix4x4 matrix = new Matrix4x4();

		matrix[0] = _right.x;
		matrix[1] = _right.y;
		matrix[2] = _right.z;

		matrix[4] = up.x;
		matrix[5] = up.y;
		matrix[6] = up.z;

		matrix[8] = _forward.x;
		matrix[9] = _forward.y;
		matrix[10] = _forward.z;

		Vector3 lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
		Vector3 nextPoint = Vector3.zero;

		for (var i = 0; i < 91; i++)
		{
			nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
			nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
			nextPoint.y = 0;

			nextPoint = position + matrix.MultiplyPoint3x4(nextPoint);

			Debug.DrawLine(lastPoint, nextPoint, color);
			lastPoint = nextPoint;
		}
	}

	public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1)
	{
		Vector3 up = (end - start).normalized * radius;
		Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
		Vector3 right = Vector3.Cross(up, forward).normalized * radius;

		Color oldColor = Gizmos.color;
		Gizmos.color = color;

		float height = (start - end).magnitude;
		float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
		Vector3 middle = (end + start) * 0.5f;

		start = middle + ((start - middle).normalized * sideLength);
		end = middle + ((end - middle).normalized * sideLength);

		//Radial circles
		DrawCircle(start, up, color, radius);
		DrawCircle(end, -up, color, radius);

		//Side lines
		Gizmos.DrawLine(start + right, end + right);
		Gizmos.DrawLine(start - right, end - right);

		Gizmos.DrawLine(start + forward, end + forward);
		Gizmos.DrawLine(start - forward, end - forward);

		for (int i = 1; i < 26; i++)
		{

			//Start endcap
			Gizmos.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start);
			Gizmos.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start);

			//End endcap
			Gizmos.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end);
			Gizmos.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end);
		}

		Gizmos.color = oldColor;
	}

	public static void DrawWireSphere(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
	{
		float angle = 10.0f;

		Vector3 x = new Vector3(position.x, position.y + radius * Mathf.Sin(0), position.z + radius * Mathf.Cos(0));
		Vector3 y = new Vector3(position.x + radius * Mathf.Cos(0), position.y, position.z + radius * Mathf.Sin(0));
		Vector3 z = new Vector3(position.x + radius * Mathf.Cos(0), position.y + radius * Mathf.Sin(0), position.z);

		Vector3 new_x;
		Vector3 new_y;
		Vector3 new_z;

		for (int i = 1; i < 37; i++)
		{

			new_x = new Vector3(position.x, position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
			new_y = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
			new_z = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z);

			Debug.DrawLine(x, new_x, color, duration, depthTest);
			Debug.DrawLine(y, new_y, color, duration, depthTest);
			Debug.DrawLine(z, new_z, color, duration, depthTest);

			x = new_x;
			y = new_y;
			z = new_z;
		}
	}

	public static void DebugCapsule(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true)
	{
		Vector3 up = (end - start).normalized * radius;
		Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
		Vector3 right = Vector3.Cross(up, forward).normalized * radius;

		float height = (start - end).magnitude;
		float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
		Vector3 middle = (end + start) * 0.5f;

		start = middle + ((start - middle).normalized * sideLength);
		end = middle + ((end - middle).normalized * sideLength);

		//Radial circles
		DebugUtil.DebugCircle(start, up, color, radius, duration, depthTest);
		DebugUtil.DebugCircle(end, -up, color, radius, duration, depthTest);
		//Side lines
		Debug.DrawLine(start + right, end + right, color, duration, depthTest);
		Debug.DrawLine(start - right, end - right, color, duration, depthTest);

		Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
		Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

		for (int i = 1; i < 26; i++)
		{

			//Start endcap
			Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);

			//End endcap
			Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
		}
	}

	public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
	{
		Vector3 _up = up.normalized * radius;
		Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
		Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

		Matrix4x4 matrix = new Matrix4x4();

		matrix[0] = _right.x;
		matrix[1] = _right.y;
		matrix[2] = _right.z;

		matrix[4] = _up.x;
		matrix[5] = _up.y;
		matrix[6] = _up.z;

		matrix[8] = _forward.x;
		matrix[9] = _forward.y;
		matrix[10] = _forward.z;

		Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
		Vector3 _nextPoint = Vector3.zero;

		color = (color == default(Color)) ? Color.white : color;

		for (var i = 0; i < 91; i++)
		{
			_nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
			_nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
			_nextPoint.y = 0;

			_nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

			Debug.DrawLine(_lastPoint, _nextPoint, color, duration, depthTest);
			_lastPoint = _nextPoint;
		}
	}
}
