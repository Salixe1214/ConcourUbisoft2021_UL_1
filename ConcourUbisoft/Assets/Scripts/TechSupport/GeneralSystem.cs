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
        [SerializeField] [NotNull] public GameObject tabulationIndicationButton;
        
        [Header("Button to Select camera")]
        [SerializeField] private Sprite outlineSprite;
        [SerializeField] private ColorBlock colors;

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
            cameras.ForEach(c => c.items.Init());
            _gridSystem.Init(cameras.Count());
            GridInterface();
            _fullScreenSystem.SetTarget(cameras.First().items);
            _informationsSystem.Init();
            SystemSwitch(mode);
        }

        public void Escape()
        {
            if (_gameController && _gameController.IsGameMenuOpen) return; 
            if (mode == SurveillanceMode.Focused)
            {
                SystemSwitch(SurveillanceMode.Grid);
            }
        }

        public void Focus()
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
        }

        private void HideGeneralInformation(bool hide)
        {
            //_informationsSystem.GetInformationDisplay().gameObject.SetActive(!hide);
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
            _informationsSystem.ActvivateInformation(newMode == SurveillanceMode.Focused);
            _onSwitchMethods[mode = newMode]?.Invoke();
            OnModeSwitched?.Invoke();
        }

        private void ExitFullScreen()
        {
            tabulationIndicationButton.SetActive(false);
            _fullScreenSystem.EscapeFullScreen();
        }

        private void ExitGrid()
        {
            SurveillanceCamera selected =
                cameras.First(item => item.items.Contains(Input.mousePosition)).items;
            ActivateGridInterface(false);
            HideGeneralInformation(true);
            if (selected != null)
            {
                _fullScreenSystem.SetTarget(selected);
            }
        }

        private void OnGrid()
        {
            EnableAll(true);
            ActivateGridInterface(true);
            HideGeneralInformation(false);
            _gridSystem.Grid(cameras.Select(input => input.items.GetCamera()));
        }

        private void OnFullScreen()
        {
            EnableAll(false);
            tabulationIndicationButton.SetActive(true);
            _fullScreenSystem.RenderFullScreen();
        }

        #endregion
    }
}
