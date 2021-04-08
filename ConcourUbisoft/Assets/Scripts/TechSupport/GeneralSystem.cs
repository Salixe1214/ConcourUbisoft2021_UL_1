using System;
using System.Collections.Generic;
using System.Linq;
using Buttons;
using Inputs;
using JetBrains.Annotations;
using TechSupport.Informations;
using TechSupport.Surveillance;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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

        [Header("Surveillance System")]
        [SerializeField] private SurveillanceMode mode = SurveillanceMode.Grid; // Default mode : grid
        [SerializeField] private List<OrderedItems> cameras;
        
        [Header("Button to Select camera")]
        [SerializeField] private Sprite outlineSprite;
        [SerializeField] private ColorBlock colors;

        private List<Button> _buttons;
        private GameController _gameController;
        private Inputs.Controller _currentController;
        private EventSystem _eventSystem;
        private InputManager _inputManager;
        private bool _gridIsOpened;

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
            _eventSystem = EventSystem.current;
            _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
            cameras.Sort((a, b) => a.number.CompareTo(b.number));
            cameras.ForEach(c => c.items.Init());
            _gridSystem.Init(cameras.Count());
            _fullScreenSystem.SetTarget(cameras.First().items);
            GridInterface();
            _gridIsOpened = false;
            _informationsSystem.Init();
            SystemSwitch(mode);
        }

        private void Start()
        {
            _currentController = InputManager.GetController();
        }

        private void OnEnable()
        {
            _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
            _gameController.OnInGameMenuClosed += OnInGameMenuClosed;
        }

        private void OnDisable()
        {
            _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
            _gameController.OnInGameMenuClosed -= OnInGameMenuClosed;
        }

        public void Escape()
        {
            if (_gameController && _gameController.IsGameMenuOpen) return; 
            if (mode == SurveillanceMode.Focused)
            {
                SystemSwitch(SurveillanceMode.Grid);
            }
            else
            {
                FocusBack();
            }
        }

        public void Focus()
        {
            if (_gameController && (_gameController.IsGameMenuOpen || _gameController.IsEndGameMenuOpen)) return;
            if (mode == SurveillanceMode.Grid)
            {
                SurveillanceCamera selected;

                if (_currentController == Inputs.Controller.Playstation || _currentController == Inputs.Controller.Xbox)
                {
                    selected =_eventSystem.currentSelectedGameObject.GetComponentInParent<SurveillanceCamera>();
                }
                else
                {
                    selected = cameras.First(item => item.items.Contains(Input.mousePosition)).items;
                }
                if (selected != null)
                {
                    _fullScreenSystem.SetTarget(selected);
                }
                SystemSwitch(SurveillanceMode.Focused);
            }
        }
        
        public void FocusBack()
        {
            if (_gameController && _gameController.IsGameMenuOpen) return;
            if (mode == SurveillanceMode.Grid)
            {
                SystemSwitch(SurveillanceMode.Focused);
            }
        }

        #region Interface
        
        private void GridInterface()
        {
            _buttons = new List<Button>();
            foreach (var items in cameras)
            {
                OutlineButton b = items.items.gameObject.GetComponentInChildren<OutlineButton>();
                b.colors = colors;
                b.image.sprite = outlineSprite;
                b.onClick.AddListener(Focus);
                _buttons.Add(b);
            }

           /*if (InputManager.GetController() != Inputs.Controller.Other)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_fullScreenSystem.GetTarget().gameObject.GetComponentInChildren<OutlineButton>().gameObject);
            }*/
        }


        private void ActivateGridInterface(bool activate)
        {
            _buttons.ForEach(b => b.gameObject.SetActive(activate));
        }
        
        #endregion

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
            // TODO: Uncomment if the information must be hide
            // _informationsSystem.ActivateInformation(newMode == SurveillanceMode.Focused);
            _onSwitchMethods[mode = newMode]?.Invoke();
            OnModeSwitched?.Invoke();
        }

        private void ExitFullScreen()
        {
            _fullScreenSystem.EscapeFullScreen();
        }

        private void ExitGrid()
        {

            _eventSystem.SetSelectedGameObject(null);
            
            ActivateGridInterface(false);
            _gridIsOpened = false;

        }

        private void OnGrid()
        {
            EnableAll(true);
            ActivateGridInterface(true);
            _gridIsOpened = true;
            if (_currentController == Inputs.Controller.Playstation || _currentController == Inputs.Controller.Xbox)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_fullScreenSystem.GetTarget().gameObject.GetComponentInChildren<OutlineButton>().gameObject);
            }
            else
            {
                _eventSystem.SetSelectedGameObject(null);
            }
            _gridSystem.Grid(cameras.Select(input => input.items.GetCamera()));
        }

        private void OnFullScreen()
        {
            EnableAll(false);
            _fullScreenSystem.RenderFullScreen();
        }

        private void OnControllerTypeChanged()
        {
            Inputs.Controller newController = InputManager.GetController();

            Debug.Log("Controller type changed GeneralSystem");
            
            if (newController == Inputs.Controller.Playstation || newController == Inputs.Controller.Xbox)
            {
                if (mode == SurveillanceMode.Grid)
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(_fullScreenSystem.GetTarget().gameObject.GetComponentInChildren<OutlineButton>().gameObject);
                }
                else if(mode == SurveillanceMode.Focused)
                {
                    _eventSystem.SetSelectedGameObject(null);
                }
            }
            else
            {
                _eventSystem.SetSelectedGameObject(null);
            }
            _currentController = newController;
        }

        private void OnInGameMenuClosed()
        {
            if (_gridIsOpened)
            {
                OnGrid();
            }
        }

        #endregion
    }
}
