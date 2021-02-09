using UnityEngine;

public class IKJoint : MonoBehaviour
{
    public Transform Target;

    protected Vector3 StartDirection;

    protected Quaternion StartRotation;

    void Awake()
    {
        if (Target == null)
            return;

        StartDirection = Target.position - transform.position;
        StartRotation = transform.rotation;
    }

    public void RotateToTarget()
    {
        if (Target == null)
            return;


        transform.rotation = Quaternion.FromToRotation(StartDirection, Target.position - transform.position) *
                             StartRotation;
    }
}