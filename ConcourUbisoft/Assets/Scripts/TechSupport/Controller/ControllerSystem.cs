using System;
using System.Collections.Generic;
using System.Linq;
using Arm;
using TechSupport.Surveillance;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Controller
{
    public class ControllerSystem : MonoBehaviour
    {
        private IEnumerable<ArmController> _controllers;
        private IEnumerable<GameObject> _buttons = new GameObject[]{};

        [Header("Button")]
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject buttonPrefabs;
        [SerializeField] private Vector3 offset;
        private readonly Quaternion _rotation = Quaternion.identity; // No rotation needed

        #region Init

        private void Start()
        {
            Vector3 position = buttonPrefabs.transform.position;
            int index = 1;
            _controllers = FindObjectsOfType<ArmController>();

            foreach (ArmController controller in _controllers)
            {
                CreateButton("Controller " + index,controller, position);
                position += offset;
                index++;
            }
        }


        void CreateButton(string controllerName, ArmController controller, Vector3 position)
        {
            GameObject buttonGameObject = Instantiate(buttonPrefabs, position, _rotation, parent);

            buttonGameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>(true)?.SetText(controllerName);
            buttonGameObject.GetComponent<Button>()?.onClick.AddListener(delegate { ControlConvoyer(controller); });
            _buttons = _buttons.Append(buttonGameObject);
        }

        #endregion

        #region Control

        // TODO:
        private void ControlConvoyer(ArmController controller)
        {
            if (controller.IsControlled())
            {
                return; // No need to reset if the chosen controller is already on control
            }

            Reset();
            controller.ControlConvoyer(true);
        }

        private void Reset()
        {
            foreach (ArmController c in _controllers)
            {
                c.ControlConvoyer(false);
            }
        }

        #endregion

    }
}
