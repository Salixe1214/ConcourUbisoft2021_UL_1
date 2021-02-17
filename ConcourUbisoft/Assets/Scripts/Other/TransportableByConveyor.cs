using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TransportableByConveyor : MonoBehaviour
{
    [SerializeField] public bool HasBeenPickUp = false;

    private new Renderer renderer = null;
    public Color Color { get { return renderer.material.color; } set { renderer.material.color = value; } }
    public bool Consumed { get; set; }

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
}
