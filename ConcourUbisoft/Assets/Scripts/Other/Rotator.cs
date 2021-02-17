using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float speed = 25;

    void Update()
    {
        transform.Rotate(Time.deltaTime * speed * Vector3.up);
    }
}