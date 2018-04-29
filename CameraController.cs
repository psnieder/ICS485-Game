using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

// followed tutorial on Renaissance Coders YouTube channel

	public Transform target;

	[System.Serializable]
	public class PositionSettings
	{
		public Vector3 targetPosOffset = new Vector3 (0, 1.0f, 0);
		public float lookSmooth = 100f;
		public float distanceFromTarget = -3;
		public bool smoothFollow = true;
		public float smooth = 0.05f;

		[HideInInspector]
		public float adjDistance = -3;
	}

	[System.Serializable]
	public class OrbitSettings 
	{
		public float xRotation = -20.0f;
		public float yRotation = -180.0f;
		public float maxXrotation = 45.0f;
		public float minXRotation = -85.0f;
		public float vOrbitSmooth = 150.0f;
		public float hOrbitSmooth = 150.0f;
	}

	[System.Serializable]
	public class InputSettings
	{
		public string ORBIT_HORIZONTAL = "Orbit X";
		public string ORBIT_VERTICAL = "Orbit Y";
		public string HORIZONTAL_SNAP = "Horizontal Snap";
	}
		
	public PositionSettings position = new PositionSettings ();
	public OrbitSettings orbit = new OrbitSettings ();
	public InputSettings input = new InputSettings ();
	public CameraCollision collision = new CameraCollision ();

	Vector3 targetPos = Vector3.zero;
	Vector3 destination = Vector3.zero;
	Vector3 adjDestination = Vector3.zero;
	Vector3 camVel = Vector3.zero;
	float vOrbitInput;
	float hOrbitInput;
	float hOrbitSnapInput;

	void Start()
	{
		SetCameraTarget (target);
		MoveToTarget ();
		collision.Initialize (Camera.main);
		collision.UpdateCameraClipPoints (transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPoints (destination, transform.rotation, ref collision.desiredCameraClipPoints);
	}

	public void SetCameraTarget(Transform t)
	{
		target = t;
	}

	void Update()
	{
		GetInput ();
	}

	void LateUpdate()
	{
		MoveToTarget ();
		LookAtTarget ();
		OrbitTarget ();

		collision.UpdateCameraClipPoints (transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPoints (destination, transform.rotation, ref collision.desiredCameraClipPoints);

		collision.CheckColliding (targetPos);
		position.adjDistance = collision.GetAdjustedPositionWithRayFrom (targetPos);
	}

	void MoveToTarget()
	{
		targetPos = target.position + position.targetPosOffset;
		destination = Quaternion.Euler (orbit.xRotation, orbit.yRotation, 0) * -Vector3.forward * position.distanceFromTarget;
		destination += targetPos;

		if (collision.colliding) 
		{
			adjDestination = Quaternion.Euler (orbit.xRotation, orbit.yRotation, 0) * Vector3.forward * position.adjDistance;
			adjDestination += targetPos;

			if (position.smoothFollow) 
			{
				transform.position = Vector3.SmoothDamp (transform.position, adjDestination, ref camVel, position.smooth);
			} 
			else 
			{
				transform.position = adjDestination;
			}
		} 
		else 
		{
			if (position.smoothFollow) 
			{
				transform.position = Vector3.SmoothDamp (transform.position, destination, ref camVel, position.smooth);
			} 
			else 
			{
				transform.position = destination;
			}
		}
	}

	void LookAtTarget()
	{
		Quaternion targetRotation = Quaternion.LookRotation (targetPos - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
	}

	void GetInput()
	{
		vOrbitInput = Input.GetAxis (input.ORBIT_VERTICAL);
		hOrbitInput = Input.GetAxis (input.ORBIT_HORIZONTAL);
		hOrbitSnapInput = Input.GetAxis (input.HORIZONTAL_SNAP);
	}

	void OrbitTarget()
	{
		if (hOrbitSnapInput > 0) 
		{
			orbit.yRotation = -180 + target.eulerAngles.y;
		}

		orbit.xRotation += GameManager.Instance.inverted * vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime;
		orbit.yRotation += hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime;

		if (orbit.xRotation > orbit.maxXrotation) 
		{
			orbit.xRotation = orbit.maxXrotation;
		}
		if (orbit.xRotation < orbit.minXRotation) 
		{
			orbit.xRotation = orbit.minXRotation;
		}
	}

}
