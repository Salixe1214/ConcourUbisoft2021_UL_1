using System.Collections;
using System.Collections.Generic;
using Arm;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ArmSound : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private ArmController armController;
	private Vector3 lastHeadPosition;
	private AudioSource audioSource;

	void Start()
	{
		lastHeadPosition = armController.Head.position;

		audioSource = GetComponent<AudioSource>();
		audioSource.loop = true;
	}

	void LateUpdate()
	{
		float distance = Vector3.Distance(armController.Head.position, lastHeadPosition);
		if (distance >= float.Epsilon)
		{
			audioSource.volume = distance / (armController.ControlSpeed * Time.deltaTime);
		}
		else
		{
			audioSource.volume = 0;
		}
		lastHeadPosition = armController.Head.position;
	}
}