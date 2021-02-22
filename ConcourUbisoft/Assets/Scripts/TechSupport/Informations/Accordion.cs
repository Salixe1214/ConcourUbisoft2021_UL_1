using System;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{ 
    [RequireComponent(typeof(VerticalLayoutGroup)), RequireComponent(typeof(ContentSizeFitter)), RequireComponent(typeof(ToggleGroup))]
    public class Accordion : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<ToggleGroup>().allowSwitchOff = true;
        }
    }
}
