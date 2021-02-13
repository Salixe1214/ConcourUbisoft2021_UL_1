using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTransportable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TransportableByConveyor transportableByConveyor = null;
        if (other.gameObject.TryGetComponent(out transportableByConveyor))
        {
            Destroy(other.gameObject);
        }
    }
}
