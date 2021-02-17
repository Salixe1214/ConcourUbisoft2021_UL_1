using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleConveyor : Conveyor
{
    [SerializeField] private bool ClockWise = true;

    protected override void MoveObject(Rigidbody rigidbody)
    {
        Vector3 centerToObject = rigidbody.position - transform.position;
        Vector2 centerToObject2D = new Vector2(centerToObject.x, centerToObject.z);
        float radius = centerToObject2D.magnitude;
        float circumference = 2 * Mathf.PI * radius;
        float distanceToDo = Time.deltaTime * Speed;
        float percentOfCircumference = distanceToDo / circumference;
        float angle = percentOfCircumference * 360;
        float angleInRadian = angle * Mathf.Deg2Rad * (ClockWise ? -1 : 1);
        Vector2 rotateVector = new Vector2(centerToObject2D.x * Mathf.Cos(angleInRadian) - centerToObject2D.y * Mathf.Sin(angleInRadian), centerToObject2D.x * Mathf.Sin(angleInRadian) + centerToObject2D.y * Mathf.Cos(angleInRadian));

        Vector3 newPosition = transform.position + new Vector3(rotateVector.x, rigidbody.position.y - transform.position.y, rotateVector.y);

        rigidbody.transform.LookAt(newPosition, Vector3.up);
        rigidbody.MovePosition(newPosition);
    }
}
