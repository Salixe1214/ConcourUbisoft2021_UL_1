using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float speed = 25;
    [SerializeField] private Vector3 axis=Vector3.up;

    void Update()
    {
        transform.Rotate(Time.deltaTime * speed * axis);
    }
}