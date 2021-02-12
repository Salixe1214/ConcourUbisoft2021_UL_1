using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TechSupport
{
    enum SurveillanceMode
    {
        Grid,
        Focused
    }
    public class SurveillanceSystem : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private SurveillanceMode mode = SurveillanceMode.Grid; // Default mode : grid
        private readonly GridSystem _gridSystem = new GridSystem();
        private readonly FullScreenSystem _fullScreenSystem = new FullScreenSystem();

        private List<SurveillanceCamera> _cameras = new List<SurveillanceCamera>();

        private void Start()
        {
            _cameras = FindObjectsOfType<SurveillanceCamera>().ToList();
            _gridSystem.SearchGridSize(_cameras.Count);
            _fullScreenSystem.SetTarget(_cameras.First().GetCamera());
            SystemSwitch(mode);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (mode == SurveillanceMode.Grid)
                {
                    Select(Input.mousePosition);
                }
            } else if (Input.GetButtonUp("Cancel"))
            {
                if (mode == SurveillanceMode.Focused)
                {
                    Escape();
                }
            }
        }

        #region Camera

        private void EnableAll(bool enabledCamera)
        {
            foreach (SurveillanceCamera cam in _cameras)
            {
                cam.Enable(enabledCamera);
            }
        }

        #endregion

        #region General System

        private void Select(Vector3 mousePosition)
        {
            SurveillanceCamera selected =
                _cameras.Find(surveillanceCamera => surveillanceCamera.Contains(mousePosition));
            if (selected == null)
            {
                return;
            }
            _fullScreenSystem.SetTarget(selected.GetCamera());
            SystemSwitch(SurveillanceMode.Focused);
        }

        private void Escape()
        {
            SystemSwitch(SurveillanceMode.Grid);
            _fullScreenSystem.EscapeFullScreen();
        }

        private void SystemSwitch(SurveillanceMode newMode)
        {
            EnableAll(newMode != SurveillanceMode.Focused);
            mode = newMode;
            switch (newMode)
            {
                case SurveillanceMode.Grid:
                    _gridSystem.Grid(_cameras.ConvertAll(input => input.GetCamera()),
                        _gridSystem.GetGridSize());
                    break;
                case SurveillanceMode.Focused:
                    _fullScreenSystem.RenderFullScreen();
                    break;
            }
        }

        #endregion
    }
}
