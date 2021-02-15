using System.Collections.Generic;
using Arm;
using TechSupport.Surveillance;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Controller
{
    public class ControllerSystem : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private List<ArmController> controllers;

        [Header("Button")]
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject buttonPrefabs;
        [SerializeField] private Vector3 offset;
        private readonly Quaternion _rotation = Quaternion.identity; // No rotation needed

        #region Init

        // TODO:
        void Start()
        {
            Vector3 position = buttonPrefabs.transform.position;
            SurveillanceSystem surveillanceSystem = FindObjectOfType<SurveillanceSystem>();
            int index = 1;

            surveillanceSystem.OnGridMode += Reset;
            foreach (ArmController controller in controllers)
            {
                GameObject button = CreateButton("Controller " + index,controller, position);
                surveillanceSystem.OnGridMode += delegate { button.SetActive(false); };
                surveillanceSystem.OnFullScreenMode += delegate { button.SetActive(surveillanceSystem.IsTarget(gameObject)); };
                button.SetActive(surveillanceSystem.IsTarget(gameObject));
                position += offset;
                index++;
            }
        }

        GameObject CreateButton(string controllerName, ArmController controller, Vector3 position)
        {
            GameObject buttonGameObject = Instantiate(buttonPrefabs, position, _rotation, parent);

            buttonGameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>(true)?.SetText(controllerName);
            buttonGameObject.GetComponent<Button>()?.onClick.AddListener(delegate { ControlConvoyer(controller); });
            return buttonGameObject;
        }

        #endregion
        
        
        // TODO:
        private void ControlConvoyer(ArmController controller)
        {
            if (controller.IsControlled())
            {
                return; // No need to reset if the chosen controller is already on control
            }
            controllers.ForEach(c => c.ControlConvoyer(false));
            controller.ControlConvoyer(true);
        }

        private void Reset()
        {
            controllers.ForEach(c => c.ControlConvoyer(false)); // If the focus is not anymore on the camera
        }
    }
}
