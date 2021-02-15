using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TechSupport.Surveillance
{
    public enum SurveillanceMode
    {
        Grid,
        Focused
    }
    public class SurveillanceSystem : MonoBehaviour
    {
        [SerializeField] private SurveillanceMode mode = SurveillanceMode.Grid; // Default mode : grid
        private readonly GridSystem _gridSystem = new GridSystem();
        private readonly FullScreenSystem _fullScreenSystem = new FullScreenSystem();

        private List<SurveillanceCamera> _cameras = new List<SurveillanceCamera>();

        #region Callbacks

        public Action<SurveillanceMode, SurveillanceMode> OnSwitchMode;
        public Action OnModeSwitched;
        public Action OnGridMode;
        public Action OnFullScreenMode;
        
        #endregion

        private void Start()
        {
            _cameras = FindObjectsOfType<SurveillanceCamera>().ToList();
            _gridSystem.SearchGridSize(_cameras.Count);
            _fullScreenSystem.SetTarget(_cameras.First().GetCamera());

            OnGridMode += OnGrid;
            OnFullScreenMode += OnFullScreen;
            OnSwitchMode += OnSwitch;
            
            SystemSwitch(mode);
        }

        // TODO: Improve this basic input system
        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (mode == SurveillanceMode.Grid)
                {
                    SystemSwitch(SurveillanceMode.Focused);
                }
            } else if (Input.GetButtonUp("Cancel"))
            {
                if (mode == SurveillanceMode.Focused)
                {
                    SystemSwitch(SurveillanceMode.Grid);
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

        public bool IsTarget(GameObject obj)
        {
            return _fullScreenSystem.GetTarget().gameObject.GetInstanceID() == obj.GetInstanceID();
        }
        
        #endregion

        #region General System

        private void SystemSwitch(SurveillanceMode newMode)
        {
            if (mode == newMode)
            {
                return;
            }

            OnSwitchMode?.Invoke(mode, newMode);
            // TODO: Make a tab of it
            switch (mode = newMode)
            {
                case SurveillanceMode.Grid:
                    OnGridMode?.Invoke();
                    break;
                case SurveillanceMode.Focused:
                    OnFullScreenMode?.Invoke();
                    break;
            }
            OnModeSwitched?.Invoke();
        }

        private void OnSwitch(SurveillanceMode oldMode, SurveillanceMode newMode)
        {
            // TODO: 
            switch (oldMode)
            {
                case SurveillanceMode.Focused:
                    _fullScreenSystem.EscapeFullScreen();
                    break;
                case SurveillanceMode.Grid:
                    SurveillanceCamera selected =
                        _cameras.Find(surveillanceCamera => surveillanceCamera.Contains(Input.mousePosition));
                    if (selected != null)
                    {
                        _fullScreenSystem.SetTarget(selected.GetCamera());
                    }
                    break;
            }
        }

        private void OnGrid()
        {
            EnableAll(true);
            _gridSystem.Grid(_cameras.ConvertAll(input => input.GetCamera()),
                _gridSystem.GetGridSize());
        }

        private void OnFullScreen()
        {
            EnableAll(false);
            _fullScreenSystem.RenderFullScreen();
        }

        #endregion
    }
}
