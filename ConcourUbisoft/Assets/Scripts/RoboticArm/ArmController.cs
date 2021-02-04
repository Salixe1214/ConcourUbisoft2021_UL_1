using System;
using DitzelGames.FastIK;
using UnityEngine;

enum ControlScheme
{
    WASD,
    ROTATION,
}

public class ArmController : FastIKFabric
{
    [SerializeField] private ControlScheme scheme;
    [SerializeField] float speed = 1f;
    [SerializeField] private Transform targetRoot;
    [SerializeField] private float rotationSpeed = 2f;
    private float maxRange;
    private float minRange;

    private void Start()
    {
        maxRange = CompleteLength - 0.1f;
        minRange = CompleteLength / 4;
    }

    void Update()
    {
        if (scheme == ControlScheme.WASD)
        {
            Vector3 translation = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                translation += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                translation += Vector3.back;
            }

            if (Input.GetKey(KeyCode.A))
            {
                translation += Vector3.left;
            }

            if (Input.GetKey(KeyCode.D))
            {
                translation += Vector3.right;
            }

            Target.transform.Translate(Time.deltaTime * speed * translation);
            float distanceToTarget = Vector3.Distance(Root.position, Target.position);
            if (distanceToTarget > maxRange)
            {
                Vector3 dirToTarget = (Target.transform.position - Root.transform.position).normalized;
                Target.transform.position = new Vector3(
                    dirToTarget.x * maxRange,
                    Target.transform.position.y,
                    dirToTarget.z * maxRange);
            }

            if (distanceToTarget < minRange)
            {
                Vector3 dirToTarget = (Target.transform.position - Root.transform.position).normalized;
                Target.transform.position = new Vector3(
                    dirToTarget.x * minRange,
                    Target.transform.position.y,
                    dirToTarget.z * minRange);
            }
        }
        else if (scheme == ControlScheme.ROTATION)
        {
            float translation = 0;

            if (Input.GetKey(KeyCode.W))
                translation += 1;
            if (Input.GetKey(KeyCode.S))
                translation -= 1;

            if (Input.GetKey(KeyCode.D))
                targetRoot.Rotate(new Vector3(0, 1, 0), Mathf.Rad2Deg * rotationSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.A))
                targetRoot.Rotate(new Vector3(0, 1, 0), Mathf.Rad2Deg * -rotationSpeed * Time.deltaTime);

            Target.Translate(new Vector3(translation * speed * Time.deltaTime, 0, 0));

            if (Target.localPosition.x > maxRange)
            {
                Target.localPosition = Vector3.right * maxRange;
            }
            else if (Target.localPosition.x < minRange)
            {
                Target.localPosition = Vector3.right * minRange;
            }
        }

        Vector3 direction = Target.position - Root.position;
        direction.y = 0;
        direction.Normalize();
        float rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Root.rotation = Quaternion.Euler(0f, rotationY - 90, 0f);
    }
}