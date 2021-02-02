
using UnityEngine;

public class IK : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform[] joins;
    private float[] distances;

    private void Start()
    {
        distances = new float[joins.Length-1];
    }

    void Update()
    {
        
    }
}
