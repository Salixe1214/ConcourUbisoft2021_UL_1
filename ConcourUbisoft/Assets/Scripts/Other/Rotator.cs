using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    [SerializeField] private float speed=25;
    void Update()
    {
        transform.Rotate(Vector3.up*Time.deltaTime*speed);
    }
}
