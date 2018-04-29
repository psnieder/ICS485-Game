using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraCollision {

// followed tutorial on Renaissance Coders YouTube channel

	public LayerMask collisionLayer;

	[HideInInspector] 
	public bool colliding = false;
	[HideInInspector]
	public Vector3[] adjustedCameraClipPoints;
	[HideInInspector]
	public Vector3[] desiredCameraClipPoints;

	Camera camera;

	public void Initialize(Camera cam)
	{
		camera = cam;
		adjustedCameraClipPoints = new Vector3[5];
		desiredCameraClipPoints = new Vector3[5];
	}

	bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
	{
		for (int i = 0; i < clipPoints.Length; i++) 
		{
			Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
				float distance = Vector3.Distance(clipPoints[i], fromPosition);
			if (Physics.Raycast (ray, distance, collisionLayer)) 
			{
				return true;
			}
		}

		return false;
	}

	public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
	{
		if (!camera)
			return;

		intoArray = new Vector3[5];

		float z = camera.nearClipPlane;
		float x = Mathf.Tan (camera.fieldOfView / 3.41f) * z;
		float y = x / camera.aspect;

		intoArray [0] = (atRotation * new Vector3 (-x, y, z)) + cameraPosition;
		intoArray [1] = (atRotation * new Vector3 (x, y, z)) + cameraPosition;
		intoArray [2] = (atRotation * new Vector3 (-x, -y, z)) + cameraPosition;
		intoArray [3] = (atRotation * new Vector3 (x, -y, z)) + cameraPosition;
		intoArray [4] = cameraPosition - camera.transform.forward;
	}

	public float GetAdjustedPositionWithRayFrom(Vector3 from)
	{
		float distance = -1;

		for (int i = 0; i < desiredCameraClipPoints.Length; i++) 
		{
			Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) 
			{
				if (distance == -1)
					distance = hit.distance;
				else 
				{
					if (hit.distance < distance)
						distance = hit.distance;
				}
			}
		}

		if (distance == -1)
			return 0;
		else
			return distance;
	}

	public void CheckColliding(Vector3 targetPosition)
	{
		if (CollisionDetectedAtClipPoints (desiredCameraClipPoints, targetPosition))
			colliding = true;
		else
			colliding = false;
	}
		
}
