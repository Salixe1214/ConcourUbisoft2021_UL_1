using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleConveyor : Conveyor
{
    [SerializeField] private bool ClockWise = true;


    private Vector3 center = new Vector3();
    private new Collider collider = null;

    private void Awake()
    {
        collider = GetComponentInChildren<Collider>();
        center = collider.bounds.center + new Vector3(collider.bounds.extents.x, 0, 0);
    }

    protected override void MoveObject(Rigidbody rigidbody)
    {
        Vector3 centerToObject = rigidbody.position - center;
        Vector2 centerToObject2D = new Vector2(centerToObject.x, centerToObject.z);
        float radius = centerToObject2D.magnitude;
        float circumference = 2 * Mathf.PI * radius;
        float distanceToDo = Time.deltaTime * Speed;
        float percentOfCircumference = distanceToDo / circumference;
        float angle = percentOfCircumference * 360;
        //float angleInRadian = angle * Mathf.Deg2Rad * (ClockWise ? -1 : 1);
        //Vector2 rotateVector =
        //    new Vector2(centerToObject2D.x * Mathf.Cos(angleInRadian) - centerToObject2D.y * Mathf.Sin(angleInRadian),
        //        centerToObject2D.x * Mathf.Sin(angleInRadian) + centerToObject2D.y * Mathf.Cos(angleInRadian));

        //Vector3 newPosition = center + new Vector3(rotateVector.x, rigidbody.position.y - center.y, rotateVector.y);

        //rigidbody.transform.LookAt(newPosition, Vector3.up);
        rigidbody.transform.RotateAround(center, Vector3.up, (ClockWise ? 1 : -1) * angle);
        //rigidbody.MovePosition(newPosition);
    }
}