using System;
using System.Collections.Generic;
using System.Linq;
using Doors;
using TechSupport.Informations.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;


// TODO: <div>Icons made by <a href="https://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>

namespace TechSupport.Informations
{
    public class InformationsSystem : MonoBehaviour
    {
        [Header("List")] 
        [SerializeField] private Vector2 sizeList;
        private ImageLayout _imageLayout;

        [Header("Accordion")]
        [SerializeField] private Button button;
        [SerializeField] private GameObject informationPanel;
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite front;
        private Animator _animator;
        private Accordion _accordion;
        private List<InformationItem> _items;
        
        [Header("Tech Book")] 
        [SerializeField] private SerializableDictionary<DoorCode.Symbol, Sprite> symbols =
            new SerializableDictionary<DoorCode.Symbol, Sprite>(
                new Dictionary<DoorCode.Symbol, Sprite>
                {
                    { DoorCode.Symbol.One, null },
                    { DoorCode.Symbol.Two, null },
                    { DoorCode.Symbol.Three, null }
                });

        [SerializeField] private Sprite arrow;

        private RectTransform _listRectTransform;
        private RectTransform _accordionRectTransform;

        public void Init()
        {
            _animator = informationPanel.GetComponent<Animator>();
            _items = new List<InformationItem>()
            {
                new TechnicienBook(symbols, arrow)
            };

            CreateList();
        }

        private void Update()
        {
            if (_imageLayout)
                _imageLayout.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.95f, 0);
            if (_accordion)
            {
                _accordion.transform.position = new Vector3(Screen.width * 0.80f, Screen.height * 0.5f);
                _accordionRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 0.25f);
            }
        }
        private void SetSize(RectTransform rectTransform, Vector2 size)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
        private void CreateList()
        {
            _imageLayout = new GameObject().AddComponent<ImageLayout>();
            _listRectTransform = _imageLayout.GetComponent<RectTransform>();
            _listRectTransform.SetParent(transform);
            SetSize(_listRectTransform, sizeList);
        } 

        private void CreateAccordion()
        {
            _accordion = new GameObject().AddComponent<Accordion>();
            _accordionRectTransform = _accordion.GetComponent<RectTransform>();
            _accordionRectTransform.SetParent(transform);
            _accordion.Setup(background, front);
            _accordion.CreateAccordion(_items);
        }

        public ImageLayout GetList()
        {
            return _imageLayout;
        }

        public Accordion GetInformationDisplay()
        {
            return _accordion;
        }

        public void ActvivateInformation(bool activation)
        {
            if (_animator != null)
            {
                _animator.SetBool("Showed", false);
            }
            button.gameObject.SetActive(activation);
        }

        public void OpenInfo()
        {
            if (_animator != null)
            {
                bool isShowed = _animator.GetBool("Showed");
                
                button.gameObject.SetActive(isShowed);
                _animator.SetBool("Showed", !isShowed);
            }
        }
    }
}
