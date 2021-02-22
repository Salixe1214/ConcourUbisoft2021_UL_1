using Arm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Pickable))]
[RequireComponent(typeof(Collider))]
public class TransportableByConveyor : MonoBehaviour
{
    public bool HasBeenPickUp { get { return pickable.HasBeenPickup; } set { pickable.HasBeenPickup = value; } }

    public Color Color { get { return renderer.material.color; } set { renderer.material.color = value; } }

    private SortedList<int, object> priorityConveyor = new SortedList<int, object>();

    private new Renderer renderer = null;
    private new Collider collider = null;
    private Pickable pickable = null;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        pickable = GetComponent<Pickable>();
        collider = GetComponent<Collider>();
    }

    public void AddConveyor(int priority, object conveyor)
    {
        if (!priorityConveyor.ContainsKey(priority))
        {
            priorityConveyor.Add(priority, conveyor);
        }
        else
        {
            if(!priorityConveyor.ContainsValue(conveyor))
            {
                Debug.LogError("You cannot have 2 conveyor with the same priority.");
            }
        }
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

    public void Consume()
    {
        collider.enabled = false;
    }
}
