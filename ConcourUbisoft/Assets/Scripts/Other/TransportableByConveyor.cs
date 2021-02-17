using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TransportableByConveyor : MonoBehaviour
{
    [SerializeField] public bool HasBeenPickUp = false;

    private new Renderer renderer = null;
    public Color Color { get { return renderer.material.color; } set { renderer.material.color = value; } }
    public bool Consumed { get; set; }

    private SortedList<int, object> priorityConveyor = new SortedList<int, object>();

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void AddConveyor(int priority, object conveyor)
    {
        priorityConveyor.Add(priority, conveyor);
    }

    public void RemoveConveyor(object conveyor)
    {
        priorityConveyor.Remove(priorityConveyor.IndexOfValue(conveyor));
    }

    public object GetFirstConveyorToAffectObject()
    {
        if (priorityConveyor.Count == 0)
        {
            return null;
        }
        else
        {
            return priorityConveyor.First().Value;
        }
    }
}
