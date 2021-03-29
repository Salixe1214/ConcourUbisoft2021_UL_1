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
    private SortedList<int, Conveyor> priorityConveyor = new SortedList<int, Conveyor>();
    private Pickable pickable = null;

    private void Awake()
    {
        pickable = GetComponent<Pickable>();
    }

    public void AddConveyor(int priority, Conveyor conveyor)
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

    public void RemoveConveyor(Conveyor conveyor)
    {
        priorityConveyor.RemoveAt(priorityConveyor.IndexOfValue(conveyor));
    }

    public Conveyor GetFirstConveyorToAffectObject()
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

    public bool IsOnConveyor()
    {
        return priorityConveyor.Count > 0;
    }

    public float ConveyorSpeed()
    {
        if(priorityConveyor.Count == 0)
        {
            return 0.0f;
        }

        return priorityConveyor.First().Value.GetSpeed();
    }
}
