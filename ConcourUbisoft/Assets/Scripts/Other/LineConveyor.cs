using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConveyor : Conveyor
{
    protected override void MoveObject(Rigidbody rigidbody)
    {
        Vector3 newPosition = rigidbody.position + this.transform.forward.normalized * Speed * Time.deltaTime;
        //rigidbody.transform.LookAt(newPosition, Vector3.up);
        rigidbody.transform.position = newPosition;
    }
}