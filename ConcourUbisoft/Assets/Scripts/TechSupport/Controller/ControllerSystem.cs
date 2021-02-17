using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Controller
{
    public class ControllerSystem : MonoBehaviour
    {
        private IEnumerable<Controllable> _controllers;
        private IEnumerable<GameObject> _buttons = new GameObject[]{};

        [Header("Button")]
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject buttonPrefabs;
        [SerializeField] private Vector3 offset;
        private readonly Quaternion _rotation = Quaternion.identity; // No rotation needed

        #region Init

        private void Awake()
        {
            Vector3 position = buttonPrefabs.transform.position;
            int index = 1;
            _controllers = FindObjectsOfType<Controllable>();

            foreach (Controllable controller in _controllers)
            {
                CreateButton("Controller " + index,controller, position);
                position += offset;
                index++;
            }
        }


        void CreateButton(string controllerName, Controllable controller, Vector3 position)
        {
            GameObject buttonGameObject = Instantiate(buttonPrefabs, position, _rotation, parent);

            buttonGameObject.gameObject.GetComponentInChildren<Text>(true).text = controllerName;
            buttonGameObject.GetComponent<Button>()?.onClick.AddListener(delegate { ControlConvoyer(controller); });
            _buttons = _buttons.Append(buttonGameObject);
        }

        #endregion

        #region Control

        // TODO:
        private void ControlConvoyer(Controllable controller)
        {
            if (controller.IsControlled)
            {
                return; // No need to reset if the chosen controller is already on control
            }

            Reset();
            controller.IsControlled = true;
        }

        private void Reset()
        {
            foreach (Controllable c in _controllers)
            {
                c.IsControlled = false;
            }
        }

        #endregion

    }
}
