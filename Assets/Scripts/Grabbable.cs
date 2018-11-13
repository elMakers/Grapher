using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Grabbable : MonoBehaviour
{
	public float RequiredConfidence = 0.9f;
	public float GrabDistance = 2.0f;
	
	private Rigidbody _body;
	
	// Use this for initialization
	private void Start () {
		_body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	private void Update()
	{
		bool grabbed = false;
		bool grabbedLeft = false;
		MLHandKeyPose pose = MLHandKeyPose.NoHand;		
		if (MLHands.Right.KeyPoseConfidence >= RequiredConfidence && MLHands.Right.KeyPose != MLHandKeyPose.NoHand)
		{
			pose = MLHands.Right.KeyPose;
		} 
		else if (MLHands.Left.KeyPoseConfidence >= RequiredConfidence)
		{
			grabbedLeft = true;
			pose = MLHands.Left.KeyPose;
		}

		if (pose == MLHandKeyPose.Pinch)
		{
			grabbed = true;
		}

		if (grabbed)
		{
			Vector3 position = grabbedLeft ? MLHands.Left.Center : MLHands.Right.Center;
			_body.transform.position = position + Camera.main.transform.forward * GrabDistance;
			var targetRotation = Camera.main.transform.rotation.eulerAngles;
			targetRotation.x = 0;
			targetRotation.z = 0;
			_body.transform.rotation = Quaternion.Euler(targetRotation);
		}
	}
}
