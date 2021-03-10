using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConveyor : Conveyor
{
    protected override void MoveObject(Rigidbody rigidbody)
    {
        rigidbody.MovePosition(rigidbody.position + this.transform.forward * Speed * Time.deltaTime);
    }
}