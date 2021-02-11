using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private float Speed = 0.0f;
    [SerializeField] private bool ClockWise = true;

    private HashSet<Rigidbody> objectsOnConveyor = new HashSet<Rigidbody>();
    private List<Rigidbody> toRemoveNullReference = new List<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        TransportableByConveyor transportableByConveyor = null;
        Rigidbody rigidbody = null;
        if (collision.gameObject.TryGetComponent(out transportableByConveyor) && collision.gameObject.TryGetComponent(out rigidbody))
        {
            Debug.Log($"{collision.gameObject.name} has entered the conveyor");
            objectsOnConveyor.Add(rigidbody);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        TransportableByConveyor transportableByConveyor = null;
        Rigidbody rigidbody = null;
        if (collision.gameObject.TryGetComponent(out transportableByConveyor) && collision.gameObject.TryGetComponent(out rigidbody))
        {
            Debug.Log($"{collision.gameObject.name} has left the conveyor");
            objectsOnConveyor.Remove(rigidbody);
        }
    }

    private void FixedUpdate()
    {
        toRemoveNullReference.Clear();
        foreach (Rigidbody objectOnConveyor in objectsOnConveyor)
        {
            if(objectOnConveyor != null)
            {
                MoveObject(objectOnConveyor);
            }
            else
            {
                toRemoveNullReference.Add(objectOnConveyor);
            }
        }
        toRemoveNullReference.ForEach(x => objectsOnConveyor.Remove(x));
    }

    private void MoveObject(Rigidbody rigidbody)
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

        rigidbody.MovePosition(transform.position + new Vector3(rotateVector.x, rigidbody.position.y, rotateVector.y));
    }
}
