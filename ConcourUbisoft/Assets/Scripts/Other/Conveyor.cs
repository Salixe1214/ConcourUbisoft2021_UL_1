using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Conveyor : MonoBehaviour
{
    public class ConveyorObjectData
    {
        public TransportableByConveyor TransportableByConveyor { get; set; }
        public Rigidbody Rigidbody { get; set; }
        public int NumberOfTimeOnConveyor { get; set; }
    }

    [SerializeField] protected float Speed = 0.0f;
    [SerializeField] protected int Priority = 0;
    [SerializeField] private TextureAnimator[] textureAnimators = null;

    private Dictionary<TransportableByConveyor, ConveyorObjectData> objectsOnConveyor = new Dictionary<TransportableByConveyor, ConveyorObjectData>();
    private List<TransportableByConveyor> toRemoveNullReference = new List<TransportableByConveyor>();

    private void OnCollisionEnter(Collision collision)
    {
        TransportableByConveyor transportableByConveyor = null;
        Rigidbody rigidbody = null;
        if (collision.gameObject.TryGetComponent(out transportableByConveyor) && collision.gameObject.TryGetComponent(out rigidbody))
        {
            if (!objectsOnConveyor.ContainsKey(transportableByConveyor))
            {
                objectsOnConveyor[transportableByConveyor] = new ConveyorObjectData() { NumberOfTimeOnConveyor = 0, Rigidbody = rigidbody, TransportableByConveyor = transportableByConveyor };
                transportableByConveyor.AddConveyor(Priority, this);
            }
            objectsOnConveyor[transportableByConveyor].NumberOfTimeOnConveyor = objectsOnConveyor[transportableByConveyor].NumberOfTimeOnConveyor + 1;
            Debug.Log($"{collision.gameObject.name} has entered the conveyor. ({objectsOnConveyor[transportableByConveyor].NumberOfTimeOnConveyor})");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        TransportableByConveyor transportableByConveyor = null;
        Rigidbody rigidbody = null;
        if (collision.gameObject.TryGetComponent(out transportableByConveyor) && collision.gameObject.TryGetComponent(out rigidbody))
        {
            if (objectsOnConveyor.ContainsKey(transportableByConveyor))
            {
                objectsOnConveyor[transportableByConveyor].NumberOfTimeOnConveyor = objectsOnConveyor[transportableByConveyor].NumberOfTimeOnConveyor - 1;
                Debug.Log($"{collision.gameObject.name} has left the conveyor. ({objectsOnConveyor[transportableByConveyor].NumberOfTimeOnConveyor})");
                if (objectsOnConveyor[transportableByConveyor].NumberOfTimeOnConveyor == 0)
                {
                    transportableByConveyor.RemoveConveyor(this);
                    objectsOnConveyor.Remove(transportableByConveyor);
                }
            }
        }
    }

    private void Update()
    {
        toRemoveNullReference.Clear();
        foreach (KeyValuePair<TransportableByConveyor, ConveyorObjectData> objectOnConveyor in objectsOnConveyor)
        {
            if (objectOnConveyor.Key != null)
            {
                if (objectOnConveyor.Value.TransportableByConveyor.GetFirstConveyorToAffectObject() == this)
                {
                    MoveObject(objectOnConveyor.Value.Rigidbody);
                }
            }
            else
            {
                toRemoveNullReference.Add(objectOnConveyor.Key);
            }
        }
        toRemoveNullReference.ForEach(x => objectsOnConveyor.Remove(x));
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;

        foreach (TextureAnimator ta in textureAnimators)
        {
            ta.SetTranslation(new Vector2(speed, 0));
        }
    }

    public float GetSpeed()
    {
        return Speed;
    }

    protected abstract void MoveObject(Rigidbody rigidbody);
}

