using System;
using System.Collections.Generic;
using TechSupport.Controller;
using UnityEngine;
using UnityEngine.UI;
// TODO: Credit to <div>Icons made by <a href="https://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
namespace TechSupport.Surveillance
{
    [RequireComponent(typeof(Camera))]
    public class SurveillanceCamera : MonoBehaviour
    {
        private Camera _camera;
        private Rect defaultRect;

        [Header("Display")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text clockText;

        [Header("Controllable")]
        [SerializeField] private List<ControllableOutline> controllable = new List<ControllableOutline>();

        public void Init()
        {
            _camera = GetComponent<Camera>();
            ActivateClock(false);
            if (nameText != null)
                nameText.text = gameObject.name;
        }

        private void Update()
        {
            clockText.text = GetClockText();
        }

        private static string GetClockText()
        {
            return DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss");
        }

        public Rect GetPrintedRect()
        {
            Rect rect = _camera.rect;

            return new Rect(
                    rect.x * Screen.width,
                    rect.y * Screen.height,
                (rect.x + rect.width) * Screen.width,
                (rect.y + rect.height) * Screen.height);
        }
        
        public bool Contains(Vector3 point)
        {
            Rect rect = GetPrintedRect();
            return (point.x > rect.x && point.x < rect.width) &&
                   (point.y > rect.y && point.y < rect.height);
        }

        public Camera GetCamera()
        {
            return _camera;
        }

        public void Enable(bool enabledCamera)
        {
            _camera.enabled = enabledCamera;
        }

        public void EnableController(bool enableController)
        {
            controllable.ForEach(outline => outline.Enable(enableController, _camera));
        }

        public void ActivateClock(bool activation)
        {
            // clockText.gameObject.SetActive(activation);
        }
    }
}
