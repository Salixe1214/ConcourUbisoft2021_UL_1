using System.Collections;
using System.Collections.Generic;
using Arm;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ArmSound : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private ArmController armController;
    [SerializeField] private int smoothIterationCount = 10;
    [SerializeField] private float volumeMultiplier = 1;
    private Vector3 lastTargetPosition;
    private AudioSource audioSource;

    void Start()
    {
        lastTargetPosition = target.position;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    void LateUpdate()
    {
        float distance = Vector3.Distance(lastTargetPosition, target.position);
        if (distance >= float.Epsilon)
        {
            float volume = distance / (armController.ControlSpeed * Time.deltaTime);
            audioSource.volume =
                (((audioSource.volume * smoothIterationCount) + volume) / (smoothIterationCount + 1)) *
                volumeMultiplier;
        }
        else
        {
            audioSource.volume = 0;
        }

        lastTargetPosition = target.position;
    }
}