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
            Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

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
            float translation = Input.GetAxis("Vertical");
            targetRoot.Rotate(new Vector3(0, 1, 0), Mathf.Rad2Deg * Input.GetAxis("Horizontal") * Time.deltaTime);

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