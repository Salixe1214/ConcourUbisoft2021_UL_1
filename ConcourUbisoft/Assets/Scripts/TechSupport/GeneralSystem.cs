using System;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using TechSupport.Informations;
using TechSupport.Surveillance;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace TechSupport
{
    public enum SurveillanceMode
    {
        Grid,
        Focused
    }
    public class GeneralSystem : MonoBehaviour
    {
        [Serializable]
        private struct OrderedItems
        {
            public int number;
            public SurveillanceCamera items;

            public OrderedItems(int number, SurveillanceCamera items, Button button)
            {
                this.items = items;
                this.number = number;
            }
        }

        private readonly GridSystem _gridSystem = new GridSystem();
        private readonly FullScreenSystem _fullScreenSystem = new FullScreenSystem();
        private InformationsSystem _informationsSystem;

        [SerializeField] private SurveillanceMode mode = SurveillanceMode.Grid; // Default mode : grid
        [SerializeField] private List<OrderedItems> cameras;
        private List<Button> _buttons;
        private GameController _gameController;

        #region Callbacks

        public Action OnModeSwitched;

        public readonly IDictionary<SurveillanceMode, Action> _onSwitchMethods;
        public readonly IDictionary<SurveillanceMode, Action> _exitMethods;

        public GeneralSystem()
        {
            _onSwitchMethods
                = new Dictionary<SurveillanceMode, Action>()
                {
                    { SurveillanceMode.Focused, OnFullScreen },
                    { SurveillanceMode.Grid, OnGrid },
                };
            _exitMethods
                = new Dictionary<SurveillanceMode, Action>()
                {
                    { SurveillanceMode.Focused, ExitFullScreen },
                    { SurveillanceMode.Grid, ExitGrid },
                };
        }

        #endregion

        private void Awake()
        {
            _gameController = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameController>();
            _informationsSystem = GetComponent<InformationsSystem>();
            cameras.Sort((a, b) => a.number.CompareTo(b.number));
            cameras.ForEach(c =>
            {
                c.items.Init();
            });
            _gridSystem.Init(cameras.Count());
            _buttons = new List<Button>();
            foreach (var items in cameras)
            {
                Button b = GameObjectsInstantiator.InstantiateButton(transform);
                b.onClick.AddListener(Focus);
                _buttons.Add(b);
            }
            _gridSystem.Grid(_buttons);

            _fullScreenSystem.SetTarget(cameras.First().items);
            _informationsSystem.Init();
            SystemSwitch(mode);
        }

        private void Update()
        {
            if (_gameController && _gameController.IsGameMenuOpen) return;
            if (!Input.GetButtonUp(InputManager.GetInputNameByController("CameraEscape"))) return;
            if (mode == SurveillanceMode.Focused)
            {
                SystemSwitch(SurveillanceMode.Grid);
            }
        }

        private void Focus()
        {
            if (_gameController && _gameController.IsGameMenuOpen) return;
            if (mode == SurveillanceMode.Grid)
            {
                SystemSwitch(SurveillanceMode.Focused);
            }
        }

        #region Camera

        private void EnableAll(bool enabledCamera)
        {
            cameras.ForEach(cam => cam.items.Enable(enabledCamera));
        }

        #endregion

        #region General System

        private void SystemSwitch(SurveillanceMode newMode)
        {
            if (mode != newMode)
            {
                _exitMethods[mode]?.Invoke();
            }
            _onSwitchMethods[mode = newMode]?.Invoke();
            OnModeSwitched?.Invoke();
        }

        private void ExitFullScreen()
        {
            _fullScreenSystem.EscapeFullScreen();
        }

        private void ExitGrid()
        {
            SurveillanceCamera selected =
                cameras.First(item => item.items.Contains(Input.mousePosition)).items;
            if (selected != null)
            {
                _fullScreenSystem.SetTarget(selected);
            }
        }

        private void OnGrid()
        {
            EnableAll(true);
            _gridSystem.Grid(cameras.Select(input => input.items.GetCamera()));
        }

        private void OnFullScreen()
        {
            EnableAll(false);
            _fullScreenSystem.RenderFullScreen();
        }

        #endregion
    }
}
