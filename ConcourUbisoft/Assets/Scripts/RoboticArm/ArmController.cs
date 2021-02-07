using System;
using DitzelGames.FastIK;
using UnityEngine;
using UnityEngine.Serialization;

enum ControlScheme
{
    WASD,
    ROTATION,
}

enum GrabState
{
    none,
    decending,
    grabbing,
    grabbed,
    realeasing,
}

public class ArmController : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private IKSolver armIKSolver;
    [SerializeField] private ControlScheme scheme;
    [SerializeField] float speed = 3f;
    [SerializeField] private float grabSpeed = 3f;
    [SerializeField] private Transform targetRotationRoot;
    [SerializeField] private Transform armRotationRoot;
    [SerializeField] private float rotationSpeed = 2f;

    private float maxRange;
    private float minRange;
    private GrabState grabState = GrabState.none;
    private Grabbable grabTarget = null;

    private void Start()
    {
        maxRange = armIKSolver.TotalLength - (armIKSolver.TotalLength / 5);
        minRange = armIKSolver.TotalLength / 4;
    }

    void Update()
    {
        UpdateArmTarget();
        UpdateHead();
    }

    private void UpdateArmTarget()
    {
        if (grabState == GrabState.none)
        {
            if (scheme == ControlScheme.WASD)
            {
                Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                armIKSolver.Target.transform.Translate(Time.deltaTime * speed * translation);
                float distanceToTarget = Vector3.Distance(transform.position, armIKSolver.Target.position);
                if (distanceToTarget > maxRange)
                {
                    Vector3 dirToTarget = (armIKSolver.Target.transform.position - transform.position).normalized;
                    armIKSolver.Target.transform.position = new Vector3(
                        dirToTarget.x * maxRange,
                        armIKSolver.Target.transform.position.y,
                        dirToTarget.z * maxRange);
                }

                if (distanceToTarget < minRange)
                {
                    Vector3 dirToTarget = (armIKSolver.Target.transform.position - transform.position).normalized;
                    armIKSolver.Target.transform.position = new Vector3(
                        dirToTarget.x * minRange,
                        armIKSolver.Target.transform.position.y,
                        dirToTarget.z * minRange);
                }
            }
            else if (scheme == ControlScheme.ROTATION)
            {
                float translation = Input.GetAxis("Vertical");
                targetRotationRoot.Rotate(new Vector3(0, 1, 0),
                    Mathf.Rad2Deg * Input.GetAxis("Horizontal") * Time.deltaTime);

                armIKSolver.Target.Translate(new Vector3(translation * speed * Time.deltaTime, 0, 0));

                if (armIKSolver.Target.localPosition.x > maxRange)
                {
                    armIKSolver.Target.localPosition = Vector3.right * maxRange;
                }
                else if (armIKSolver.Target.localPosition.x < minRange)
                {
                    armIKSolver.Target.localPosition = Vector3.right * minRange;
                }
            }
        }

        //arm base look at target
        Vector3 direction = armIKSolver.Target.position - transform.position;
        direction.y = 0;
        direction.Normalize();
        float rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        armRotationRoot.rotation = Quaternion.Euler(0f, rotationY - 90, 0f);
    }

    private void UpdateHead()
    {
        head.transform.rotation = Quaternion.Euler(0, head.transform.localRotation.y, 180);
        if (grabState == GrabState.none || grabState == GrabState.decending)
        {
            RaycastHit hit;
            if (Physics.Raycast(head.transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                Grabbable target = hit.transform.GetComponent<Grabbable>();
                if (target != null)
                {
                    if (grabState == GrabState.none && Input.GetKeyDown(KeyCode.Space))
                    {
                        grabState = GrabState.grabbing;
                        grabTarget = target;
                    }
                    else if (grabState == GrabState.decending)
                    {
                    }

                    Debug.DrawRay(head.transform.position, Vector3.down * hit.distance,
                        Color.green);
                }
                else if (grabState == GrabState.decending)
                {
                }
            }
            else
            {
                Debug.DrawRay(head.transform.position, Vector3.down * 1000, Color.red);
            }
        }
        else if (grabState == GrabState.grabbing)
        {
            if (Vector3.Distance(grabTarget.transform.position, transform.position) > maxRange)
            {
                grabState = GrabState.none;
                grabTarget = null;
            }

            //armIKSolver.Target.position -= Vector3.down * Time.deltaTime;
        }
    }
}